import wx
from pubsub import pub

class DefaultView(wx.Frame):  
    """
    Constructor for the view that display, when first opening the program 
    """           
    def __init__(self, parent, title, gameOneScores, gameTwoScores): 
      super(DefaultView, self).__init__(parent, title = title, size= wx.Size(1000,800), style= wx.CAPTION | wx.CLOSE_BOX | wx.BORDER_SIMPLE)  
      self.SetBackgroundColour('white')

      self.Header = 1

      # Create scoreboards for both games
      self.panel_one = ScoreBoard(self, gameOneScores)
      self.panel_two = ScoreBoard(self, gameTwoScores)

      self.panel_two.Hide()
      
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

      vBox.Add(headerBox, 0, wx.EXPAND)
    
      vBox.Add(self.panel_one, 1, wx.EXPAND)
      vBox.Add(self.panel_two, 1, wx.EXPAND)
      
      btn1 = wx.Button(self, label = "Login")
      
      btn1.SetFocus() 
      btn1.Bind(wx.EVT_BUTTON, self.Login)
      vBox.AddSpacer(0)
      
      vBox.Add(btn1, 0, wx.ALIGN_CENTER)

      self.Bind(wx.EVT_CLOSE, self.onClose)
      
      # Timer for switching between score
      self.timer = wx.Timer(self)
      self.Bind(wx.EVT_TIMER, self.UpdateHeader, self.timer)
      
      self.color = wx.Colour(61, 61, 194)
      self.color2 = wx.Colour(255, 255, 255)
      
      self.header1.BackgroundColour = self.color
      self.header2.BackgroundColour = self.color2
      
      self.SetSizerAndFit(vBox)
      self.SetMinSize((350,400))
      self.SetMaxSize((350,400))
      self.Centre()
      self.Layout()

      
    def Login(self, event):
        """
        Login button is clicked 
        """
        self.timer.Stop()   
        pub.sendMessage('login.open')
        self.timer.Start(5000)

    def UpdateHeader(self, event):
      """
      Update the displayed scoreboard every 5 seconds
      """
      if self.Header == 1:
          self.panel_one.Hide()
          self.panel_two.Show()          
          self.header1.BackgroundColour = self.color2
          self.header2.BackgroundColour = self.color
          self.Header = 2

      elif self.Header == 2:
          self.panel_two.Hide()
          self.panel_one.Show()
          self.header2.BackgroundColour = self.color2
          self.header1.BackgroundColour = self.color
          self.Header = 1
      self.Update()
      self.Layout()

    def switchGame(self,event):
      """
      Change the displayed scoreboard based on user 
      """
      gameID = event.GetEventObject().name
      self.timer.Stop()
      if gameID == 1:
        self.panel_two.Hide()
        self.panel_one.Show()

        self.Header = 1
        self.header1.BackgroundColour = self.color
        self.header2.BackgroundColour = self.color2
      elif gameID == 2:
        self.panel_one.Hide()
        self.panel_two.Show()
        
        self.Header = 2
        self.header1.BackgroundColour = self.color2
        self.header2.BackgroundColour = self.color

      self.Layout()
      self.timer.Start(5000)
		
    def onClose(self, event):
      pub.sendMessage('app.end')
      exit(0)


class ScoreBoard(wx.Panel):
   def __init__(self, parent, score):
      """Constructor for ScoreBoard"""
      wx.Panel.__init__(self, parent=parent)
      vert_sizer = wx.BoxSizer(wx.VERTICAL)
      self.vert_sizer = vert_sizer
      color1 = wx.Colour(127, 252, 137)
      # color3 = wx.Colour(61, 194, 128)
      color4 = wx.Colour(180, 251, 186)
      # color2 = wx.Colour(100, 206, 100)
      count = 0
      
      # Display Top 10 Scores with alternating color. Empty scores replaced with '-'
      while count < 10:
        text = None
        if count < len(score):
          text = wx.StaticText(self, label=str(count + 1) + ". " + str(score[count][1]) + " - " + str(score[count][0]), style=wx.ALIGN_CENTER  | wx.ALIGN_CENTER_VERTICAL)
        else:
          text = wx.StaticText(self, label=str(count + 1) + ". -", style=wx.ALIGN_CENTER| wx.ALIGN_CENTER_VERTICAL)
        if count%2 == 0:
          text.SetBackgroundColour(color1)
        else:
          text.SetBackgroundColour(color4)
        
        self.vert_sizer.Add(text, 1, wx.EXPAND | wx.ALL)
        count+=1
      
      self.SetSizerAndFit(vert_sizer)
      self.Layout()

# Tester for score board display
if __name__ == '__main__':
  app = wx.App()
  g1 = [(695, 'young'), (688, 'ALL'), (659, 'Will'), (658, 'Will'), (385, 'Will'), (375, 'admin'), (365, 'test'), (356, 'ALL'), (355, 'Will'), (350, 'TEST')]    
  g2 = [(91, 'young'), (81, 'ALL'), (59, 'Will'), (51, 'Will'), (50, 'test'), (30, 'test'), (15, 'Will'), (5, 'TEST')]
  view = DefaultView(None, "Test", g1, g2)
  view.Show()
  app.MainLoop()