import math
from matplotlib.pyplot import flag
import wx
from pubsub import pub
import logging
import numpy as np

from matplotlib.figure import Figure
from matplotlib.backends.backend_wxagg import FigureCanvasWxAgg as FigureCanvas

class StatisticsView(wx.Frame): 
   def __init__(self, parent): 
      # wx.Frame.__init__(self, parent)
      # super(HubView, self).__init__(parent, title = "Training System")
      super(StatisticsView, self).__init__(parent, title = "Statistics") 
      
      self.panel_one = BalancePanel(self, 0, 0)
      self.panel_two = BalancePanel(self, 1, 1)
      self.panel_two.Hide()

      self.sizer = wx.BoxSizer(wx.VERTICAL)
      self.sizer.Add(self.panel_one, 1, wx.EXPAND)
      self.sizer.Add(self.panel_two, 1, wx.EXPAND)
      # hsizer = wx.BoxSizer(wx.HORIZONTAL)
      gridSizer = wx.GridSizer(1, 2,5,5)
      # gridSizer.AddMany([
      #    (wx.RadioButton(self,label="Balance Score"), 0, wx.EXPAND),
      #    (wx.RadioButton(self,label="Game Score"), 0, wx.EXPAND)
      # ])
      rbutton = wx.RadioButton(self,label="Balance Score")
      rbutton.SetFocus()
      # hsizer.Add(rbutton, 1, wx.ALIGN_CENTER|wx.ALL, 25)
      rbutton1 = wx.RadioButton(self,label="Game Score")
      # hsizer.AddStretchSpacer(2)
      gridSizer.Add(rbutton, 0, wx.EXPAND)
      gridSizer.Add(rbutton1, 0, wx.EXPAND)
      self.sizer.Add(gridSizer, 0, wx.ALL|wx.EXPAND, 5)
      self.Bind(wx.EVT_CLOSE, self.onClose)
      self.Bind(wx.EVT_RADIOBUTTON, self.onSwitchPanels)
      self.SetSizerAndFit(self.sizer)
      pub.subscribe(self.plotScores, "statistics.plot")



    #----------------------------------------------------------------------
   def onSwitchPanels(self, event):
      """"""
      radioBox = event.GetEventObject() 
      if radioBox.GetLabel() == "Game Score":
         self.SetTitle("Panel Two Showing")
         self.panel_one.Hide()
         self.panel_two.Show()
      else:
         self.SetTitle("Panel One Showing")
         self.panel_one.Show()
         self.panel_two.Hide()
      self.Layout()

   def plotScores(self, bScore, gScore):
      # print(bScore)
      # print(gScore)
      logging.info("START")
      # self.panel_one.plotScore(gScore)
      # self.panel_two.plotScore(bScore)
      logging.info("END")
      pass

   def onClose(self, event):
      self.Show(False)

class BalancePanel(wx.Panel):
   def __init__(self, parent, xCords, yCords):
      """Constructor"""
      wx.Panel.__init__(self, parent=parent)
      vert_sizer = wx.BoxSizer(wx.VERTICAL)
      self.vert_sizer = vert_sizer

      self.figure = Figure()
      self.canvas = FigureCanvas(self, -1, self.figure)
   
      self.axes = self.figure.add_subplot(111)
      self.plotScore(None)
      # panel.axes2 = panel.figure.add_subplot(212)

      vert_sizer.Add(self.canvas, 1, wx.LEFT | wx.TOP | wx.EXPAND)
      self.SetSizerAndFit(vert_sizer)
      # self.canvas.mpl_connect('button_release_event', self.displayBreakdown)
      self.canvas.mpl_connect('pick_event', self.displayBreakdown)
      # datacursor(lines)
      self.canvas.draw()
   
   def displayBreakdown(self, event):
      thisline = event.artist
      xdata = thisline.get_xdata()
      ydata = thisline.get_ydata()
      xVal = event.mouseevent.xdata
      if  (math.ceil(xVal) - xVal > 0.1) and xVal - math.floor(xVal)  > 0.1:
         return
      ind = round(event.mouseevent.xdata)
      wx.MessageBox('x :'+str(xdata[ind]) + ' y: ' + str(ydata[ind]), 'Info',wx.OK | wx.ICON_INFORMATION)

   def plotScore(self, score):
         x = np.arange(10)
         y = np.random.randn(10)
         self.axes.plot(x,y, 'b', ls = ':', picker = True, pickradius =3, marker = 'o')
         #  x = np.arange(15)
      #   y = np.random.randn(15)
      #   self.axes.plot(x,y, 'o', picker = 5)
        
if __name__ == '__main__':
   app = wx.App()
   view = StatisticsView(None)
   view.Show()
   app.MainLoop()
   
   # app=wx.PySimpleApp()
   # frame=TestFrame(None,"Hello Matplotlib !")
   # frame.Show()
   # app.MainLoop()