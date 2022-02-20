#
#  server.py
#  Created by: William Johnson
#

from pip import main
# import serial
import time
from _thread import *;
import logging
import zmq

class Server():
    def __init__(self, port):
        # self.arduino = serial.Serial(port='COM4', baudrate=115200, timeout=.1)    
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind(port)
        self.score = None;

    # TODO
    def calculateBalanceScore(self,sensorData):
        print("Determine sensorData" + sensorData)
    
    # TODO
    def setMotionFloor(self, angle1, angle2):
        print("Angle1: " +angle1)
        print("Angle2: " +angle2)

    def unityRead(self):
        message = self.socket.recv()
        decodedMessage = message.decode("utf-8")
        return decodedMessage
    
    def unityWrite(self, message):
        encodedMessage = message.encode()
        self.socket.send(encodedMessage)

    # def arduinoRead(self):
    #    data = self.arduino.readline()
    #    return data
    

if __name__ == "__main__":
    port = "tcp://*:5555"
    server = Server(port)
    score = None
    while True :
        # Recieve messages from the Unity Game
        decodedMessage = server.unityRead()
        if(decodedMessage == "endGame"):
            # TODO: Send game score to unity
            print("End of game reached")
            score = None
        elif(decodedMessage == "closeGame"):
            # TODO: Send game score to unity
            print("End of application reached")
            break
        elif(decodedMessage == "readScore"):
            # TODO: Read values from database
            print("Data recieved")
            # Send message that data has been recieved so game can start again
            
            # Put array in algorithm 
            score = server.calculateBalanceScore()

        else:
            posList = decodedMessage.split(" ")
            # TODO: Set Motion Floor Platform to these angles

            # Send angles back to Unity Game as confirmation
            unityAngles = str(posList[0]) + " " + str(posList[1]) 
            server.unityRead(unityAngles)