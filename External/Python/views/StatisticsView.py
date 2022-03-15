import math
import wx
from pubsub import pub
import multiprocessing as mp
# from matplotlib.widgets import Cursor

import logging
import matplotlib.dates as mdates
from datetime import datetime
import matplotlib

from matplotlib.figure import Figure
from matplotlib.backends.backend_wxagg import FigureCanvasWxAgg as FigureCanvas
matplotlib.use('WXAgg')

class StatisticsView(wx.Frame): 
   def __init__(self, parent): 
      super(StatisticsView, self).__init__(parent, title = "Statistics") 

      self.panel_one = BalancePanel(self)
      self.panel_two = BalancePanel(self)
      self.panel_two.Hide()
      
      self.sizer = wx.BoxSizer(wx.VERTICAL)
      self.SetMinSize((400,300))
      self.sizer.Add(self.panel_one, 1, wx.EXPAND | wx.ALL, 10)
      self.sizer.Add(self.panel_two, 1, wx.EXPAND | wx.ALL, 10)

      gridSizer = wx.GridSizer(1, 2,5,5)
      
      rbutton = wx.RadioButton(self,label="Balance Score")
      rbutton.SetFocus()
      
      rbutton1 = wx.RadioButton(self,label="Game Score")
      
      gridSizer.Add(rbutton, 0, wx.EXPAND)
      gridSizer.Add(rbutton1, 0, wx.EXPAND)
      self.sizer.Add(gridSizer, 0, wx.ALL|wx.EXPAND, 5)
      self.Bind(wx.EVT_CLOSE, self.onClose)
      self.Bind(wx.EVT_RADIOBUTTON, self.onSwitchPanels)
      self.SetSizerAndFit(self.sizer)
      
      pub.subscribe(self.plotBalanceScore, "statistics.game")
      pub.subscribe(self.plotGameScore, "statistics.balance")

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
   
   def plotBalanceScore(self, score):
      logging.info("Plotting Balance Score Statistics")
      if score[0] is not None and score[1] is not None and score[2] is not None:
         self.panel_one.plotScore(score)
      else:
         self.panel_one.noResults()
      logging.info("Caching Balance Score Statistics")
   
   def plotGameScore(self, score):
      logging.info("Plotting Game Score Statistics")
      if score[0] is not None and score[1] is not None and score[2] is not None:
         self.panel_two.plotScore(score)
      else:
         self.panel_two.noResults()
      logging.info("Caching Game Score Statistics")

   def onClose(self, event):
      logging.info("Closing Statistics View")
      self.Show(False)

class BalancePanel(wx.Panel):
   def __init__(self, parent):
      """Constructor"""
      wx.Panel.__init__(self, parent=parent)
      vert_sizer = wx.BoxSizer(wx.VERTICAL)
      self.figure = Figure()
   
      self.axes = self.figure.subplots()
      self.canvas = FigureCanvas(self, -1, self.figure)
      
      self.default = wx.StaticText(self, label="No Results to Display", style=wx.ALIGN_CENTER)
      
      vert_sizer.Add(self.default, 1, wx.EXPAND | wx.CENTER)
      self.default.Show(False)
      
      vert_sizer.Add(self.canvas, 1, wx.LEFT | wx.TOP | wx.EXPAND)
      self.canvas.draw()
      self.SetSizerAndFit(vert_sizer)
      self.vert_sizer = vert_sizer
      
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
      
   def noResults(self):
      self.axes.cla()
      self.default.Show(True)

   def plotScore(self, scores):
      self.default.Show(False)
      colors = ['r', 'b', 'g']
      labels = ["G1", "G2", "G3"]
      for i in range(0,len(scores)):
         if scores[i] == None:
            continue

         xScore, yScore = zip(*scores[i])
         dateScore = [datetime.fromtimestamp(x) for x in xScore]

         line, = self.axes.plot(dateScore, yScore, colors[i], ls = ':', picker = True, pickradius =3, marker = 'o')
         line.set_label(labels[i])
         
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