import math
import wx
from wx.lib import plot as wxplot
# import pyqtgraph as pg
from pubsub import pub

import logging
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
from datetime import datetime
# from database.dbcalls import db
import matplotlib
import numpy as np

from matplotlib.figure import Figure
from matplotlib.backends.backend_wxagg import FigureCanvasWxAgg as FigureCanvas
matplotlib.use('WXAgg')

class StatisticsView(wx.Frame): 
   def __init__(self, parent): 
      super(StatisticsView, self).__init__(parent, title = "Statistics") 
      self.panel_one = BalancePanel(self)
      self.panel_two = BalancePanel(self)
      self.panel_two.Hide()
      # self.panel_one.Draw(graphics1)
      # self.panel_two.Draw(graphics2) 
      self.sizer = wx.BoxSizer(wx.VERTICAL)
      self.SetMinSize((400,300))
      self.sizer.Add(self.panel_one, 1, wx.EXPAND | wx.ALL, 10)
      self.sizer.Add(self.panel_two, 1, wx.EXPAND | wx.ALL, 10)
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
      logging.info("Plotting Score Statistics")
      self.panel_one.plotScore(gScore)
      self.panel_two.plotScore(bScore)
      logging.info("Storing Score Statistics")

   def onClose(self, event):
      logging.info("Closing Statistics View")
      self.Show(False)

class BalancePanel(wx.Panel):
   def __init__(self, parent):
      """Constructor"""
      wx.Panel.__init__(self, parent=parent)
      vert_sizer = wx.BoxSizer(wx.VERTICAL)
      self.vert_sizer = vert_sizer

      self.figure = Figure()
      self.canvas = FigureCanvas(self, -1, self.figure)
      
      self.axes = self.figure.add_subplot(111)
      
      
      vert_sizer.Add(self.canvas, 1, wx.LEFT | wx.TOP | wx.EXPAND)
      self.SetSizerAndFit(vert_sizer)
      
      
   
   def displayBreakdown(self, event):
      print("Something")
      thisline = event.artist
      xdata = thisline.get_xdata()
      ydata = thisline.get_ydata()
      xVal = event.mouseevent.xdata
      if  (math.ceil(xVal) - xVal > 0.1) and xVal - math.floor(xVal)  > 0.1:
         return
      ind = round(event.mouseevent.xdata)
      
      wx.MessageBox('x :'+ str(xdata[ind]) + ' y: ' + str(ydata[ind]), 'Info',wx.OK | wx.ICON_INFORMATION)
      
   def plotScore(self, scores):
      xGame1 = []
      yGame1 = []
      xGame2 = []
      yGame2 = []
      xGame3 = []
      yGame3 = []

      for score in scores:
         if score[2] == 1:
            xGame1.append(datetime.fromisoformat(score[1]))
            yGame1.append(score[0])
         elif score[2] == 2:
            xGame2.append(datetime.fromisoformat(score[1]))
            yGame2.append(score[0])
         elif score[2] == 3:
            xGame3.append(datetime.fromisoformat(score[1]))
            yGame3.append(score[0])
         
      
      self.line1, = self.axes.plot(xGame1,yGame1, 'b', ls = ':', picker = True, pickradius =3, marker = 'o')
      self.line1.set_label('Label via method')

      self.line2, = self.axes.plot(xGame2,yGame2, 'g', ls = ':', picker = True, pickradius =3, marker = 'o')
      self.line2.set_label('Label via method')
      
      self.line3, = self.axes.plot(xGame3,yGame3, 'r', ls = ':', picker = True, pickradius =3, marker = 'o')
      self.line3.set_label('Label via method')
      # mdates.
      self.axes.xaxis.set_major_locator(mdates.DayLocator(bymonthday=range(1, 32)))
      self.axes.xaxis.set_minor_locator(mdates.DayLocator())
      self.axes.grid(True)
      self.axes.legend()

      self.canvas.draw()
      self.canvas.mpl_connect('pick_event', self.displayBreakdown)
      

      # self.annot = self.axes.annotate("", xy=(0,0), xytext=(20,20),textcoords="offset points",
      #               bbox=dict(boxstyle="round", fc="w"),
      #               arrowprops=dict(arrowstyle="->"))
        
if __name__ == '__main__':
   app = wx.App()
   view = StatisticsView(None, True)
   view.Show()
   app.MainLoop()
   
   # app=wx.PySimpleApp()
   # frame=TestFrame(None,"Hello Matplotlib !")
   # frame.Show()
   # app.MainLoop()