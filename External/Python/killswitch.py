#
#  killswitch.py
#  Created by: William Johnson
#

from re import T
from threading import Thread
from pubsub import pub

class KillSwitchMonitor(Thread):
    def __init__(self):
        Thread.__init__(self)
        # Initialize connection to kill switch
    def run(self):
        status = True
        while True:
            # Check if connection is on and constantly send live
            if status:
                pub.sendMessage('killSwitch.check',message="live")
            else:
                pub.sendMessage('killSwitch.check',message="kill")