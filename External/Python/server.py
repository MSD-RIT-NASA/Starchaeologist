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


data = [
    [None, 2.13465207868007,4.89956174599918,2.66060181598207,7.85470370475888],
    [None, 4.28587711941725,3.87793918274247,9.30913868002088,5.61656090739023],
    [None, 5.7841504070834,4.20312918219861,2.35600260801427,7.57694010975628],
    [None, 9.54939253281308,3.75112311297614,9.99550894414797,9.71744358433292],
    [None, 5.60526204137377,3.69543214267838,3.09467786787761,9.71541751255999],
    [None, 4.15204880673068,5.39928982658279,3.79305433099766,7.38310103275958],
    [None, 8.79014501571196,3.85258397034024,8.30697977184643,6.75868917245357],
    [None, 4.82898640997195,9.63078790140841,3.9874571068154,9.37223417562071],
    [None, 4.55177440659691,4.31705308534068,1.83755879515668,3.39807878453195],
    [None, 8.64012769173837,7.05287374185998,8.30119405933711,3.20412490712671]
]
data2 = [
    [None, 1.29759106333255,1.41730118422643,0.883845783715996,2.04727100536566],
    [None, 0.683016567343388,2.21436281093134,2.80327027281899,2.65911087471019],
    [None,1.86047466185897,1.55068611472411,4.50416328362383,4.46881027075872],
    [None,3.27897346265566,2.9918062169998,0.499271904816512,2.08878341926671],
    [None,1.81902880771972,4.18124504675215,2.09451893255791,1.60953734703806],
    [None,2.03891879834184,0.735982816077761,2.35437806339673,1.8513676393085],
    [None,0.893243843174187,4.27308916406384,1.61331337672347,1.72286500210076],
    [None,1.82488548980322,3.81299511194834,1.54527430075466,4.59720880891998],
    [None,1.42773845578904,4.70682336609197,4.2455909113612,1.19746566386775],
    [None,3.6725038297812,4.78532591452139,2.17495555718194,4.35049065917288]
]

data3 = [
    [None, 2.5,2.6,2.5,2.5],
    [None, 2.5,2.5,2.6,2.5],
    [None, 2.5,2.5,2.5,2.6],
    [None, 2.6,2.5,2.5,2.5],
    [None, 2.5,2.7,2.5,2.5],
    [None, 2.5,2.5,2.7,2.5],
    [None, 2.5,2.5,2.5,2.7],
    [None, 2.7,2.5,2.5,2.5],
    [None, 2.5,2.8,2.5,2.5],
]
# def mean_confidence_interval(data, confidence=0.95):
#     a = 1.0 * np.array(data)
#     n = len(a)
#     m, se = np.mean(a), sem(a)
#     h = se * scipy.stats.t.ppf((1 + confidence) / 2., n-1)
#     return m, m-h, m+h

