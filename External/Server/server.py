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

# Create Socket to send and receive data from Board sensor
UDP_IP= "192.168.4.2"
UDP_PORT = 4210
MESSAGE = "We have liftoff!"

boardSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
# need to send any message over to initialize connection to sensor
boardSock.sendto(bytes(MESSAGE, "utf-8"), (UDP_IP, UDP_PORT))
boardSock.setblocking(0) # allows the program to pass the blocking recvfrom() for the board

# Create UDP socket to use for sending and receiving data from Unity game
sock = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001, enableRX=True, suppressWarnings=True)

logging.basicConfig(level=logging.INFO, 
    format='%(asctime)s.%(msecs)03d %(levelname)s:\t%(message)s',
    datefmt='%Y-%m-%d %H:%M:%S')


logging.info("Starting Server")
time.sleep(1)

decodedMessage = [20] #adjust this if more strings and arguments are necessary

while True: 
    # Constantly read message from Unity
    #logging.info("Waiting For Message From Unity")
    decodedMessage = sock.ReadReceivedData() # read data

    # Handles messages that have 2 arguments. Such as "testing 123" -> ['testing2', '123']
    if (decodedMessage == None):
        decodedMessage = [' ']
    else:
        print(decodedMessage)
        

    try:
        decodedMessage = decodedMessage.split(' ')
    except AttributeError:
        #print(AttributeError)
        pass

    #SPLIT THE MESSAGE NDUMMY HEAD

    # For checking for the board sensor in the minecart level
    # then sends board data
    try: 
        boardMsg = boardSock.recvfrom(16)
        value = boardMsg[0].decode('utf-8')
        try:
            newval = float(value.replace('\U00002013', '-'))*360/math.pi*2
            sock.SendData("boardMove " + str(newval))
            #print(newval)
        except ValueError:
            print ("Not a float")
            pass
    except BlockingIOError: # when board sensor is not connected
        #print("blocked!!")
        pass
    

    # decode message from unity

    if(decodedMessage[0] == "quit"):
        logging.info("End of Unity Game reached")
        sock.unityShutDown()               
        break
    elif(decodedMessage[0] == "gameStart"):
        logging.info("Game has started!")
        gameStarted = True #in case python needs to know
        sock.SendData("ACKgameStart")
        if(decodedMessage.__contains__("deadTime")):
            logging.info("receiving deadTime from Unity")
            counter = 0
            for data in decodedMessage:
                counter+=1
                if data == "deadTime":
                    deadTime = decodedMessage[counter]
                    print(deadTime)
                    sock.SendData("ACKdeadTime")

    elif(decodedMessage[0] == "startCalibrating"):
        logging.info("Game is trying to calibrate")
        getCalibration = U.UdpComms.sensorCalibration()
        if (getCalibration): 
            sock.SendData("calibratedRigsuccess")
        else:
            sock.SendData("calibratedRigFailed")

    # elif(decodedMessage.__contains__("deadTime")):
    #     logging.info("receiving deadTime from Unity")
    #     counter = 0
    #     for data in decodedMessage:
    #         counter+=1
    #         if data == "deadTime":
    #             deadTime = decodedMessage[counter]
    #             print(deadTime)
    #             sock.SendData("ACKdeadTime")


#####################################################################
############################## TESTING ##############################
#####################################################################

    elif(decodedMessage[0] == "testing1"):
        print("Testing the communication")
        sock.SendData("testingPython 5555.00")

    # testing multi argument value strings sending back and forth to game
    elif(decodedMessage[0] == "testing2"):
        new_split_message = decodedMessage[1]
        logging.info("Testing the communication with multi: " + new_split_message)
        sock.SendData("longStringVerified")
        decodedMessage = [3] # reset split message or it'll keep sending this message

    # testing long string with many arguments
    # ex: hello 1234 world 5678
    elif(decodedMessage.__contains__("world")):
        logging.info("CONTAINS TEST RUN")
        counter = 0
        for data in decodedMessage:
            counter+=1
            if data == "world":
                print(decodedMessage[counter])
            if data == "hello":
                print(decodedMessage[counter])
            
        
    
          
