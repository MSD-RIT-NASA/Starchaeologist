# Created by Youssef Elashry to allow two-way communication between Python3 and Unity to send and receive strings

# Feel free to use this in your individual or commercial projects BUT make sure to reference me as: Two-way communication between Python 3 and Unity (C#) - Y. T. Elashry
# It would be appreciated if you send me how you have used this in your projects (e.g. Machine Learning) at youssef.elashry@gmail.com

# Use at your own risk
# Use under the Apache License 2.0

# Example of a Python UDP server

# NASA x RIT author: Angela Hudak

import UdpComms as U
import sensors as s
import time
import logging
import socket
import math
import os
from threading import Thread, Event
import matlab_data, planet_data_collection
import serial
from queue import Queue
import actuator_control

# PLANET CONSTANTS
UDP_IP = "192.168.4.2"
UDP_PORT = 4210
MESSAGE = "We have liftoff!"

def sensorCalibration():
    # set up the serial line
    print("Attempting to calibrate sensors")
    try:
        global ser
        ser = serial.Serial(com_port, 9600) # will need to change COM # per device
    except Exception:
        return 0 # game will stop the game and retry to calibrate the sensors

    time.sleep(2)
    # reading if calibration was complete
    if ser.readline().decode("ISO-8859-1").strip() == "Calibration completed" :
        print("Calibration completed\n")
        # send to start gathering data
        # read unity for "Game start" or GAME MODE
        # when the game mode is not 0 or 3 then start the score collection

        val = input("Step on sensor and type 'y' to begin or anykey to quit: ")
        if val == "y":
            ser.write(val.encode()) #arduino code waits for 'y' to start collecting data
            return 1 #Calibrated!
        else:
            print("CALIBRATION FAILED")
            return 0 
    else:
        print("recalibrating\n")
        ser.close()
        sensorCalibration()

# Grab sensor data from the arduino
def getdata(sock):

    balanceData = []
    dataEntry = []

    # TODO: Change this loop to continue for however long the game lasts. 
    
    #for i in range(1000):
    while True:
        data = ser.readline().decode("ISO-8859-1").strip()       # read a byte string
        if data == "END":
            balanceData.append(dataEntry)
            dataEntry = []
        elif data == '' or data == "Calibration completed":
            continue
        else:
            print(data)
            dataEntry.append(float(data))
        # on arduino side, when the game ends then stop getting the score by sending a message to arduino
        decodedMessage = sock.ReadReceivedData()  # read data

        # Handles messages that have 2 arguments. Such as "testing 123" -> ['testing2', '123']
        if (decodedMessage == None):
            decodedMessage = [' ']
        else:
            print(decodedMessage)

        try:
            decodedMessage = decodedMessage.split(' ')
        except AttributeError:
            pass

        try: 
            if (decodedMessage[0] == "gameOver"):
                stop.set()
                baseScore = s.getscore(balanceData)
                print(baseScore)
                sock.SendData("baseScore " + str(int(baseScore)))
                break
        except TypeError:
            pass

    return balanceData

# PLANET events
collect = Event()
log_data = Event()
# Actuator events
riverRun = Event()
puzzlingTimes = Event()
active = Event()
stop = Event()
queue = Queue()

