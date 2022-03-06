import wx
from pubsub import pub

import numpy as np

from matplotlib.figure import Figure
from matplotlib.backends.backend_wxagg import FigureCanvasWxAgg as FigureCanvas

class StatisticsView(wx.Frame): 
   def __init__(self, parent): 
      # wx.Frame.__init__(self, parent)
      super(StatisticsView, self).__init__(parent, title = "Statistics") 
      
      self.panel_one = BalancePanel(self)
      self.panel_two = BalancePanel(self)
      self.panel_two.Hide()

      self.sizer = wx.BoxSizer(wx.VERTICAL)
      self.sizer.Add(self.panel_one, 1, wx.EXPAND)
      self.sizer.Add(self.panel_two, 1, wx.EXPAND)
      self.SetSizer(self.sizer)


      menubar = wx.MenuBar()
      fileMenu = wx.Menu()
      switch_panels_menu_item = fileMenu.Append(wx.ID_ANY, 
                                                "Switch Panels", 
                                                "Some text")
      self.Bind(wx.EVT_MENU, self.onSwitchPanels, 
               switch_panels_menu_item)
      menubar.Append(fileMenu, '&File')
      self.SetMenuBar(menubar)

    #----------------------------------------------------------------------
   def onSwitchPanels(self, event):
      """"""
      if self.panel_one.IsShown():
         self.SetTitle("Panel Two Showing")
         self.panel_one.Hide()
         self.panel_two.Show()
      else:
         self.SetTitle("Panel One Showing")
         self.panel_one.Show()
         self.panel_two.Hide()
      self.Layout()




class BalancePanel(wx.Panel):
   def __init__(self, parent):
      """Constructor"""
      wx.Panel.__init__(self, parent=parent)
      vert_sizer = wx.BoxSizer(wx.VERTICAL)
      self.vert_sizer = vert_sizer

      self.figure = Figure()
      self.canvas = FigureCanvas(self, -1, self.figure)
   
      self.axes = self.figure.add_subplot(111)
      # panel.axes2 = panel.figure.add_subplot(212)

      vert_sizer.Add(self.canvas, 1, wx.LEFT | wx.TOP | wx.EXPAND)
      self.SetSizer(vert_sizer)
      self.Fit()

      self.plotScore()

      self.canvas.mpl_connect('pick_event', self.displayBreakdown)
   def displayBreakdown(self, event):
      if self.canvas.HasCapture(): self.canvas.ReleaseMouse()
      wx.MessageBox('x :'+str(event.mouseevent.xdata) + 'y: ' + str(event.mouseevent.ydata), 'Info',wx.OK | wx.ICON_INFORMATION)

   def plotScore(self):
        x = np.arange(10)
        y = np.random.randn(10)
        self.axes.plot(x,y, 'b', picker = 5)
        x = np.arange(15)
        y = np.random.randn(15)
        self.axes.plot(x,y, 'o', picker = 5)
        
if __name__ == '__main__':
   app = wx.App()
   view = StatisticsView(None)
   view.Show()
   app.MainLoop()