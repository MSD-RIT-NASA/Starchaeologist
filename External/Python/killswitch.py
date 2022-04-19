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
    def __init__(self, port, debug=False):
        Thread.__init__(self)
        self.end = False
        self.debug = debug
        
        self.arduino = serial.Serial(port=port, baudrate=115200, timeout=.1)
        
        pub.subscribe(self.endThread, 'killswitch.end')

    def run(self):
        logging.info("Starting Kill Switch Thread")
        while not self.end:
            command = self.arduino.readline().decode("utf-8")
            if command == "1":
                # print("live")
                pub.sendMessage('killSwitch.check',message="live")
            elif command == "2":
                # print("kill")
                pub.sendMessage('killSwitch.check',message="kill")
            time.sleep(0.05)

    def endThread(self):
        logging.info("Ending Kill Switch Thread")
        self.end = True
            
if __name__ == '__main__':
    killSwitch = KillSwitchMonitor(False)
    killSwitch.start()