#
#  server.py
#  Created by: William Johnson
#

from threading import Thread
from pip import main
from pubsub import pub
import serial
from _thread import *;
import logging
import zmq
import time
from itertools import count
from multiprocessing import Process
from scipy.stats import norm, chi2
import matplotlib.pyplot as plt
from matplotlib.patches import Ellipse
import numpy as np
from scipy.spatial import ConvexHull
from scipy.spatial.distance import cdist
from scipy.stats import t
import math

# from scipy.stats import ppf
from itertools import combinations


class Server(Thread):
    def __init__(self, debug):
        Thread.__init__(self)
        if debug:
            self.arduino = None
        else:
            self.arduino = serial.Serial(port='COM3', baudrate=115200, timeout=.1)
                
        self.debug = debug
        self.end = False
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind("tcp://*:5555")
        self.score = None;
        pub.subscribe(self.killSwitch, "killSwitch.check")
        pub.subscribe(self.endThread, 'server.end')

    
    def killSwitch(self, message):
        if message == "live":
            if self.debug == False:
                self.unityWrite("live")
        else:
            self.unityWrite("kill")

    def gatherBalanceData(self):
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

    # TODO
    def calculateBalanceScore(self,sensorData):
        print(sensorData)
        points = []
        xComponents = []
        yComponents = []
        xComponent = 0
        yComponent = 0
        for i in range(0,len(sensorData),1):
            xComponent = 0
            yComponent = 0
            xComponent += sensorData[i][1] + sensorData[i][3] - (sensorData[i][0] + sensorData[i][2])
            yComponent += sensorData[i][1] + sensorData[i][0] - (sensorData[i][3] + sensorData[i][2])
            
            point = np.array([xComponent, yComponent])
            points.append(point)
            xComponents.append(xComponent)
            yComponents.append(yComponent)
            
        points = np.array(points, dtype='int64')
        hull = ConvexHull(points)
        area = hull.area
        
        m = points.mean()
        s = points.std() 
        
        dof = len(points)-1    
        confidence = 0.75

        t_crit = np.abs(t.ppf((1-confidence)/2,dof))
        lowerRange = m-s*t_crit/np.sqrt(len(points)) 
        upperRange = m+s*t_crit/np.sqrt(len(points)) 
        
        ranges = upperRange-lowerRange
        
        distances = [self.dist(p1, p2) for p1, p2 in combinations(points, 2)]
        avg_distance = sum(distances) / len(distances)
        std_distance = np.std(distances)
        
        # score = round(75/avg_distance + 75/std_distance + 75/ranges + 75/area)
        
        score = round((area)/(avg_distance + std_distance + ranges) * 100)
        
        logging.info("Calculated Balance Score")
        
        # Plotting
        # cov = np.cov(points, rowvar=False)
        # mean_pos = points.mean(axis=0)
        # width1, height1, theta1 = self.cov_ellipse(points, cov, 2)
        # width2, height2, theta2 = self.cov_ellipse2(points, cov, 2)
        # ax = plt.gca()
        # plt.plot(xComponents,yComponents, 'ro-')
        # ellipse1 = Ellipse(xy=mean_pos, width=width1, height=height1, angle=theta1,
        #                 edgecolor='b', fc='None', lw=2, zorder=4)
        # ax.add_patch(ellipse1)
        # ellipse2 = Ellipse(xy=mean_pos, width=width2, height=height2, angle=theta2,
        #                 edgecolor='r', fc='None', lw=.8, zorder=4)
        # ax.add_patch(ellipse2)
        # plt.show()
        # hullpoints = points[hull.vertices,:]
        # hdist = cdist(hullpoints, hullpoints, metric='euclidean')
        # bestpair = np.unravel_index(hdist.argmax(), hdist.shape)
        # plt.show()

        return score
    

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
        while True:
            data_raw = self.arduino.readline().decode("ISO-8859-1").strip() 
            if data_raw:
                break

        return data_raw
    
    def arduinoWrite(self, message): # START SEND STOP
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
        logging.info("Starting Server")
        time.sleep(1)
        self.arduinoWrite("3")
        while True and not self.end:     
            if self.debug:
                logging.info("Server in debug mode")
                time.sleep(1)
                break
            logging.info("Waiting For Message From Unity")
            decodedMessage = self.unityRead()
            logging.info("Message Recieved From Unity: " + decodedMessage)
            if(decodedMessage == "quit"): 
                # TODO: Used for ending script
                print("End of Unity Game reached")
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
                    balanceScore = self.calculateBalanceScore(balanceData)

                    # Send message that data has been recieved so game can start again
                    calibrationConfirmation = "calibrate " + balanceScore 
                    self.unityWrite(calibrationConfirmation)

                    # Send scores to database
                    pub.sendMessage('database.GameScore', score=gameScore[1])
                    pub.sendMessage('database.BalanceScore', score=balanceScore)
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

    logging.info("Confirming Force Platform Connection")
    # server.confirmArduinoConnection()

    logging.info("Confirming Unity Connection")
    # server.confirmUnityConnection()
    
    # server.plotSensorData(data)
    # server.plotSensorData(data2)
    # server.plotSensorData(data3)
    # server.arduinoWrite(port.encode())
    # text = server.arduinoRead()
    # print(text)
    # logging.info(text)
    # text = server.arduinoRead()
    # print(text)
    # logging.info(text)
    if setupError == 0:
        logging.info("Starting Server")
        time.sleep(1)
        server.arduinoWrite("3")
        while True :
            # time.sleep(1)
            # server.arduinoWrite("3")
            decodedMessage = "readScore"
            # logging.info("Waiting For Message From Unity")
            # decodedMessage = server.unityRead()
            # logging.info("Message Recieved From Unity: " +decodedMessage)
            if(decodedMessage == "endGame"):
                logging.info("End of Mini-Game Reached")
                # TODO: Send game score to unity
                server.unityWrite(str(score))
                logging.info("Score Sent To Unity: " + score)
                score = None
            elif(decodedMessage == "closeApp"): 
                # TODO: Used for ending script
                print("End of application reached")
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
