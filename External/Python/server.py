#
#  server.py
#  Created by: William Johnson
#

from pip import main
import serial
from _thread import *;
import logging
import zmq
import time
from itertools import count
from multiprocessing import Process
import matplotlib.pyplot as plt

data = [
    [None, 1, 7.0],
    [None, 2, 5.0],
    [None, 3, 3.0],
    [None, 4, 1.0],
    [None, 1, 4.0],
    [None, 2, 4.0],
    [None, 3, 4.0],
    [None, 4, 4.0],
    [None, 1, 2.0],
    [None, 2, 4.0],
    [None, 3, 6.0],
    [None, 4, 8.0],
    [None, 1, 3.0],
    [None, 2, 5.0],
    [None, 3, 8.0],
    [None, 4, 7.0],
    [None, 1, 2.0],
    [None, 2, 3.0],
    [None, 3, 3.0],
    [None, 4, 9.0],
    [None, 1, 5.0],
    [None, 2, 7.0],
    [None, 3, 4.0],
    [None, 4, 3.0],
    [None, 1, 3.0],
    [None, 2, 2.0],
    [None, 3, 1.0],
    [None, 4, 4.0],
    [None, 1, 6.0],
    [None, 2, 7.0],
    [None, 3, 4.0],
    [None, 4, 1.0],


]
class Server():
    def __init__(self, port):
        # self.arduino = serial.Serial(port='COM4', baudrate=115200, timeout=.1)    
        self.arduino = None
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind(port)
        self.score = None;

    def confirmUnityConnection(self):
        self.unityWrite("Setup")
        self.unityRead()
        return True
    
    def confirmArduinoConnection(self):
        self.arduinoWrite("START")
        self.arduinoRead()
        return True

    # TODO
    def gatherBalanceData(self):
        print("Going to gather info from force platform")
        self.arduinoWrite("START")
        # data = self.arduino.readline()
        return

    # TODO
    def calculateBalanceScore(self,sensorData):
        print("Determine Balance" + sensorData)
        return
    
    def plotSensorData(self,sensorData):
        xComponents = []
        yComponents = []
        xComponent = 0
        yComponent = 0
        for i in range(0,len(sensorData),1):
            if sensorData[i][1] == 1:
                xComponent -= sensorData[i][2]
                yComponent += sensorData[i][2]
            elif sensorData[i][1] == 2:
                xComponent += sensorData[i][2]
                yComponent += sensorData[i][2]
            elif sensorData[i][1] == 3:
                xComponent -= sensorData[i][2]
                yComponent -= sensorData[i][2]
            elif sensorData[i][1] == 4:
                xComponent += sensorData[i][2]
                yComponent -= sensorData[i][2]
                xComponents.append(xComponent)
                yComponents.append(yComponent)
                xComponent = 0
                yComponent = 0
            else:
                logging.error("Error in reading data")
                print(sensorData[i])
        plt.plot(xComponents, yComponents, 'ro-')
        plt.show()

    # TODO
    def setMotionFloor(self, angle1, angle2):
        return

    def unityRead(self):
        message = self.socket.recv()
        decodedMessage = message.decode("utf-8")
        return decodedMessage
    
    def unityWrite(self, message):
        encodedMessage = message.encode()
        self.socket.send(encodedMessage)
        
    def arduinoRead(self): 
        data = self.arduino.readline()
        return data
    
    def arduinoWrite(self, message): # START SEND STOP
        self.arduino.write(bytes(message, 'utf-8'))
    
    def shutDown(self):
        self.arduinoWrite("STOP")

if __name__ == "__main__":
    port = "tcp://*:5555"
    server = Server(port)
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
    if setupError == 0:
        logging.info("Starting Server")
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
