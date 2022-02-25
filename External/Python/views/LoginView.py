import wx
from pubsub import pub

class LoginView(wx.Dialog): 
   def __init__(self, parent): 
      super(LoginView, self).__init__(parent, title = "Login") 
      panel = wx.Panel(self) 
      user_sizer = wx.BoxSizer(wx.HORIZONTAL)
        
      user_lbl = wx.StaticText(self, label="Username:")
      user_sizer.Add(user_lbl, 0, wx.ALL|wx.CENTER, 5)
      self.user = wx.TextCtrl(self)
      user_sizer.Add(self.user, 0, wx.ALL, 5)
      
      # pass info
      p_sizer = wx.BoxSizer(wx.HORIZONTAL)
      
      p_lbl = wx.StaticText(self, label="Password:")
      p_sizer.Add(p_lbl, 0, wx.ALL|wx.CENTER, 5)
      self.password = wx.TextCtrl(self, style=wx.TE_PASSWORD|wx.TE_PROCESS_ENTER)
      p_sizer.Add(self.password, 0, wx.ALL, 5)
      
      main_sizer = wx.BoxSizer(wx.VERTICAL)
      main_sizer.Add(user_sizer, 0, wx.ALL, 5)
      main_sizer.Add(p_sizer, 0, wx.ALL, 5)
      
      btn = wx.Button(self, label="Login")
      btn.Bind(wx.EVT_BUTTON, self.onLogin)
      main_sizer.Add(btn, 0, wx.ALL|wx.CENTER, 5)
      panel.SetSizer(main_sizer) 
      self.SetSizerAndFit(main_sizer)
      self.Centre()
      # self.btn = wx.Button(panel, wx.ID_OK, label = "ok", size = (50,20), pos = (75,50))
   def onLogin(self, event):
        """
        Check credentials and login
        """
        user_username = self.user.GetValue()
        user_password = self.password.GetValue()
        
        pub.sendMessage('login.attempt', username=user_username, password=user_password)