def run(taskQueue: Queue, responseQueue: Queue):

    global com_port
    global game_diff

    logging.getLogger("pycomm3").setLevel(logging.ERROR)
    logging.Formatter(fmt='%(asctime)s',datefmt='%Y-%m-%d,%H:%M:%S.%f')

    script_path = os.path.abspath(__file__)
    root_path = os.path.dirname(script_path)
    csv_root = root_path+"\\Planet Skeleton Data"

    boardSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    # need to send any message over to initialize connection to sensor
    boardSock.sendto(bytes(MESSAGE, "utf-8"), (UDP_IP, UDP_PORT))
    boardSock.setblocking(0)  # allows the program to pass the blocking recvfrom() for the board

    # Create UDP socket to use for sending and receiving data from Unity game
    sock = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001, enableRX=True, suppressWarnings=True)

    logging.basicConfig(level=logging.INFO,
                        format='%(asctime)s.%(msecs)03d %(levelname)s:\t%(message)s',
                        datefmt='%Y-%m-%d %H:%M:%S')

    logging.info("Starting Server")
    time.sleep(1)

    decodedMessage = [20]  # adjust this if more strings and arguments are necessary
    collect_data = 0
    timestamp = 0
    deadTime = 0

    # Start PLANET data collection
    #planet_data = Thread(target=planet_data_collection.run, args=(collect, log_data))
    #planet_data.start()

    # Actuator control
    #actuator_thread = Thread(target=actuator_control.run, args=(riverRun, puzzlingTimes, active, stop))
    #actuator_thread.start()

    while True:

        ############################################################
        #                GUI COMMUNICATION LOOP                    #
        ############################################################
        if not taskQueue.empty():
            message = taskQueue.get()

            if message[0] == 'updateCOM':
                com_port = message[1]
                logging.info("COM Port updated to " + com_port)

            elif message[0] == 'updateDiff':
                game_diff = message[1]
                logging.info("Difficulty updated to " + str(game_diff))

            elif message[0] == 'calibrateSensors':
                logging.info("Game is trying to calibrate")
                getCalibration = sensorCalibration()
                if (getCalibration):
                    sock.SendData("calibratedRigsuccess")
                else:
                    sock.SendData("calibratedRigFailed")
            
            elif message[0] == "stopServer":
                break

        ############################################################
        #                UNITY COMMUNICATION LOOP                  #
        ############################################################

        # Constantly read message from Unity
        # logging.info("Waiting For Message From Unity")
        decodedMessage = sock.ReadReceivedData()  # read data

        # Handles messages that have 2 arguments. Such as "testing 123" -> ['testing2', '123']
        if (decodedMessage == None):
            decodedMessage = [' ']
        else:
            logging.info("Message Recieved: " + decodedMessage.strip('\n'))

        try:
            decodedMessage = decodedMessage.split(' ')
        except AttributeError:
            pass

        # For checking for the board sensor in the minecart level
        # then sends board data
        try:
            boardMsg = boardSock.recvfrom(16)
            value = boardMsg[0].decode('utf-8')
            try:
                newval = -1.0*(float(value.replace('\U00002013', '-')) * 360 / math.pi * 2)
                sock.SendData("boardMove " + str(newval))
                # print(newval)
            except ValueError:
                print("Not a float")
                pass
        except BlockingIOError:  # when board sensor is not connected
            # print("blocked!!")
            pass

        # decode message from unity

        if (decodedMessage[0] == "quit"):
            logging.info("End of Unity Game reached")
            sock.unityShutDown()
            break

        elif (decodedMessage[0] == "gameStart"):
            logging.info("Game has started!")
            sock.SendData("ACKgameStart")
            if (decodedMessage.__contains__("deadTime")):
                logging.info("Receiving deadTime from Unity")
                counter = 0
                for data in decodedMessage:
                    counter += 1
                    if data == "deadTime":
                        deadTime = decodedMessage[counter]
                        print(deadTime)
                        sock.SendData("ACKdeadTime")
                collect.set()
            if (decodedMessage.__contains__("collectBaseData")):
                logging.info("Started actuator subroutines")

                #if level == riverRun:
                riverRun.set()
                #elif level == puzzlingTimes:
                #puzzlingTimes.set()
                stop.clear()
                active.set()

                logging.info("Started to collect data")
                sensordata = getdata(sock)

        elif (decodedMessage[0] == "gameOver"):
            logging.info("Game has ended!")
            gameOver = True
            end_time = time.time()
            timestamp = str(end_time).split('.')[0]
            stop.set()
            log_data.set()
            sock.SendData("ACKgameOver")
            if (decodedMessage.__contains__("getPlanetScore")):
                planetScore = matlab_data.run(csv_root + "/" + timestamp, "Astronaut", 5.0, float(deadTime), 1.5, 3.0, 7.5, 15.0)
                print(planetScore)
                sock.SendData("planetScore " + str(int(planetScore)))
            elif (decodedMessage.__contains__("getBaseScore")):
                # TODO: implement sending balance score from BASE
                baseScore = s.getscore(sensordata)
                sock.SendData("baseScore " + str(int(baseScore)))
                pass

        #####################################################################
        ############################## TESTING ##############################
        #####################################################################

        elif (decodedMessage[0] == "testing1"):
            print("Testing the communication")
            sock.SendData("testingPython 5555.00")

        # testing multi argument value strings sending back and forth to game
        elif (decodedMessage[0] == "testing2"):
            new_split_message = decodedMessage[1]
            logging.info("Testing the communication with multi: " + new_split_message)
            sock.SendData("longStringVerified")
            decodedMessage = [3]  # reset split message or it'll keep sending this message

        # testing long string with many arguments
        # ex: hello 1234 world 5678
        elif (decodedMessage.__contains__("world")):
            logging.info("CONTAINS TEST RUN")
            counter = 0
            for data in decodedMessage:
                counter += 1
                if data == "world":
                    print(decodedMessage[counter])
                if data == "hello":
                    print(decodedMessage[counter])

    logging.info("Stopping Server")