import wx
from pubsub import pub


class DefaultView(wx.Frame):             
    def __init__(self, parent, title): 
      super(DefaultView, self).__init__(parent, title = title)  
      self.SetBackgroundColour('white')
      self.Header = 1
      panel = wx.Panel(self) 
      hbox = wx.BoxSizer(wx.HORIZONTAL)
      fgs = wx.FlexGridSizer(12, 3, 10,10)
      fgs.AddGrowableCol(0)
      fgs.AddGrowableCol(1)
      fgs.AddGrowableCol(2)

      self.header1 = wx.Button(panel, -1,"Game 1")
      fgs.Add(self.header1, 0, wx.EXPAND)
      self.header1.name = 1
      self.header1.Bind(wx.EVT_BUTTON, self.switchGame)
      

      self.header2 = wx.Button(panel, -1,label="Game 2")
      fgs.Add(self.header2, 0, wx.EXPAND)
      self.header2.name = 2
      self.header2.Bind(wx.EVT_BUTTON, self.switchGame)

      self.header3 = wx.Button(panel, -1,label="Game 3")
      fgs.Add(self.header3, 0, wx.EXPAND)
      self.header3.name = 3
      self.header3.Bind(wx.EVT_BUTTON, self.switchGame)

      for i in range(1 , 11):
        fgs.AddSpacer(0)
        text2 = wx.StaticText(panel, label="Player " + str(i), style=wx.ALIGN_CENTER)
        fgs.Add(text2, 0, wx.EXPAND | wx.CENTER)
        fgs.AddSpacer(0)

      btn1 = wx.Button(panel, label = "Login")
      btn1.SetFocus() 
      btn1.Bind(wx.EVT_BUTTON, self.Login)
      fgs.AddSpacer(0)
      fgs.Add(btn1, 0, wx.ALIGN_CENTER)


      self.timer = wx.Timer(self)
      self.Bind(wx.EVT_TIMER, self.UpdateHeader, self.timer)
      # self.toggleBtn = wx.Button(panel, wx.ID_ANY, "Start")
      # self.toggleBtn.Bind(wx.EVT_BUTTON, self.onToggle)
      self.timer.Start(5000)
      hbox.Add(fgs, proportion = 2, flag = wx.ALL|wx.EXPAND, border = 15) 

      # panel.SetSizer(hbox) 
      panel.SetSizerAndFit(hbox)
      self.header1.BackgroundColour = "blue"
      self.Centre()
      self.SetSizerAndFit(hbox)
      # sizer.Fit(self)
      # self.Show(True) 
      # self.SetSizer(hbox)
    
    def Login(self, event):
        self.timer.Stop()   
        pub.sendMessage('login.open')
        self.timer.Start(5000)

            
    def UpdateHeader(self, event):
      # print(self.Header)
      if self.Header == 1:
          self.header1.BackgroundColour = "white"
          self.header2.BackgroundColour = "blue"
          self.Header = 2

      elif self.Header == 2:
          self.header2.BackgroundColour = "white"
          self.header3.BackgroundColour = "blue"
          self.Header = 3
      elif self.Header == 3:
          self.header3.BackgroundColour = "white"
          self.header1.BackgroundColour = "blue"
          self.Header = 1

    def switchGame(self,event):
      gameID = event.GetEventObject().name
      self.timer.Stop()
      if gameID == 1:
        self.Header = 1
        self.header1.BackgroundColour = "blue"
        self.header2.BackgroundColour = "white"
        self.header3.BackgroundColour = "white"
      elif gameID == 2:
        self.Header = 2
        self.header1.BackgroundColour = "white"
        self.header2.BackgroundColour = "blue"
        self.header3.BackgroundColour = "white"
      elif gameID == 3:
        self.Header = 3
        self.header1.BackgroundColour = "white"
        self.header2.BackgroundColour = "white"
        self.header3.BackgroundColour = "blue"
      self.timer.Start(5000)
		