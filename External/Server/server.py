#
#  server.py
#  Authors: Angela Hudak & Corey Sheirden

from threading import Thread
from pubsub import pub
from _thread import *;
#from editpyxl import Workbook
import logging
# from System.Runtime.InteropServices import Marshal
import zmq
import time

class Server(Thread):
    """
    Server controls interactions between Python to and from Unity 
    """
    def __init__(self, port, debug= False):
        Thread.__init__(self)
          
        self.debug = debug
        self.end = False
        context = zmq.Context()
        self.socket = context.socket(zmq.REP)
        self.socket.bind("tcp://*:5678")
        # context.term()
        
        pub.subscribe(self.endThread, 'server.end')


    def unityRead(self):
        """
        Read data from unity
        """
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


    def run(self):
        """
        Behavior for the Python server
        """
        #logging.info("Starting Server")
        print("Starting Server")
        time.sleep(1)
        while True and not self.end:     
            if self.debug:
                #logging.info("Debug Mode: Server closed")
                print("Debug Mode: Server Closed")
                time.sleep(1)
                break
            # Constantly read message from Unity
            print("Waiting For Message From Unity")
            #logging.info("Waiting For Message From Unity")
            decodedMessage = self.unityRead()
            logging.info("Message Recieved From Unity: " + decodedMessage)
            if(decodedMessage == "quit"):
                logging.info("End of Unity Game reached")
                self.unityShutDown()               
                break
            elif(decodedMessage.startswith("rotation")):
                try:
                    self.unityWrite("THIS IS BALANCE BOARD DATA")
                except:
                    logging.error("Error occured while parsing data")
                    self.unityWrite("error " + decodedMessage)
    

    def endThread(self):
        """
        End Server Thread
        """
        logging.info("Ending Server Thread")
        self.end = True


# Testing for the server's motion control
if __name__ == "__main__":
    server = Server(debug=False, port="COM4")
    server.run()
    
