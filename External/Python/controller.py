import logging
import multiprocessing
import wx
import wx.adv
from views import DefaultView, HubView, LoginView, StatisticsView
from pubsub import pub
import server as Server
from threading import Thread
import time
import killswitch as KillSwitch
from database.dbcalls import db

class Controller:
    def __init__(self, debug):
        self.db = db()
        
        self.currentUser = None
        self.debug = debug

        self.plottingProcesses = []
        self.bPlotted = False
        self.gPlotted = False

        # Threads
        self.server = Server.Server(debug=self.debug)
        self.server.start()
        time.sleep(1)
        self.killSwitch = KillSwitch.KillSwitchMonitor()
        self.killSwitch.start()
        
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
        
        self.mainView.Show(True)
        self.mainView.timer.Start(5000)


    def loginOpen(self):
        self.loginView.ShowModal()

    def loginAttempt(self, username, password):
        val = self.db.findUserID(username)
        if val is not None:
            if password == val[0][2]:
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
            else:
                resp = wx.MessageDialog(None, "Incorrect Password for this Username. Please try again.", "Failed Login" , wx.OK)
                resp.ShowModal()
                resp.Destroy()
        else:
            val = self.db.addUser(username, password)
            # New User Added Popup?
            resp = wx.MessageDialog(None,"New User Added to Database","Welcome to the Training System " + username + "!", wx.OK)
            resp.ShowModal()
            resp.Destroy()

            self.currentUser = str(val[0][0])
            self.mainView.Show(False)
            self.loginView.Close()
            # self.hubView.Show(True)
                            
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
        logging.info("Opening Statistics")
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

    def gameStart(self):
        # self.db.addScore(5,1,1,10)
        # self.db.addScore(5,1,2,60)
        # self.db.addScore(5,1,3,30)
        # self.db.addScore(5,2,1,70)
        # self.db.addScore(5,2,2,50)
        # self.db.addScore(5,2,3,30)
        # [SteamVR Directory]\bin\win64\vrstartup.exe
        # server = Server.Server(debug=debug)
        # server.start()
        # server.join()
        # logging.info("Closing Server")
        pass
    
    def closeApp(self):
        window = wx.MessageDialog(None, "Are you sure you want to quit?", "Close" , wx.YES_NO|wx.YES_DEFAULT)
        resp = window.ShowModal()
        if resp == wx.ID_YES:
            logging.info("Closing Application")
            self.mainView.Show(False)
            self.hubView.Show(False)
            self.loginView.Show(False)
            self.statisticsView.Show(False)
            logging.info("Closed All Views")
            pub.sendMessage('killswitch.end')
            self.killSwitch.join()
            pub.sendMessage('server.end')
            self.server.join()
            logging.info("Closed All Servers")
            window.Destroy()
            exit(0)
        else:
            window.Destroy()

