# Created by Youssef Elashry to allow two-way communication between Python3 and Unity to send and receive strings

# Feel free to use this in your individual or commercial projects BUT make sure to reference me as: Two-way communication between Python 3 and Unity (C#) - Y. T. Elashry
# It would be appreciated if you send me how you have used this in your projects (e.g. Machine Learning) at youssef.elashry@gmail.com

# Use at your own risk
# Use under the Apache License 2.0

# Example of a Python UDP server

# NASA x RIT author: Angela Hudak


import UdpComms as U
import time
import logging
import socket
import math

UDP_IP= "192.168.4.4"
UDP_PORT = 4210
MESSAGE = "Hello, World!"


# Create UDP socket to use for sending (and receiving)
sock = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001, enableRX=True, suppressWarnings=True)


boardSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
boardSock.sendto(bytes(MESSAGE, "utf-8"), (UDP_IP, UDP_PORT))
boardSock.setblocking(0) # allows the program to pass the blocking recvfrom() for the board

logging.basicConfig(level=logging.INFO, 
    format='%(asctime)s.%(msecs)03d %(levelname)s:\t%(message)s',
    datefmt='%Y-%m-%d %H:%M:%S')


logging.info("Starting Server")
time.sleep(1)

while True: 
    # Constantly read message from Unity
    #logging.info("Waiting For Message From Unity")
    decodedMessage = sock.ReadReceivedData() # read data
    print(decodedMessage)

    # For checking for the board sensor in the minecart level
    # then sends board data
    try: 
        boardMsg = boardSock.recvfrom(16)
        value = boardMsg[0].decode('utf-8')
        try:
            newval = float(value.replace('\U00002013', '-'))*360/math.pi
            sock.SendData("boardMove " + str(newval))
            print(newval)
        except ValueError:
            print ("Not a float")
            pass
    except socket.timeout: 
        #print("timeout!!")
        pass
    except BlockingIOError: # when board sensor is not connected
        #print("blocked!!")
        pass
    

    # decode message from unity
    if(decodedMessage == "quit"):
        logging.info("End of Unity Game reached")
        sock.unityShutDown()               
        break

    elif(decodedMessage == "startCalibrating"):
        print("Game is trying to calibrate")
        getCalibration = U.UdpComms.sensorCalibration()
        if (getCalibration): 
            sock.SendData("calibratedRigsuccess")
        else:
            sock.SendData("calibratedRigFailed")

    elif(decodedMessage == "testing"):
        print("Testing the communication")
        sock.SendData("testingPython 5555.00")
          
