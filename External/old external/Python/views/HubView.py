import wx
from pubsub import pub

class HubView(wx.Frame): 
    """
    Constructor for the application hub
    """            
    def __init__(self, parent): 
      super(HubView, self).__init__(parent, title = "Training System")  
      self.SetBackgroundColour('white')
      self.Header = 1
      panel = wx.Panel(self) 
      hbox = wx.BoxSizer(wx.HORIZONTAL)
      fgs = wx.FlexGridSizer(3, 3, 10,10)
      fgs.AddGrowableCol(0)
      fgs.AddGrowableCol(1)
      fgs.AddGrowableCol(2)
      self.statistics = wx.Button(panel, -1,"Statistics")
      fgs.Add(self.statistics, 0, wx.EXPAND)
      self.statistics.Bind(wx.EVT_BUTTON, self.statisticsOpen)

      fgs.AddSpacer(0)
      # self.logOut = wx.Button(panel, -1,"Logout")
      self.logOut = wx.Button(self, -1,"Logout")

      fgs.Add(self.logOut, 0, wx.EXPAND)
      self.logOut.Bind(wx.EVT_BUTTON, self.logoutOpen)
      fgs.AddSpacer(0)
      fgs.AddSpacer(0)
      fgs.AddSpacer(0)
      fgs.AddSpacer(0)
      self.gameButton = wx.Button(panel, -1,"Game Button")
      fgs.Add(self.gameButton, 0, wx.EXPAND)
      self.gameButton.Bind(wx.EVT_BUTTON, self.startGame)

      self.Bind(wx.EVT_CLOSE, self.onClose)
      
      hbox.Add(fgs, proportion = 2, flag = wx.ALL|wx.EXPAND, border = 15) 
      
      panel.SetSizerAndFit(hbox)
      
      self.SetSizerAndFit(hbox)
      self.Centre()
      
      
    def statisticsOpen(self, event):
      """
      Open Statistics View
      """
      pub.sendMessage('statistics.open')
       
    def logoutOpen(self, event):
      """
      Open Logout Dialog
      """
      pub.sendMessage('logout.open')

    def startGame(self, event):
      """
      Start Game
      """
      pub.sendMessage('game.start')

    def onClose(self, event):
      """
      Close Application
      """
      pub.sendMessage('app.end')