#
#  app.py
#  Created by: William Johnson
#

from distutils.log import debug
from threading import Thread
import wx
import controller as Controller
# import server as Server
import killswitch as KillSwitch
from threading import Thread
import time
import logging

class App(Thread):
    def __init__(self, debug):
        Thread.__init__(self)
        self.debug = debug
    
    def run(self):
        self.app = wx.App()
        c = Controller.Controller(self.debug)
        self.app.MainLoop()

if __name__ == '__main__':
    logging.basicConfig(
        format='%(asctime)s %(levelname)-8s %(message)s',
        level=logging.INFO,
        datefmt='%Y-%m-%d %H:%M:%S')
    app = App(debug=True)
    app.start()
    time.sleep(1)
    killSwitchMonitor = KillSwitch.KillSwitchMonitor()
    killSwitchMonitor.start()
    