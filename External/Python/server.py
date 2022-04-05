#
#  server.py
#  Created by: William Johnson
#

from threading import Thread
from pubsub import pub
import serial
from _thread import *;
import logging
import zmq
import time
from scipy.stats import norm, chi2
import matplotlib.pyplot as plt
from matplotlib.patches import Ellipse
import numpy as np
from database.dbcalls import db
import math

class Server(Thread):
    """
    Server controls interactions between Python to and from Unity and Python to and from Arduino
    """
    def __init__(self, port, debug= False):
        Thread.__init__(self)
        
        self.arduino = serial.Serial(port=port, baudrate=115200, timeout=.1)
                
        self.debug = debug
        self.end = False
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind("tcp://*:5555")
        pub.subscribe(self.killSwitch, "killSwitch.check")
        pub.subscribe(self.endThread, 'server.end')

    
    def killSwitch(self, message):
        """
        Constant send kill switch message
        """
        if message == "live":
            self.unityWrite("live")
        else:
            self.unityWrite("kill")

    def gatherBalanceData(self):
        """
        Grab sensor data from the arduino
        """
        logging.info("Started Gathering Info From Force Platform")
        time.sleep(1)
        self.arduinoWrite("8")
        balanceData = []
        dataEntry = []
        dataSet = False
        while True:
            data = self.arduinoRead()
            if data == "END":
                break
            elif data == "Done recording":
                dataSet = True
            elif dataSet:
                if data == "1":
                    balanceData.append(dataEntry)
                    dataEntry = []
                else:
                    dataEntry.append(int(data))
        logging.info("Gathered Balance Data")
        return balanceData

    def calculateBalanceScore(self,sensorData):
        """
        Calculate balance score from sensor data
        """
        logging.info("Started Calculating Balance Score")
        points = []
        xComponents = []
        yComponents = []
        times = []
        xComponent = 0
        yComponent = 0
        for i in range(0,len(sensorData),1):
            xComponent = 0
            yComponent = 0
            xComponent += sensorData[i][2] + sensorData[i][4] - (sensorData[i][1] + sensorData[i][3])
            yComponent += sensorData[i][2] + sensorData[i][1] - (sensorData[i][4] + sensorData[i][3])
            point = np.array([xComponent, yComponent])
            points.append(point)
            xComponents.append(xComponent)
            yComponents.append(yComponent)
            times.append(sensorData[i][0])
        
        factor = np.sqrt(np.square(xComponents) + np.square(yComponents))
        meanCOP = np.mean(factor)
        stdCOP = np.std(factor)
        diff = np.square(np.diff(xComponents/np.std(xComponents))) + np.square(np.diff(yComponents/np.std(yComponents)))
        lengthCOP = np.sum(np.sqrt(diff))
        
        centroid = (sum(xComponents) / len(points), sum(yComponents) / len(points))
        
        score = 5 * (meanCOP+stdCOP+lengthCOP)/(abs(centroid[0]) + abs(centroid[1]))
        
        # # Plotting
        # self.plotScore(np.array(points, dtype="int64"))

        logging.info("Finished Calculating Balance Score")
        return score, meanCOP, stdCOP, lengthCOP, centroid[0], centroid[1]
    
    def plotScore(self, points):
        """
        Plot COP over time
        """
        cov = np.cov(points, rowvar=False)
        mean_pos = points.mean(axis=0)
        width1, height1, theta1 = self.cov_ellipse(points, cov, 2)
        width2, height2, theta2 = self.cov_ellipse2(points, cov, 2)
        ax = plt.gca()
        plt.plot(points, 'ro-')
        ellipse1 = Ellipse(xy=mean_pos, width=width1, height=height1, angle=theta1,
                        edgecolor='b', fc='None', lw=2, zorder=4)
        ax.add_patch(ellipse1)
        ellipse2 = Ellipse(xy=mean_pos, width=width2, height=height2, angle=theta2,
                        edgecolor='r', fc='None', lw=.8, zorder=4)
        ax.add_patch(ellipse2)
        plt.show()

    def eigsorted(self,cov):
        '''
        Eigenvalues and eigenvectors of the covariance matrix.
        '''
        vals, vecs = np.linalg.eigh(cov)
        order = vals.argsort()[::-1]
        return vals[order], vecs[:, order]

    def cov_ellipse(self,points, cov, nstd):
        """
        Source: http://stackoverflow.com/a/12321306/1391441
        """

        vals, vecs = self.eigsorted(cov)
        theta = np.degrees(np.arctan2(*vecs[:, 0][::-1]))

        # Width and height are "full" widths, not radius
        width, height = 2 * nstd * np.sqrt(vals)

        return width, height, theta

    def cov_ellipse2(self,points, cov, nstd):
        """
        Source: https://stackoverflow.com/a/39749274/1391441
        """

        vals, vecs = self.eigsorted(cov)
        theta = np.degrees(np.arctan2(*vecs[::-1, 0]))

        # Confidence level
        q = 2 * norm.cdf(nstd) - 1
        r2 = chi2.ppf(q, 2)

        width, height = 2 * np.sqrt(vals * r2)

        return width, height, theta

    def dist(self, p1, p2):
        (x1, y1), (x2, y2) = p1, p2
        s1 = (abs(x2 - x1)) ** 2
        s2 = (abs(y2 - y1)) ** 2
        return math.sqrt(s2 + s1)
    
    # TODO
    def setMotionFloor(self, angle1, angle2):
        return

    def unityRead(self):
        message = self.socket.recv()
        decodedMessage = message.decode("utf-8")
        return decodedMessage
    
    def unityWrite(self, message):
        if self.debug:
            logging.debug(message)
        else:
            try:
                encodedMessage = message.encode()
                self.socket.send(encodedMessage)
            except:
                logging.error("Unity Not Properly Setup")
                
    def arduinoRead(self):
        """
        Read message from arduino
        """
        while True:
            data_raw = self.arduino.readline().decode("ISO-8859-1").strip() 
            if data_raw:
                break

        return data_raw
    
    def arduinoWrite(self, message): 
        """
        Send message to arduino
        """
        if self.debug:
            logging.debug(message)
        else:
            try:
                self.arduino.write(bytes(message, 'utf-8'))
            except:
                logging.error("Arduino Not Properly Setup")
    
    def arduinoShutDown(self):
        self.arduinoWrite("STOP")
    
    def unityShutDown(self):
        self.unityWrite("quit")

    def run(self):
        """
        Behavior for Python server
        """
        logging.info("Starting Server")
        time.sleep(1)
        self.arduinoWrite("3")
        while True and not self.end:     
            if self.debug:
                logging.info("Debug Mode: Server closed")
                time.sleep(1)
                break
            logging.info("Waiting For Message From Unity")
            decodedMessage = self.unityRead()
            logging.info("Message Recieved From Unity: " + decodedMessage)
            if(decodedMessage == "quit"): 
                logging.info("End of Unity Game reached")
                # pub.sendMessage("unityGameEnded")
                self.unityShutDown()
                self.arduinoShutDown()
                break
            elif(decodedMessage.startsWith("calibrate")):
                try:
                    gameScore = decodedMessage.split(" ")
                    
                    # Read values from forceplate
                    balanceData = self.gatherBalanceData()
                    
                    # Calculate balance Score 
                    balanceScore, meanCOP, stdCOP, lengthCOP, centroidX, centroidY = self.calculateBalanceScore(balanceData)

                    # Send message that data has been recieved so game can start again
                    calibrationConfirmation = "calibrate " + balanceScore 
                    self.unityWrite(calibrationConfirmation)

                    # Send scores to database
                    pub.sendMessage('database.GameScore', score=gameScore[1])
                    pub.sendMessage('database.BalanceScore', score=balanceScore, gameID=gameScore[2],meanCOP=meanCOP, stdCOP=stdCOP,
                                    lengthCOP=lengthCOP, centroidX=centroidX, centroidY=centroidY)
                except:
                    logging.error("Error occured while parsing data")
                    self.unityWrite("error " + decodedMessage)
            elif(decodedMessage.startsWith("rotation")):
                try:
                    # Get rotation values
                    rotationList = decodedMessage.split(" ")
                   
                    # Set Motion Floor Platform to these angles
                    self.setMotionFloor(rotationList[1],rotationList[2])
                    
                    # Send angles back to Unity Game as confirmation
                    rotationConfirmation = "rotation " + str(rotationList[1]) + " " + str(rotationList[1]) 
                    self.unityWrite(rotationConfirmation)
                except:
                    logging.error("Error occured while parsing data")
                    self.unityWrite("error " + decodedMessage)
    
    def endThread(self):
        """
        End Server Thread
        """
        logging.info("Ending Server Thread")
        self.end = True

