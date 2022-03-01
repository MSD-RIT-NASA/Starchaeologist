from distutils.log import debug
import logging
import wx
from views import DefaultView, HubView, LoginView, StatisticsView
from pubsub import pub
import server as Server
from database.dbcalls import db, findUserID, addUser

class Controller:
    def __init__(self, debug):
        self.mainView = DefaultView.DefaultView(None, "Training System")
        self.hubView = HubView.HubView(None)
        self.loginView = LoginView.LoginView(None)
        self.statisticsView = StatisticsView.StatisticsView(None)
        self.db = db()
        self.currentUser = None
        self.debug = debug
        # Pub subscriptions
        pub.subscribe(self.loginOpen, 'login.open')
        pub.subscribe(self.loginAttempt, 'login.attempt')

        pub.subscribe(self.logoutOpen, 'logout.open')

        pub.subscribe(self.statisticsOpen, 'statistics.open')

        pub.subscribe(self.serverStart, 'server.start')

        
        
    
        self.mainView.Show(True)
    
    def loginOpen(self):
        self.loginView.ShowModal()

    def loginAttempt(self, username, password):
        val = findUserID(self.db,username)
        if val is not None:
            if password == val[0][2]:
                self.currentUser = val[0][0]
                self.mainView.Show(False)
                self.loginView.Close()
                # Set statistics functon
                self.setUserStatistics()
                self.hubView.Show(True)
                
            else:
                resp = wx.MessageDialog(None, "Incorrect Password for this Username. Please try again.", "Failed Login" , wx.OK).ShowModal()
                resp.Destroy()
        else:
            val = addUser(self.db, username, password)
            # New User Added Popup?
            self.currentUser = val[0][0]
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
        self.statisticsView.ShowModal()

    def setUserStatistics(self):
        pass

    def serverStart(self):
        server = Server.Server(debug=debug)
        server.start()
        server.join()
        logging.info("Closing Server")    

   
