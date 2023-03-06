#
#  server.py
#  Authors: Angela Hudak & Corey Sheirden

from threading import Thread
from pubsub import pub
from _thread import *;
import logging
# from System.Runtime.InteropServices import Marshal
import zmq
import time
import serial

#from sensors import getscore

# import sys
# sys.path.insert(0, 'C:/Users/angel/Desktop/GPBA/External/BASE_sensors')
# from sensors import getdata, getscore

# Grab sensor data from the arduino
def getdata(ser):

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
            dataEntry.append(float(data))
        # on arduino side, when the game ends then stop getting the score by sending a message to arduino
        decodedMessage = server.unityRead()
        if ( decodedMessage == "Game Over"):
            break

    return balanceData


class Server(Thread):
    """
    Server controls interactions between Python to and from Unity 
    """
    def __init__(self, debug= False):
        Thread.__init__(self)
          
        self.debug = debug
        self.end = False
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind("tcp://127.0.0.1:5678")
        #context.term()
        
        pub.subscribe(self.endThread, 'server.end')


    def unityRead(self): 
        """
        Read data from unity
        """
        print("test1")
        message = self.socket.recv()
        decodedMessage = message.decode("utf-8")
        return decodedMessage
    

    def unityWrite(self, message):
        """
        Send message to unity
        """
        if self.debug:
            logging.debug(message)
        else:
            try:
                encodedMessage = message.encode()
                self.socket.send(encodedMessage)
            except:
                logging.error("Unity Not Properly Setup")

    
    def unityShutDown(self):
        """
        Inform UnityGame that the program is being shutdown
        """
        self.unityWrite("quit")


    def endThread(self):
        """
        End Server Thread
        """
        logging.info("Ending Server Thread")
        self.end = True

    def sensorCalibration():
        # set up the serial line
        ser = serial.Serial('COM10', 9600) # will need to change COM # per device
        time.sleep(2)

        # reading if calibration was complete
        if ser.readline().decode("ISO-8859-1").strip() == "Calibration completed" :
            print("Calibration completed\n")
            # send to start gathering data
            #read unity for "Game start" or GAME MODE
            # when the game mode is not 0 or 3 then start the score collection

            
            val = input("Step on sensor and type 'y' to begin or anykey to quit: ")
            if val == "y":
                ser.write(val.encode()) #arduino code waits for 'y' to start collecting data
                #Server.getScore(ser)
                return 1 #Calibrated!

            else:
                print("CALIBRATION FAILED")
                return 0 
        else:
            print("recalibrating\n")
            #ser.close()
            Server.sensorCalibration()


    # def sendBalanceScore(self, ser):
        
    #     # gather the data
    #     data = getdata(ser)
    #     ser.close()

    #     # calculate score
    #     balance_score = getscore(data)

    #     print("Final Score: " + str(balance_score))
    #     self.unityWrite(balance_score)
    #     return balance_score
    
    def sendPlatformMovement():
        return 0
    
    def getGameMovement():
        return 0
    
    def run(self):
        """
        Behavior for the Python server
        """

        logging.info("Starting Server")
        time.sleep(1)
        while True and not self.end:     
            if self.debug:
                logging.info("Debug Mode: Server closed")
                time.sleep(1)
                break
            # Constantly read message from Unity
            logging.info("Waiting For Message From Unity")
            decodedMessage = self.unityRead()
            logging.info("Message Recieved From Unity: " + decodedMessage)
            print("Message Recieved From Unity: " + decodedMessage)
            if(decodedMessage == "quit"):
                logging.info("End of Unity Game reached")
                self.unityShutDown()               
                break
            elif(decodedMessage == "startCalibrating"):
                print("Game is trying to calibrate")
                # self.unityWrite("HELLO HELLO ARE YOU THERE")
                getCalibration = self.sensorCalibration()
                if (getCalibration): 
                    self.unityWrite("calibratedRigsuccess")
                else:
                    self.unityWrite("calibratedRigFailed")
                
                

            # elif(decodedMessage.startswith("rotation")):
            #     try:
            #         self.unityWrite("THIS IS BALANCE BOARD DATA")
            #     except:
            #         logging.error("Error occured while parsing data")
            #         self.unityWrite("error " + decodedMessage)
    


# Testing for the server's motion control
if __name__ == "__main__":
    server = Server(debug=False) 

    logging.basicConfig(level=logging.INFO, 
    format='%(asctime)s.%(msecs)03d %(levelname)s:\t%(message)s',
    datefmt='%Y-%m-%d %H:%M:%S')

    server.run()
    
