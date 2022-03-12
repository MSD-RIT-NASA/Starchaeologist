import wx
from pubsub import pub


class DefaultView(wx.Frame):             
    def __init__(self, parent, title, gameOneScores, gameTwoScores, gameThreeScores): 
      super(DefaultView, self).__init__(parent, title = title, size= wx.Size(1000,800), style= wx.SYSTEM_MENU | wx.CAPTION | wx.CLOSE_BOX)  
      self.SetBackgroundColour('white')
      self.Header = 1

      self.panel_one = ScoreBoard(self, gameOneScores)
      self.panel_two = ScoreBoard(self, gameTwoScores)
      self.panel_three = ScoreBoard(self, gameThreeScores)
      self.panel_two.Hide()
      
      self.panel_three.Hide()

      vBox = wx.BoxSizer(wx.VERTICAL)
      headerBox = wx.BoxSizer(wx.HORIZONTAL)

      self.header1 = wx.Button(self, -1,"Game 1")

      headerBox.Add(self.header1, wx.EXPAND)
      self.header1.name = 1
      self.header1.Bind(wx.EVT_BUTTON, self.switchGame)
      

      self.header2 = wx.Button(self, -1,label="Game 2")

      headerBox.Add(self.header2, wx.EXPAND)
      self.header2.name = 2
      self.header2.Bind(wx.EVT_BUTTON, self.switchGame)

      self.header3 = wx.Button(self, -1,label="Game 3")

      headerBox.Add(self.header3, wx.EXPAND)
      self.header3.name = 3
      self.header3.Bind(wx.EVT_BUTTON, self.switchGame)

      vBox.Add(headerBox, 0, wx.EXPAND)
    
      vBox.Add(self.panel_one, 1, wx.EXPAND)
      vBox.Add(self.panel_two, 1, wx.EXPAND)
      vBox.Add(self.panel_three, 1, wx.EXPAND)
      
      btn1 = wx.Button(self, label = "Login")
      btn1.SetFocus() 
      btn1.Bind(wx.EVT_BUTTON, self.Login)
      vBox.AddSpacer(0)
      
      vBox.Add(btn1, 0, wx.ALIGN_CENTER)

      self.Bind(wx.EVT_CLOSE, self.onClose)
      
      self.timer = wx.Timer(self)
      self.Bind(wx.EVT_TIMER, self.UpdateHeader, self.timer)
      
      self.timer.Start(5000)
      
      self.header1.BackgroundColour = "blue"
      
      self.SetSizerAndFit(vBox)
      self.SetMinSize((350,400))
      self.Centre()

      
    def Login(self, event):
        self.timer.Stop()   
        pub.sendMessage('login.open')
        self.timer.Start(5000)

    def UpdateHeader(self, event):
      if self.Header == 1:
          self.panel_one.Hide()
          self.panel_two.Show()          
          self.header1.BackgroundColour = "white"
          self.header2.BackgroundColour = "blue"
          self.Header = 2

      elif self.Header == 2:
          self.panel_two.Hide()
          self.panel_three.Show()
          self.header2.BackgroundColour = "white"
          self.header3.BackgroundColour = "blue"
          self.Header = 3
      elif self.Header == 3:
          self.panel_three.Hide()
          self.panel_one.Show()
          self.header3.BackgroundColour = "white"
          self.header1.BackgroundColour = "blue"
          self.Header = 1
      self.Update()
      self.Layout()

    def switchGame(self,event):
      gameID = event.GetEventObject().name
      self.timer.Stop()
      if gameID == 1:
        self.panel_three.Hide()
        self.panel_two.Hide()
        self.panel_one.Show()

        self.Header = 1
        self.header1.BackgroundColour = "blue"
        self.header2.BackgroundColour = "white"
        self.header3.BackgroundColour = "white"
      elif gameID == 2:
        self.panel_three.Hide()
        self.panel_one.Hide()
        self.panel_two.Show()
        
        self.Header = 2
        self.header1.BackgroundColour = "white"
        self.header2.BackgroundColour = "blue"
        self.header3.BackgroundColour = "white"
      elif gameID == 3:
        self.panel_one.Hide()
        self.panel_two.Hide()
        self.panel_three.Show()

        self.Header = 3
        self.header1.BackgroundColour = "white"
        self.header2.BackgroundColour = "white"
        self.header3.BackgroundColour = "blue"
      self.Layout()
      self.timer.Start(5000)
		
    def onClose(self, event):
      pub.sendMessage('app.end')


class ScoreBoard(wx.Panel):
   def __init__(self, parent, score):
      """Constructor"""
      wx.Panel.__init__(self, parent=parent)
      vert_sizer = wx.BoxSizer(wx.VERTICAL)
      self.vert_sizer = vert_sizer
      count = 0
      while count < 10:        
        if count < len(score):
          text = wx.StaticText(self, label=str(count + 1) + ". " + str(score[count][1]) + " - " + str(score[count][0]), style=wx.ALIGN_CENTER)
          self.vert_sizer.Add(text, 1, wx.EXPAND | wx.CENTER)
        else:
          text = wx.StaticText(self, label=str(count + 1) + ". -", style=wx.ALIGN_CENTER)
          self.vert_sizer.Add(text, 1, wx.EXPAND | wx.CENTER)
        count+=1
      self.SetSizerAndFit(vert_sizer)
      self.Layout()

    
if __name__ == '__main__':
   app = wx.App()
   view = DefaultView(None, "Test")
   view.Show()
   app.MainLoop()