class Server(Thread):
    def __init__(self, debug):
        Thread.__init__(self)
        if debug:
            # self.arduino = serial.Serial(port='COM5', baudrate=115200, timeout=.1)
            self.arduino = None
        else:
            self.arduino = serial.Serial(port='COM5', baudrate=115200, timeout=.1)
                
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

    # TODO
    def gatherBalanceData(self):
        print("Going to gather info from force platform")
        self.arduinoWrite("3")
        balanceData = []
        while True:
            data = self.arduino.readline()
            if data.decode("utf-8") == "END":
                break
            # Edit the data, so easily readable later
            balanceData.append(data)
        return balanceData

    # TODO
    def calculateBalanceScore(self,sensorData):
        print("Determine Balance" + sensorData)
        # Return array of different attributes, first value total, others are the attributes
        return [0]
    
    def plotSensorData(self,sensorData):
        points = []
        xComponents = []
        yComponents = []
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
            
        # plt.plot(xComponents,yComponents, 'ro-')
        points = np.array(points)
        hull = ConvexHull(points)
        area = hull.area
        # print("Hull Area: " + str(area)) # Can be used for equation

        # cov = np.cov(points, rowvar=False)

        m = points.mean()
        s = points.std() 
        # print("Standard Deviation: " + str(s))
        dof = len(points)-1    
        confidence = 0.75

        t_crit = np.abs(t.ppf((1-confidence)/2,dof))
        lowerRange = m-s*t_crit/np.sqrt(len(points)) 
        upperRange = m+s*t_crit/np.sqrt(len(points)) 
        # print(lowerRange)
        # print(upperRange)
        ranges = upperRange-lowerRange
        # print("Range: " + str(ranges))
        # newpoints = list(zip(xComponents,yComponents))
        distances = [self.dist(p1, p2) for p1, p2 in combinations(points, 2)]
        avg_distance = sum(distances) / len(distances)
        std_distance = np.std(distances)
        # print("Avg STD Distance: " + str(std_distance))
        # print("Avg Distance: " + str(avg_distance))
        # np.percentile(points,[100*(1-confidence)/2,100*(1-(1-confidence)/2)]) 
        score = round(75/avg_distance + 75/std_distance + 75/ranges + 75/area) 
        print("Score: " + str(score))

        # Location of the center of the ellipse.
        # mean_pos = points.mean(axis=0)

        # # METHOD 1
        # width1, height1, theta1 = self.cov_ellipse(points, cov, 2)

        # # METHOD 2
        # width2, height2, theta2 = self.cov_ellipse2(points, cov, 2)
        # ax = plt.gca()
        # plt.plot(xComponents,yComponents, 'ro-')
        # # plt.scatter(xComponents, yComponents, c='k', s=1, alpha=.5)
        # # First ellipse
        # ellipse1 = Ellipse(xy=mean_pos, width=width1, height=height1, angle=theta1,
        #                 edgecolor='b', fc='None', lw=2, zorder=4)
        # # ellipse1.area
        # ax.add_patch(ellipse1)
        # # Second ellipse
        # ellipse2 = Ellipse(xy=mean_pos, width=width2, height=height2, angle=theta2,
        #                 edgecolor='r', fc='None', lw=.8, zorder=4)
        # ax.add_patch(ellipse2)
        # plt.show()
        # hullpoints = points[hull.vertices,:]
        # # Naive way of finding the best pair in O(H^2) time if H is number of points on
        # # hull
        # hdist = cdist(hullpoints, hullpoints, metric='euclidean')
        # # print(hdist)
        # # Get the farthest apart points
        # bestpair = np.unravel_index(hdist.argmax(), hdist.shape)
        # # print(bestpair)
        # #Print them
        # # print([hullpoints[bestpair[0]],hullpoints[bestpair[1]]])
        # plt.show()

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
        return math.sqrt((x2 - x1)**2 + (y2 - y1)**2)
    # TODO
    def setMotionFloor(self, angle1, angle2):
        return

    def unityRead(self):
        message = self.socket.recv()
        decodedMessage = message.decode("utf-8")
        return decodedMessage
    
    def unityWrite(self, message):
        if self.debug:
            print(message)
        else:
            encodedMessage = message.encode()
            self.socket.send(encodedMessage)
        
    def arduinoRead(self): 
        data = self.arduino.readline()
        return data
    
    def arduinoWrite(self, message): # START SEND STOP
        if self.debug:
            print(message)
        else:
            self.arduino.write(bytes(message, 'utf-8'))
    
    def shutDown(self):
        self.arduinoWrite("STOP")

    def run(self):
        logging.info("Starting Server")
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
                pub.sendMessage("unityGameEnded")
                break
            elif(decodedMessage.startsWith("calibrate")):
                try:
                    gameScore = decodedMessage.split(" ")
                    
                    # Read values from forceplate
                    logging.info("Read Values From Force Plate")
                    balanceData = self.gatherBalanceData()
                    
                    # Calculate balance Score 
                    balanceScore = self.calculateBalanceScore(balanceData)

                    # Send message that data has been recieved so game can start again
                    calibrationConfirmation = "calibrate " + balanceScore[0]  
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
    
    server.plotSensorData(data)
    server.plotSensorData(data2)
    server.plotSensorData(data3)
    # server.arduinoWrite(port.encode())
    # text = server.arduinoRead()
    # print(text)
    # logging.info(text)
    # text = server.arduinoRead()
    # print(text)
    # logging.info(text)
    if setupError == 0:
        # logging.info("Starting Server")
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
                print("End of application reached")
                break
            elif(decodedMessage == "readScore"):
                # TODO: Read values from database
                logging.info("Going to Read Values From Force Plate")
                
                
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
