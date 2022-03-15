#
#  killswitch.py
#  Created by: William Johnson
#

import logging
import serial
import time

from threading import Thread
from pubsub import pub

class KillSwitchMonitor(Thread):
    def __init__(self):
        Thread.__init__(self)
        self.end = False
        self.arduino = serial.Serial(port='COM6', baudrate=9600, timeout=.1)
            
        pub.subscribe(self.endThread, 'killswitch.end')
        # Initialize connection to kill switch
    def run(self):
        status = True
        logging.info("Starting Kill Switch Thread")
        while not self.end:
            # print(self.arduino.readline())
            # Check if connection is on and constantly send live
            if status:
                pub.sendMessage('killSwitch.check',message="live")
            else:
                pub.sendMessage('killSwitch.check',message="kill")
            # time.sleep(0.05)

    
    def endThread(self):
        logging.info("Ending Kill Switch Thread")
        self.end = True
            