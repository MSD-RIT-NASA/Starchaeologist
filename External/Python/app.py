#
#  app.py
#  Created by: William Johnson
#

from distutils.log import debug
from threading import Thread
from pubsub import pub
import wx
import controller as Controller
from threading import Thread
import logging

class App(Thread):
    def __init__(self, debug):
        Thread.__init__(self)
        self.debug = debug
        
    
    def run(self):
        self.app = wx.App()
        self.controller = Controller.Controller(self.debug)
        logging.info("Starting Main Loop")
        self.app.MainLoop()


if __name__ == '__main__':
    logging.basicConfig(
        format='%(asctime)s %(levelname)-8s %(message)s',
        level=logging.INFO,
        datefmt='%Y-%m-%d %H:%M:%S')
    app = App(debug=True)
    app.start()
    
    
    