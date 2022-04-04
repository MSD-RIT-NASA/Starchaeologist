import math
import wx
from pubsub import pub
import multiprocessing as mp
# import mplcursors

import numpy as np

# from matplotlib.widgets import Cursor

import logging
import matplotlib.dates as mdates
from datetime import datetime
import matplotlib

import matplotlib.pyplot as plt

from matplotlib.figure import Figure
from matplotlib.backends.backend_wxagg import FigureCanvasWxAgg as FigureCanvas
matplotlib.use('WXAgg')

class StatisticsView(wx.Frame): 
   def __init__(self, parent): 
      super(StatisticsView, self).__init__(parent, title = "Statistics") 

      self.panel_one = ScorePanel(self)
      self.panel_two = ScorePanel(self)
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
      self.SetTitle("Balance Score Showing")
      self.Centre()
      
      pub.subscribe(self.plotGameScore, "statistics.game")
      pub.subscribe(self.plotBalanceScore, "statistics.balance")

   def onSwitchPanels(self, event):
      """
      Switch between Game Score and Balance Score Panels
      """
      radioBox = event.GetEventObject() 
      if radioBox.GetLabel() == "Game Score":
         self.SetTitle("Game Score Showing")
         self.panel_one.Hide()
         self.panel_two.Show()
      else:
         self.SetTitle("Balance Score Showing")
         self.panel_one.Show()
         self.panel_two.Hide()
      self.Layout()
   
   def plotBalanceScore(self, score):
      """
      Plot Balance Score
      """
      logging.info("Plotting Balance Score Statistics")
      if score[0] is not None and score[1] is not None and score[2] is not None:
         self.panel_one.plotScore(score)
      else:
         self.panel_one.noResults()
      logging.info("Caching Balance Score Statistics")
   
   def plotGameScore(self, score):
      """
      Plot Game Score
      """
      logging.info("Plotting Game Score Statistics")
      if score[0] is not None and score[1] is not None and score[2] is not None:
         self.panel_two.plotScore(score)
      else:
         self.panel_two.noResults()
      logging.info("Caching Game Score Statistics")

   def onClose(self, event):
      """
      Close Statistics View
      """
      logging.info("Closing Statistics View")
      self.Show(False)

class ScorePanel(wx.Panel):
   def __init__(self, parent):
      """Constructor"""
      wx.Panel.__init__(self, parent=parent)
      vert_sizer = wx.BoxSizer(wx.VERTICAL)
      self.figure = Figure()

      self.axes = self.figure.subplots()
      self.axes.set_xlabel("Time")
      self.axes.set_ylabel("Score")

      self.lines = []
      self.canvas = FigureCanvas(self, -1, self.figure)

      self.default = wx.StaticText(self, label="No Results to Display", style=wx.ALIGN_CENTER)
      
      vert_sizer.Add(self.default, 1, wx.EXPAND | wx.CENTER)
      self.default.Show(False)
      
      vert_sizer.Add(self.canvas, 1, wx.LEFT | wx.TOP | wx.EXPAND)
      self.SetSizerAndFit(vert_sizer)
      
      self.vert_sizer = vert_sizer
      self.annot_x = (plt.xlim()[1] + plt.xlim()[0])/2
      self.annot_y = (plt.ylim()[1] + plt.ylim()[0])/2
      self.annot = self.axes.annotate("", xy=(0,0), xytext=(-20,20), 
         textcoords="offset points", bbox=dict(boxstyle="round", fc="w"),
         arrowprops=dict(arrowstyle="->"))  
      self.annot.set_visible(False)

      self.canvas.mpl_connect("motion_notify_event", self.hover)
 
      self.canvas.draw()
         
   def noResults(self):
      """
      Display when user has no scores in the panel 
      """
      self.axes.clear()
      self.axes.text(self.annot_x, self.annot_y, "No Data", 
                  ha='center', fontsize=36, color='#DD4012')
      self.canvas.draw()

   def plotScore(self, scores):
      """
      Plot scores into the axis, where each line represents a different game
      """
      self.axes.clear()
      self.axes.text(self.annot_x, self.annot_y,"",ha='center', fontsize=36, color='#DD4012')
      self.default.Show(False)
      colors = ['r', 'b', 'g']
      labels = ["Game 1", "Game 2", "Game 3"]

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
      
   def hover(self, event):
      vis = self.annot.get_visible()
      if event.inaxes == self.axes:
         for line in self.axes.get_lines():
            cont, ind = line.contains(event)
            if cont:
               self.update_annot(ind, line)
               self.annot.set_visible(True)
               self.canvas.draw_idle()
            else:
               if vis:
                  self.annot.set_visible(False)
                  self.canvas.draw_idle()

   def update_annot(self, ind, line):
      x, y = line.get_data()
      self.annot.xy = (x[ind["ind"][0]], y[ind["ind"][0]])
      text = str(x[ind["ind"][0]]) + ", "+ str(y[ind["ind"][0]])
      self.annot.set_text(text)
      self.annot.get_bbox_patch().set_alpha(0.4)
              
if __name__ == '__main__':
   app = wx.App()
   view = StatisticsView(None)
   
   # bScore = [None, None, None]
   # gScore = [None, None, None]
   # view.plotBalanceScore(bScore)
   # view.plotGameScore(gScore)

   bScore = [[(1646934163, 365), (1647012919, 91), (1647012919, 5), (1647099955, 70)], [(1647012919, 55), (1647012919, 51), (1647012919, 59), (1647099955, 50)], [(1647012919, 50), (1647099955, 30)]]
   # gScore = [[(1646934163, 65), (1647012919, 56), (1647099955, 10)], [(1647012919, 15), (1647012919, 19), (1647012919, 15), (1647099955, 60)], [(1647012919, 18), (1647012919, 81), (1647099955, 30)]]
   gScore = [[(1647012919, 15), (1647032929, 19), (1647072949, 15), (1647099955, 60)], [(1647012919, 18), (1647062949, 81), (1647099955, 30)]]
   view.plotBalanceScore(bScore)
   view.panel_two.plotScore(gScore)

   # bScore = [None, None, None]
   # gScore = [None, None, None]
   # view.plotBalanceScore(bScore)
   # view.plotGameScore(gScore)

   view.Show()

   app.MainLoop()
   
   # app=wx.PySimpleApp()
   # frame=TestFrame(None,"Hello Matplotlib !")
   # frame.Show()
   # app.MainLoop()