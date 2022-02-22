#
#  server.py
#  Created by: William Johnson
#

from pip import main
import serial
from _thread import *;
import logging
import zmq

class Server():
    def __init__(self, port):
        self.arduino = serial.Serial(port='COM4', baudrate=115200, timeout=.1)    
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind(port)
        self.score = None;

    # TODO
    def gatherBalanceData(self):
        # print("Going to gather info from force platform")
        # data = self.arduino.readline()
        return

    # TODO
    def calculateBalanceScore(self,sensorData):
        print("Determine Balance" + sensorData)
        return
    
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

    # def arduinoRead(self):
    #    data = self.arduino.readline()
    #    return data
    

if __name__ == "__main__":
    port = "tcp://*:5555"
    server = Server(port)
    score = None
    logging.basicConfig(
        format='%(asctime)s %(levelname)-8s %(message)s',
        level=logging.INFO,
        datefmt='%Y-%m-%d %H:%M:%S')
    logging.info("Starting Server")
    while True :
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