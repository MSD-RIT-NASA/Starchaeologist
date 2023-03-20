#
#  server.py
#  Created by: William Johnson

import os
import csv
import subprocess
from threading import Thread
from pubsub import pub
import serial
import multiprocessing
import threading
import random
# import openpyxl
from _thread import *;
import matlab.engine
import os, os.path
import ctypes
#from editpyxl import Workbook
import logging
import win32com.client

# from System.Runtime.InteropServices import Marshal
import zmq
import time
import win32com
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
        # try:
        # self.arduino = serial.Serial(port=port, baudrate=115200, timeout=.1)
        # except:
            # logging.error("Error when setting up Arduino")    
        self.debug = debug
        self.end = False
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind("tcp://*:5678")
        # context.term()
        # ln = pyz.createLink() # DDE link object
        
        # workbook = ExcelApp.Workbooks.Open(r"External/Excel/OPC Data.xlsm")
        

        # self.wb = Workbook()

        # self.source_filename = r'External/Excel/OPC Data.xlsm'

        # self.wb.open(self.source_filename)

        # self.ws = self.wb.active

        
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
        self.plotScore(np.array(points, dtype="int64"))

        logging.info("Finished Calculating Balance Score")
        return score, meanCOP, stdCOP, lengthCOP, centroid[0], centroid[1]
    
    def plotScore(self, points):
        """
        Plot Center Of Pressure points over time
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
        """

        vals, vecs = self.eigsorted(cov)
        theta = np.degrees(np.arctan2(*vecs[:, 0][::-1]))

        # Width and height are "full" widths, not radius
        width, height = 2 * nstd * np.sqrt(vals)

        return width, height, theta

    def cov_ellipse2(self, cov, nstd):
        """
        """

        vals, vecs = self.eigsorted(cov)
        theta = np.degrees(np.arctan2(*vecs[::-1, 0]))

        # Confidence level
        q = 2 * norm.cdf(nstd) - 1
        r2 = chi2.ppf(q, 2)

        width, height = 2 * np.sqrt(vals * r2)

        return width, height, theta

    def dist(self, p1, p2):
        """
        Find the distance between two 2D points
        """
        (x1, y1), (x2, y2) = p1, p2
        s1 = (abs(x2 - x1)) ** 2
        s2 = (abs(y2 - y1)) ** 2
        return math.sqrt(s2 + s1)
    
    #def setMotionFloor(self, angle1, angle2):
    #    """
    #    Set motion floor positions for the actuators based on the given angles 
    #    """
    #    x1 = 1.096 * math.sin(math.radians(angle1))
    #    pos1 = (-.125 - x1) * 100000
    #    x2 = 1.096 * math.sin(math.radians(angle2))
    #    pos2 = (-.125 + x2) * 100000
    #    ExcelApp = win32com.client.GetActiveObject("Excel.Application")
    #    ExcelApp.Visible = True
    #    ExcelApp.Range("B4").Value = [pos1]
    #    ExcelApp.Range("B13").Value = [pos2]

    def unityRead(self):
        """
        Read data from unity
        """
        message = self.socket.recv()
        decodedMessage = message.decode("utf-8")
        return decodedMessage
    
    def unityWrite(self, message):
        """
        Send message to unity
        """
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
        """
        End constant reading mode in arduino
        """
        self.arduinoWrite("STOP")
    
    def unityShutDown(self):
        """
        Inform UnityGame that the program is being shutdown
        """
        self.unityWrite("quit")

    def run(self):
        """
        Behavior for the Python server
        """
        logging.info("Starting Server")
        time.sleep(1)
        self.arduinoWrite("3")
        while True and not self.end:     
            if self.debug:
                logging.info("Debug Mode: Server closed")
                time.sleep(1)
                break
            # Constantly read message from Unity
            logging.info("Waiting For Message From Unity")
            decodedMessage = self.unityRead()
            logging.info("Message Recieved From Unity: " + decodedMessage)
            if(decodedMessage == "quit"): 
                logging.info("End of Unity Game reached")
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
                    calibrationConfirmation = "calibrate " + balanceScore + " meanCOP " + meanCOP + " stdCOP " + stdCOP + " lengthCOP " + lengthCOP + " centroidX " + centroidX + " centroidY " + centroidY
                    self.unityWrite(calibrationConfirmation)

                    # Send scores to database
                    pub.sendMessage('database.GameScore', score=gameScore[1])
                    pub.sendMessage('database.BalanceScore', score=balanceScore, gameID=gameScore[2],meanCOP=meanCOP, stdCOP=stdCOP,
                                    lengthCOP=lengthCOP, centroidX=centroidX, centroidY=centroidY)
                except:
                    logging.error("Error occured while parsing data")
                    self.unityWrite("error " + decodedMessage)
            elif(decodedMessage.startswith("rotation")):
                try:
                    # Get rotation values
                    
                    if (rotationCount == 5):
                        rotationList = decodedMessage.split(" ")
                        # Set Motion Floor Platform to these angles
                        #self.setMotionFloor(float(rotationList[1]),float(rotationList[2]))
                        
                        # Send angles back to Unity Game as confirmation
                        rotationConfirmation = "rotation " + str(rotationList[1]) + " " + str(rotationList[1]) 
                        self.unityWrite(rotationConfirmation)
                        rotationCount = 0
                    rotationCount+=1
                except:
                    logging.error("Error occured while parsing data")
                    self.unityWrite("error " + decodedMessage)
    
    def endThread(self):
        """
        End Server Thread
        """
        logging.info("Ending Server Thread")
        self.end = True

# Testing for the server's motion control
if __name__ == "__main__":
    server = Server(debug=False, port="COM4")
    score = None

    
    logging.basicConfig(level=logging.INFO, 
        format='%(asctime)s.%(msecs)03d %(levelname)s:\t%(message)s',
        datefmt='%Y-%m-%d %H:%M:%S'
    )
    for i in range(-5,5,1):
        print(i)
        #server.setMotionFloor(i,i+0.1)
        time.sleep(0.01)

    Server.start(server)

    while True:
        #server.setMotionFloor(random.uniform(-5, 5),random.uniform(-5, 5))
        time.sleep(0.01)
    