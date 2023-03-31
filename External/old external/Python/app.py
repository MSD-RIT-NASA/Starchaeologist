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
    """
    Main application thread
    """
    def __init__(self, debug = False):
        Thread.__init__(self)
        self.debug = debug
        
    def run(self):
        """
        Thread behavior
        """
        self.app = wx.App()
        self.controller = Controller.Controller(self.debug)
        logging.info("Starting Main Application")
        self.app.MainLoop()

"""
Starts main application
"""
if __name__ == '__main__':
    logging.basicConfig(level=logging.INFO, 
        format='%(asctime)s.%(msecs)03d %(levelname)s:\t%(message)s',
        datefmt='%Y-%m-%d %H:%M:%S')
    app = App(debug=False)
    app.start()
    
    
    