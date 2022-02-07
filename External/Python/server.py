#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

import serial
import time
import zmq


arduino = serial.Serial(port='COM4', baudrate=115200, timeout=.1)    
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")


def write_read(x):
    arduino.write(bytes(x, 'utf-8'))
    data = arduino.readline()
    return data
while True:
    message = socket.recv()
    print("Received request: %s" % message)
    socket.send(b"World")