if __name__ == "__main__":
    port = "tcp://*:5555"
    server = Server(debug=True)
    score = None
    setupError = 0
    logging.basicConfig(
        format='%(asctime)s %(levelname)-8s %(message)s',
        level=logging.INFO,
        datefmt='%Y-%m-%d %H:%M:%S')

    data = [
        [950, 105.5, 103, 102, 101],
        [955, 100, 105, 100, 100],
        [960, 103, 102, 105, 101],
        [965, 100, 100, 100, 105],
        [970, 117, 100, 100, 100],
        [975, 100, 110, 105, 110],
        [980, 100, 100, 110, 102],
        [985, 102, 100, 100, 110],
        [990, 115, 100, 100, 100],
        [995, 100, 115, 107, 106],
        [1000, 100, 106, 115, 100]
    ]

    data2 = [
        [950, 1006, 1034, 1027, 1023],
        [955, 1000, 1055, 1003, 1003],
        [960, 1003, 1026, 1051, 1015],
        [965, 1000, 1000, 1003, 1057],
        [970, 1170, 1000, 1009, 1001],
        [975, 1000, 1100, 1056, 1102],
        [980, 1000, 1000, 1109, 1023],
        [985, 1020, 1000, 1006, 1104],
        [990, 1150, 1000, 1003, 1005],
        [995, 1000, 1150, 1072, 1065]
    ]

    data3 = [
        [950, 1, 1, 1, 1],
        [955, 1.2, 1.1, 1.1, 1.1],
        [960, 1, 1, 1, 1],
        [965, 1.2, 1.1, 1.1, 1.1],
        [970, 1, 1, 1, 1],
        [975, 1.2, 1.1, 1.1, 1.1],
        [980, 1, 1, 1, 1],
        [985, 1.2, 1.1, 1.1, 1.1],
        [990, 1, 1, 1, 1],
        [995, 1.2, 1.1, 1.1, 1.1]
    ]
    database = db()
    balanceScore, meanCOP, stdCOP, lengthCOP, centroidX, centroidY = server.calculateBalanceScore(data)
    database.addBalanceScore(4,2,balanceScore,meanCOP,stdCOP,lengthCOP,centroidX,centroidY)
    balanceScore, meanCOP, stdCOP, lengthCOP, centroidX, centroidY = server.calculateBalanceScore(data2)
    database.addBalanceScore(2,1,balanceScore,meanCOP,stdCOP,lengthCOP,centroidX,centroidY)
    balanceScore, meanCOP, stdCOP, lengthCOP, centroidX, centroidY = server.calculateBalanceScore(data3)
    database.addBalanceScore(1,2,balanceScore,meanCOP,stdCOP,lengthCOP,centroidX,centroidY)
    
    if setupError == 0:
        logging.info("Starting Server")
        time.sleep(1)
        server.arduinoWrite("3")
        while True :
            break
            logging.info("Waiting For Message From Unity")
            decodedMessage = server.unityRead()
            logging.info("Message Recieved From Unity: " +decodedMessage)
            if(decodedMessage == "endGame"):
                logging.info("End of Mini-Game Reached")
                # TODO: Send game score to unity
                server.unityWrite(str(score))
                logging.info("Score Sent To Unity: " + score)
                score = None
            elif(decodedMessage == "closeApp"): 
                # TODO: Used for ending script
                logging.info("End of application reached")
                break
            elif(decodedMessage == "readScore"):
                # TODO: Read values from database
                logging.info("Read Values From Force Plate")
                
                
                balanceData = server.gatherBalanceData()
                # Send message that data has been recieved so game can start again
                server.unityWrite("ScoreGathered")

                # Calculate Score algorithm 
                score = server.calculateBalanceScore(balanceData)
            else:
                try:
                    posList = decodedMessage.split(" ")
                    # TODO: Set Motion Floor Platform to these angles
                    server.setMotionFloor(posList[0],posList[1])
                    # Send angles back to Unity Game as confirmation
                    unityAngles = str(posList[0]) + " " + str(posList[1]) 
                    server.unityWrite(unityAngles)
                except:
                    logging.error("Error occured while parsing data")
                    server.unityWrite(decodedMessage)
            break
