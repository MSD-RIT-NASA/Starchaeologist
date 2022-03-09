# from distutils.log import debug
import logging
import wx
from views import DefaultView, HubView, LoginView, StatisticsView
from pubsub import pub
import server as Server
import time
import killswitch as KillSwitch
from database.dbcalls import db

class Controller:
    def __init__(self, debug):
        self.mainView = DefaultView.DefaultView(None, "Training System")
        self.hubView = HubView.HubView(None)
        self.loginView = LoginView.LoginView(None)
        self.statisticsView = StatisticsView.StatisticsView(None)
        self.db = db()
        
        self.currentUser = None
        self.debug = debug
        # Threads
        self.killSwitch = KillSwitch.KillSwitchMonitor()
        self.killSwitch.start()
        time.sleep(1)
        self.server = Server.Server(debug=self.debug)
        self.server.start()
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
                self.setUserStatistics()
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
            self.setUserStatistics()
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
        print(self.currentUser)
        bScore = self.db.getBalanceScore(self.currentUser)
        gScore = self.db.getGameScore(self.currentUser)
        print(bScore)
        print(gScore)
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
        logging.info("Closed all Views")
        pub.sendMessage('killswitch.end')
        self.killSwitch.join()
        pub.sendMessage('server.end')
        self.server.join()
        exit(0)
        # 
        # pub.sendMessage('server.end')
        # pub.sendMessage('app.end')

   
