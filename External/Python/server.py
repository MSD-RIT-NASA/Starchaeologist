#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

from pip import main
import serial
import time
import zmq
class Server:
    def __init__(self):
        self.arduino = serial.Serial(port='COM4', baudrate=115200, timeout=.1)    
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind("tcp://*:5555")


    def arduinoRead(self):
        data = self.arduino.readline()
        return data
        
    def unityRead(self):
        message = self.socket.recv()
        comb = message.decode("utf-8")
        posList = comb.split(" ")
        return posList[0], posList[1]
    
    def unityWrite(self, message):
        self.socket.send(message)

if __name__ == "__main__":
    s = Server()
    while(True):
        angle1, angle2 = s.unityRead()
        print("Position 1: " + angle1)
        print("Position 2: " + angle1)
        # TODO: Set Motion Floor Platform to these angles

        # Send angles back to Unity Game as confirmation
        comb = str(angle1) + " " + str(angle1) 
        s.unityWrite(comb.encode())