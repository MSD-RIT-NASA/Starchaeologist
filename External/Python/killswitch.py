#
#  killswitch.py
#  Created by: William Johnson
#

import logging
from threading import Thread
from pubsub import pub

class KillSwitchMonitor(Thread):
    def __init__(self):
        Thread.__init__(self)
        self.end = False
        pub.subscribe(self.endThread, 'killswitch.end')
        # Initialize connection to kill switch
    def run(self):
        status = True
        logging.info("Starting Kill Switch Thread")
        while True and not self.end:
            # Check if connection is on and constantly send live
            if status:
                pub.sendMessage('killSwitch.check',message="live")
            else:
                pub.sendMessage('killSwitch.check',message="kill")
    
    def endThread(self):
        logging.info("Ending Kill Switch Thread")
        self.end = True
            