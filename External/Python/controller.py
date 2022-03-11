# from distutils.log import debug
import logging
import wx
from views import DefaultView, HubView, LoginView, StatisticsView
from pubsub import pub
import server as Server
import time
import killswitch as KillSwitch
from database.dbcalls import db
from event_channel.threaded_event_channel import ThreadedEventChannel

class Controller:
    def __init__(self, debug):
        self.db = db()
        
        self.currentUser = None
        self.debug = debug
        # Threads
        self.server = Server.Server(debug=self.debug)
        self.server.start()
        time.sleep(1)
        self.killSwitch = KillSwitch.KillSwitchMonitor()
        self.killSwitch.start()
        
        self.non_blocking_threaded = ThreadedEventChannel(blocking=False)
        self.non_blocking_threaded.subscribe("statistics.userPlot", self.setUserStatistics)

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
        

    def loginOpen(self):
        self.loginView.ShowModal()

    def loginAttempt(self, username, password):
        val = self.db.findUserID(username)
        if val is not None:
            if password == val[0][2]:
                self.currentUser = str(val[0][0])
                self.mainView.Show(False)
                self.loginView.Close()
                # Set statistics functon
                self.non_blocking_threaded.publish("statistics.userPlot")
                
                self.hubView.Show(True)
                
            else:
                resp = wx.MessageDialog(None, "Incorrect Password for this Username. Please try again.", "Failed Login" , wx.OK).ShowModal()
                resp.Destroy()
        else:
            val = self.db.addUser(username, password)
            # New User Added Popup?
            self.currentUser = str(val[0][0])
            self.mainView.Show(False)
            self.loginView.Close()
            
            # Set statistics functon
            self.non_blocking_threaded.publish("statistics.userPlot")
            
            self.hubView.Show(True)

    def logoutOpen(self):
        window = wx.MessageDialog(None, "Are you sure you want to logout?", "Logout" , wx.YES_NO|wx.YES_DEFAULT)
        resp = window.ShowModal()
        if resp == wx.ID_YES:
            self.currentUser = None
            self.hubView.Show(False)
            self.mainView.Show(True)
        window.Destroy()
    

    def statisticsOpen(self):
        self.statisticsView.Show()

    def setUserStatistics(self):
        bScore = self.db.getBalanceScore(self.currentUser)
        gScore = self.db.getGameScore(self.currentUser)
        
        pub.sendMessage("statistics.plot", bScore=bScore, gScore=gScore)
        
    def gameStart(self):
        # [SteamVR Directory]\bin\win64\vrstartup.exe
        # server = Server.Server(debug=debug)
        # server.start()
        # server.join()
        # logging.info("Closing Server")
        pass
    
    def closeApp(self):
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
        exit(0)

   