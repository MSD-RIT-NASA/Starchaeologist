import wx
from pubsub import pub


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
      if score[0] is not None and score[1] is not None:
         self.panel_one.plotScore(score, True)
      else:
         self.panel_one.noResults()
      logging.info("Caching Balance Score Statistics")
   
   def plotGameScore(self, score):
      """
      Plot Game Score
      """
      logging.info("Plotting Game Score Statistics")
      if score[0] is not None and score[1] is not None:
         self.panel_two.plotScore(score, False)
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

      self.fig, self.axes = plt.subplots()
      
      self.axes.set_xlabel("Time")
      self.axes.set_ylabel("Score")

      self.lines = []
      self.canvas = FigureCanvas(self, -1, self.fig)

      self.default = wx.StaticText(self, label="No Results to Display", style=wx.ALIGN_CENTER)
      
      vert_sizer.Add(self.default, 1, wx.EXPAND | wx.CENTER)
      self.default.Show(False)
      
      vert_sizer.Add(self.fig.canvas, 1, wx.LEFT | wx.TOP | wx.EXPAND)
      self.SetSizerAndFit(vert_sizer)
      self.mean = []
      self.std = [] 
      self.length = [] 
      self.centX = [] 
      self.centY = []

      self.vert_sizer = vert_sizer
      self.annot_x = (plt.xlim()[1] + plt.xlim()[0])/2
      self.annot_y = (plt.ylim()[1] + plt.ylim()[0])/2
         
   def noResults(self):
      """
      Display when user has no scores in the panel 
      """
      self.axes.clear()
      self.mean = []
      self.std = [] 
      self.length = [] 
      self.centX = [] 
      self.centY = []
      self.axes.text(self.annot_x, self.annot_y, "No Data", 
                  ha='center', fontsize=36, color='#DD4012')
      self.canvas.draw()

   def plotScore(self, scores, flag):
      """
      Plot scores into the axis, where each line represents a different game
      """
      self.axes.clear()
      self.axes.text(self.annot_x, self.annot_y,"jk",ha='center', fontsize=36, color='#DD4012')
      self.default.Show(False)
      colors = ['r', 'b', 'g']
      labels = ["Game 1", "Game 2", "Game 3"]
      lines = []
      for i in range(0,len(scores)):
         if scores[i] == None:
            continue
         
         xScore, yScore, mean, std, length, centX, centY = zip(*scores[i])
         dateScore = [datetime.fromtimestamp(x) for x in xScore]
         
         line, = self.axes.plot(dateScore, yScore, colors[i], ls = ':', picker = True, pickradius =3, marker = 'o')
         line.set_label(labels[i])
         line.set_picker(5)
         self.mean.append(mean)
         self.std.append(std)
         self.length.append(length)
         self.centX.append(centX)
         self.centY.append(centY)

      self.axes.xaxis.set_major_locator(mdates.DayLocator(bymonthday=range(1, 32)))
      self.axes.xaxis.set_minor_locator(mdates.DayLocator())
      self.axes.grid(True)
      self.axes.legend()
      if flag:
         self.fig.canvas.mpl_connect("pick_event", self.on_pick)
      self.fig.canvas.draw()
   
   def on_pick(self,event):
      ind = event.ind[0]
      thisline = event.artist
      date_time = thisline.get_xdata()[ind].strftime("%m/%d/%Y, %H:%M:%S")
      if thisline.get_label() == "Game 2":
         wx.MessageDialog(None,
               "Recorded on "+ str(date_time) + "\n" +
               "Average COP: "+ str(round(self.mean[1][ind], 2)) + "\n" +
               "Standard Deviation of COP: "+ str(round(self.std[1][ind], 2)) + "\n" +
               "Length of COP: "+ str(round(self.length[1][ind], 2)) + "\n" +
               "COP centers around: ("+ str(round(self.centX[1][ind], 2)) + ", " + str(round(self.centX[1][ind], 2)) +")\n" +
               "Final Score: " + str(round(thisline.get_ydata()[ind],2)),
               "Game 2 Balance Score Breakdown",
               wx.OK | wx.CLOSE
         ).ShowModal()
      else:
         r = wx.MessageDialog(None,
               "Recorded on "+ str(date_time) + "\n" +
               "Mean COP: "+ str(round(self.mean[0][ind], 2)) + "\n" +
               "Standard Deviation of COP: "+ str(round(self.std[0][ind], 2)) + "\n" +
               "Length of COP: "+ str(round(self.length[0][ind], 2)) + "\n" +
               "COP centers around: ("+ str(round(self.centX[0][ind], 2)) + ", " + str(round(self.centX[0][ind], 2)) +")\n" +
               "Final Score: " + str(round(thisline.get_ydata()[ind],2)),
               "Game 1 Balance Score Breakdown",
               wx.OK | wx.CLOSE
         ).ShowModal()

if __name__ == '__main__':
   app = wx.App()
   view = StatisticsView(None)
   
   bScore = [[(1648937325, 95, 80.0, 40.0, 30.0, 20.0, 10.0),(1648997325, 195, 80.0, 40.0, 30.0, 20.0, 10.0)], [(1648937325, 93, 12.675686465930625, 5.855042106763086, 22.91533792001619, -0.9545454545454546, 0.4090909090909091), (1648948277, 151.9689104732696, 12.675686465930625, 5.855042106763086, 22.91533792001619, -0.9545454545454546, 0.4090909090909091)]]
   # gScore = [[(1646934163, 65), (1647012919, 56), (1647099955, 10)], [(1647012919, 15), (1647012919, 19), (1647012919, 15), (1647099955, 60)], [(1647012919, 18), (1647012919, 81), (1647099955, 30)]]
   gScore = [[(1646934163, 695, 0.0, 0.0, 0.0, 0.0, 0.0)], [(1646934163, 695, 0.0, 0.0, 0.0, 0.0, 0.0), (1646954163, 191, 0.0, 0.0, 0.0, 0.0, 0.0)]]
   
   view.plotGameScore(gScore)
   view.plotBalanceScore(bScore)
   # bScore = [None, None, None]
   # gScore = [None, None, None]
   # view.plotBalanceScore(bScore)
   # view.plotGameScore(gScore)

   view.Show()

   app.MainLoop()