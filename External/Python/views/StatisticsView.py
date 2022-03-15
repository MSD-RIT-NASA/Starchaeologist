import math
import wx
from pubsub import pub
import multiprocessing as mp
# import mplcursors

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
      
      pub.subscribe(self.plotGameScore, "statistics.game")
      pub.subscribe(self.plotBalanceScore, "statistics.balance")

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
      exit(0)

class BalancePanel(wx.Panel):
   def __init__(self, parent):
      """Constructor"""
      wx.Panel.__init__(self, parent=parent)
      vert_sizer = wx.BoxSizer(wx.VERTICAL)
      self.figure = Figure()

      self.figure, self.axes = plt.subplots(1)

      self.lines = []
      self.canvas = FigureCanvas(self, -1, self.figure)
      
      self.default = wx.StaticText(self, label="No Results to Display", style=wx.ALIGN_CENTER)
      
      vert_sizer.Add(self.default, 1, wx.EXPAND | wx.CENTER)
      self.default.Show(False)
      
      vert_sizer.Add(self.canvas, 1, wx.LEFT | wx.TOP | wx.EXPAND)
      self.canvas.draw()
      self.SetSizerAndFit(vert_sizer)
      

      self.vert_sizer = vert_sizer
      self.annot_x = (plt.xlim()[1] + plt.xlim()[0])/2
      self.annot_y = (plt.ylim()[1] + plt.ylim()[0])/2
      # self.annot = self.axes.annotate("", xy=(0,0), xytext=(20,20),textcoords="offset points",
      #               bbox=dict(boxstyle="round", fc="w"),
      #               arrowprops=dict(arrowstyle="->"))
      self.annot = self.axes.annotate("", xy=(0,0), xytext=(5,5),textcoords="offset points")
      self.annot.set_visible(False)
      # xfmt = mdates.DateFormatter('%Y-%m-%d %H:%M:%S')
      # self.axes.xaxis.set_major_formatter(xfmt)
      
   def displayBreakdown(self, event):
      print("Something")
      thisline = event.artist
      xdata = thisline.get_xdata()
      # print(xdata)
      # event.mo
      ydata = thisline.get_ydata()
      # print(ydata)
      xVal = event.mouseevent.xdata
      ind = round(event.mouseevent.xdata)
      print(mdates.num2date(xVal))
      print(mdates.num2date(ind))
      print(xdata)
      # print(xdata[1])
      # print(xdata[2])
      # print(xdata[3])
      # # print(type(xDate))
      # print(xdata)
      # mdates.num2
      # # print(type(xdata[0]))
      # # print(type(xDate))
      # if date in xdata:
      #    index = xdata.index(date)
      #    print(index)
         
         # wx.MessageBox('x :'+ str(xdata[ind]) + ' y: ' + str(ydata[ind]), 'Info',wx.OK | wx.ICON_INFORMATION)
      
   def noResults(self):
      self.axes.cla()
      self.axes.text(self.annot_x, self.annot_y, "No Data", 
                  ha='center', fontsize=36, color='#DD4012')
      # self.default.Show(True)

   def plotScore(self, scores):
      print(scores)
      self.axes.text(self.annot_x, self.annot_y,"")
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
         self.lines.append(line)
         
      self.axes.xaxis.set_major_locator(mdates.DayLocator(bymonthday=range(1, 32)))
      self.axes.xaxis.set_minor_locator(mdates.DayLocator())
      self.axes.grid(True)
      self.axes.legend()
      
      # mplcursors.cursor(hover=True)
      self.canvas.draw()
      self.annot = self.axes.annotate("", xy=(0,0), xytext=(5,5),textcoords="offset points")
      self.annot.set_visible(False)
      # mplcursors.cursor(hover=True)
      # self.canvas.mpl_connect('pick_event', self.displayBreakdown)
      self.canvas.mpl_connect("motion_notify_event", self.hover)

   def hover(self, event):
      for line in self.axes.get_lines():
         if line.contains(event)[0]:
            linePointsX = line.get_xdata()
            linePointsY = line.get_ydata()
            idx = min(range(len(linePointsX)), key=lambda i: abs(mdates.date2num(linePointsX[i])-event.xdata))
            # self.axes.text(event.xdata,event.ydata + 0.5, "Date:" + str(linePointsY[idx]))
            self.annot.xy = (event.xdata, event.ydata)
            # print(self.annot.xy)
            # self.axes.annotate("", xy=(linePointsX[idx],
            #    linePointsY[idx]+40),
            #    xytext=(linePointsX[idx],
            #    linePointsY[idx]+500), 
            #    arrowprops=dict(arrowstyle='simple', color='black'))
            # line.annotate("%i" % 10, (10 + 10, 10 + 10), ha= 'center')
            # write the name of every point contained in the event
            # self.annot.set_text("{}".format(', '.join([str(linePointsY[n]) for n in [idx]])))
            # # self.annot.set_text("Date:" + str(linePointsY[idx]))
            # self.annot.set_visible(True)    
         # else:
         #    # self.annot.set_visible(False)
         #    pass
   
            # print(idx)
            # print(event.xdata)
      # pass

      # 
        
if __name__ == '__main__':
   app = wx.App()
   view = StatisticsView(None)
   # bScore = [None, None, None]
   # gScore = [None, None, None]
   # view.plotBalanceScore(bScore)
   # view.plotGameScore(gScore)
   bScore = [[(1646934163, 365), (1647012919, 91), (1647012919, 5), (1647099955, 70)], [(1647012919, 55), (1647012919, 51), (1647012919, 59), (1647099955, 50)], [(1647012919, 50), (1647099955, 30)]]
   gScore = [[(1646934163, 65), (1647012919, 56), (1647099955, 10)], [(1647012919, 15), (1647012919, 19), (1647012919, 15), (1647099955, 60)], [(1647012919, 18), (1647012919, 81), (1647099955, 30)]]
   view.plotBalanceScore(bScore)
   view.plotGameScore(gScore)
   view.Show()
   app.MainLoop()
   
   # app=wx.PySimpleApp()
   # frame=TestFrame(None,"Hello Matplotlib !")
   # frame.Show()
   # app.MainLoop()