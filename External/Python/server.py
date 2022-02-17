#
#  server.py
#  Created by: William Johnson
#

from pip import main
# import serial
import time
from _thread import *;
from threading import Thread, Lock;
import logging
import zmq

class ServerThread(Thread):
    def __init__(self, port):
        # self.arduino = serial.Serial(port='COM4', baudrate=115200, timeout=.1)    
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind(port)
        self.lock = Lock()
        self.score = None;

    def calculateBalanceScore(self,sensorData):
        print("Determine sensorData" + sensorData)
    
    def setMotionFloor(self, angle1, angle2):
        print("Angle1: " +angle1)
        print("Angle2: " +angle2)

    # def arduinoRead(self):
    #    data = self.arduino.readline()
    #    return data

    def run(self):
        while True :
            # Recieve messages from the Unity Game
            message = self.socket.recv()
            decodedMessage = message.decode("utf-8")
            if(decodedMessage == "endGame"):
                # TODO: Send game score to unity
                print("End of game reached")
            elif(decodedMessage == "closeGame"):
                # TODO: Send game score to unity
                print("End of application reached")
                break
            elif(decodedMessage == "readScore"):
                # TODO: Read values from database
                print("Data recieved")
                # Send message that data has been recieved so game can start again
                
                # Put array in algorithm 
                self.score = self.calculateBalanceScore()

            else:
                posList = decodedMessage.split(" ")
                # TODO: Set Motion Floor Platform to these angles

                # Send angles back to Unity Game as confirmation
                unityAngles = str(posList[0]) + " " + str(posList[1]) 
                encodedMessage = unityAngles.encode()
                self.socket.send(encodedMessage)
    
        
    

if __name__ == "__main__":
    port = "tcp://*:5555"
    server = ServerThread(port)
    server.start()
    server.join()