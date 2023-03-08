# Created by Youssef Elashry to allow two-way communication between Python3 and Unity to send and receive strings

# Feel free to use this in your individual or commercial projects BUT make sure to reference me as: Two-way communication between Python 3 and Unity (C#) - Y. T. Elashry
# It would be appreciated if you send me how you have used this in your projects (e.g. Machine Learning) at youssef.elashry@gmail.com

# Use at your own risk
# Use under the Apache License 2.0

# Example of a Python UDP server

import UdpComms as U
import time
import logging


# Create UDP socket to use for sending (and receiving)
sock = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001, enableRX=True, suppressWarnings=True)

logging.basicConfig(level=logging.INFO, 
    format='%(asctime)s.%(msecs)03d %(levelname)s:\t%(message)s',
    datefmt='%Y-%m-%d %H:%M:%S')


logging.info("Starting Server")
time.sleep(1)

while True: 
    # Constantly read message from Unity
    logging.info("Waiting For Message From Unity")
    decodedMessage = sock.ReadReceivedData() # read data
    print(decodedMessage)
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
            
