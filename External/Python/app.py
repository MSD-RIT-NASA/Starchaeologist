#
#  app.py
#  Created by: William Johnson
#

from threading import Thread
import wx
import controller as Controller
import server as Server
from threading import Thread
import time
import logging

class App(Thread):
    def __init__(self):
        Thread.__init__(self)
    
    def run(self):
        self.app = wx.App()
        c = Controller.Controller()
        self.app.MainLoop()

if __name__ == '__main__':
    logging.basicConfig(
        format='%(asctime)s %(levelname)-8s %(message)s',
        level=logging.INFO,
        datefmt='%Y-%m-%d %H:%M:%S')
    app = App()
    app.start()
    time.sleep(1)
    server = Server.Server()
    server.start()
    