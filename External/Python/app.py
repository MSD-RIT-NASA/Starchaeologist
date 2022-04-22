#
#  app.py
#  Created by: William Johnson
#

from threading import Thread
import wx
import controller as Controller
from threading import Thread
import logging

class App(Thread):
    def __init__(self, debug = False):
        Thread.__init__(self)
        self.debug = debug
        
    def run(self):
        self.app = wx.App()
        self.controller = Controller.Controller(self.debug)
        logging.info("Starting Main Application")
        self.app.MainLoop()

if __name__ == '__main__':
    logging.basicConfig(level=logging.INFO, 
        format='%(asctime)s.%(msecs)03d %(levelname)s:\t%(message)s',
        datefmt='%Y-%m-%d %H:%M:%S')
    app = App(debug=True)
    app.start()
    
    
    