#
#  killswitch.py
#  Created by: William Johnson
#

from distutils.log import debug
import logging
import serial
import time

from threading import Thread
from pubsub import pub

class KillSwitchMonitor(Thread):
    def __init__(self, debug):
        Thread.__init__(self)
        self.end = False
        self.debug = debug
        if not self.debug:
            self.arduino = serial.Serial(port='COM6', baudrate=115200, timeout=.1)
            
        pub.subscribe(self.endThread, 'killswitch.end')
        # Initialize connection to kill switch
    def run(self):
        logging.info("Starting Kill Switch Thread")
        while not self.end:
            command = self.arduino.readline().decode("utf-8")
            if command == "1":
                pub.sendMessage('killSwitch.check',message="live")
            elif command == "2":
                pub.sendMessage('killSwitch.check',message="kill")
            time.sleep(0.05)

    def endThread(self):
        logging.info("Ending Kill Switch Thread")
        self.end = True
            
if __name__ == '__main__':
    killSwitch = KillSwitchMonitor(False)
    killSwitch.start()