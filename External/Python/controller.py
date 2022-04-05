import logging
import serial.tools.list_ports
import multiprocessing
import threading
import wx
import wx.adv
from views import DefaultView, HubView, LoginView, StatisticsView
from pubsub import pub
import server as Server
import subprocess
import time
import killswitch as KillSwitch
from database.dbcalls import db

class Controller:
    def __init__(self, debug=False):
        self.db = db()
        
        self.currentUser = None
        self.debug = debug

        self.plottingProcesses = []
        self.bPlotted = False
        self.gPlotted = False
        serverPort = "COM6"
        killSwitchPort = "COM3"
        # Threads
        ports = [tuple(p) for p in list(serial.tools.list_ports.comports())]
        
        serverPortStatus = [port for port in ports if serverPort in port ]
        killSwitchPortStatus = [port for port in ports if killSwitchPort in port ]
        if(len(serverPortStatus) > 0):
            if serverPortStatus[0][1].startswith("Arduino"):
                self.server = Server.Server(debug=self.debug, port=serverPort)
                self.server.start()
                time.sleep(1)
            else:
                self.server = None
                logging.error("Server not started: Arduino not connected to port " + serverPort)
        else:
            self.server = None
            logging.error("Server not started: Arduino not connected to port " + serverPort)
        if(len(killSwitchPortStatus) > 0):
            if killSwitchPortStatus[0][1].startswith("Arduino"):
                self.killSwitch = KillSwitch.KillSwitchMonitor(debug=self.debug, port=killSwitchPort)
                self.killSwitch.start()
            else:
                self.killSwitch = None
                logging.error("KillSwitch not started: Arduino not connected to port " + killSwitchPort)
        else:
            self.killSwitch = None
            logging.error("KillSwitch not started: Arduino not connected to port " + killSwitchPort)
        
        gameOneScores, gameTwoScores, gameThreeScores = self.db.getTopScores()
        
        #Views
        self.mainView = DefaultView.DefaultView(None, "Training System", gameOneScores, gameTwoScores, gameThreeScores)
        self.hubView = HubView.HubView(None)
        self.loginView = LoginView.LoginView(None)
        self.statisticsView = StatisticsView.StatisticsView(None)
        
        
        # Pub subscriptions
        pub.subscribe(self.loginOpen, 'login.open')
        pub.subscribe(self.loginAttempt, 'login.attempt')

        pub.subscribe(self.logoutOpen, 'logout.open')

        pub.subscribe(self.statisticsOpen, 'statistics.open')

        pub.subscribe(self.gameStart, 'game.start')

        pub.subscribe(self.closeApp, "app.end")

        pub.subscribe(self.addBalanceScore, "database.BalanceScore")
        pub.subscribe(self.addGameScore, "database.GameScore")


        self.mainView.Show(True)
        self.mainView.timer.Start(5000)


    def loginOpen(self):
        self.loginView.ShowModal()

    def loginAttempt(self, username, password):
        logging.info("Attempting User Login")
        val = self.db.findUserID(username)
        if val is not None:
            if password == val[0][2]:
                logging.info("Successful User Login")
                self.currentUser = str(val[0][0])
                self.mainView.Show(False)
                self.loginView.Close()            
    
                # Set statistics functon works
                self.bPlotted = False
                self.gPlotted = False

                proc1 = multiprocessing.Process(target=self.setUserGameStatistics(), args=())
                self.plottingProcesses.append(proc1)
                proc2 = multiprocessing.Process(target=self.setUserBalanceStatistics(), args=())
                self.plottingProcesses.append(proc2)
                th1 = threading.Thread(target=proc1.start)
                th2 = threading.Thread(target=proc2.start)
                th1.start()
                th2.start()
                
                self.hubView.Show(True)
            else:
                resp = wx.MessageDialog(None, "Incorrect Password for this Username. Please try again.", "Incorrect Login Credentials" , wx.OK)
                resp.ShowModal()
                resp.Destroy()
        else:
            logging.info("New User Logging In")
            val = self.db.addUser(username, password)
            # New User Added Popup?
            resp = wx.MessageDialog(None,"New User Added to Database","Welcome to the Training System " + username + "!", wx.OK)
            resp.ShowModal()
            resp.Destroy()

            self.currentUser = str(val[0][0])
            self.mainView.Show(False)
            self.loginView.Close()
                            
            # Set statistics functon works
            self.bPlotted = False
            self.gPlotted = False

            proc1 = multiprocessing.Process(target=self.setUserGameStatistics(), args=())
            self.plottingProcesses.append(proc1)
            proc2 = multiprocessing.Process(target=self.setUserBalanceStatistics(), args=())
            self.plottingProcesses.append(proc2)
            proc1.start()
            proc2.start()  

            self.hubView.Show(True) 

    def logoutOpen(self):
        window = wx.MessageDialog(None, "Are you sure you want to logout?", "Logout" , wx.YES_NO|wx.YES_DEFAULT)
        resp = window.ShowModal()
        if resp == wx.ID_YES:
            logging.info("User Logging Out")
            self.currentUser = None
            self.hubView.Show(False)
            self.mainView.Show(True)

            self.bPlotted = False
            self.gPlotted = False
            for proc in self.plottingProcesses:
                proc.kill()
            self.plottingProcesses = []
            self.mainView.timer.Start(5000)
        window.Destroy()
    
    def setUserBalanceStatistics(self):
        bScore = self.db.getBalanceScore(self.currentUser)
        pub.sendMessage("statistics.balance", score=bScore)
        self.bPlotted = True

    def setUserGameStatistics(self):
        gScore = self.db.getGameScore(self.currentUser)
        pub.sendMessage("statistics.game", score=gScore)
        self.gPlotted = True

    def statisticsOpen(self):
        """
        Opens Statistics View. Displays loading message 
        """
        logging.info("Opening Statistics View")
        dialog = wx.ProgressDialog("Loading Statistics", "", maximum=100, style=wx.PD_CAN_ABORT | wx.PD_ELAPSED_TIME)
        cancelled = False
        while (self.bPlotted == False or self.gPlotted == False) and cancelled == False:
            wx.Sleep(1)
            if dialog.WasCancelled():
                cancelled = True
            dialog.Pulse()
        dialog.Destroy()

        if not cancelled:
            self.statisticsView.Show()


    def addGameScore(self, score, gameID):
        self.db.addGameScore(self.currentUser,gameID,score)
    
    def addBalanceScore(self, score, gameID, meanCOP, stdCOP, lengthCOP, centroidX, centroidY):
        self.db.addBalanceScore(self.currentUser,gameID,score,meanCOP,stdCOP,lengthCOP,centroidX,centroidY)

    def gameStart(self):
        """
        Opens Unity Game through SteamVR
        """
        logging.info("Starting Unity Game")
        subprocess.call("./Starchaeologist/builds/Starchaeologist.exe")
    
    def closeApp(self):
        """
        Close Entire Application, after user check
        """
        window = wx.MessageDialog(None, "Are you sure you want to quit?", "Close" , wx.YES_NO|wx.YES_DEFAULT)
        resp = window.ShowModal()
        if resp == wx.ID_YES:
            logging.info("Closing Application")
            self.mainView.Show(False)
            self.hubView.Show(False)
            self.loginView.Show(False)
            self.statisticsView.Show(False)
            logging.info("Closed All Views")
            if self.killSwitch != None:
                pub.sendMessage('killswitch.end')
                self.killSwitch.join()
            if self.server != None:
                pub.sendMessage('server.end')
                self.server.join()
            logging.info("Closed All Servers")
            window.Destroy()
            exit(0)
        else:
            window.Destroy()

if __name__ == '__main__':
    logging.basicConfig(
        format='%(asctime)s %(levelname)-8s %(message)s',
        level=logging.INFO,
        datefmt='%Y-%m-%d %H:%M:%S')
    # subprocess.call("./Starchaeologist/builds/Starchaeologist.exe")
    c = Controller()
