from tkinter import *
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from matplotlib.figure import Figure
import matplotlib.animation as animation
import numpy as np
import random
import time

# for testing performance
DATA_POLLING_FREQ = 1000
DATA_DISPLAY_FREQ = 10
USE_BLIT = True
PLOT_BOOL = True

class BioxApp(Tk):
    def __init__(self):
        global f
        global a
        global barcollection

        self.calls = 0
        self.master = Tk()

        f = Figure(figsize=(5, 2), dpi=122)
        a = f.add_subplot(111)

        self.canvas = FigureCanvasTkAgg(f, master=self.master)
        self.canvas.draw()

        self.canvas.get_tk_widget().grid(
            column=4, row=7, columnspan=3, rowspan=3, pady=5, padx=2
        )

        # start data polling
        self.master.after(1000 // DATA_POLLING_FREQ, self.data_source)

        # start frequency measurement
        self.freq_text = StringVar()
        self.freq_text.set("")
        self.freq_label = Label(textvariable=self.freq_text)
        self.freq_label.grid(column=1, row=1, columnspan=7)
        self.master.after(1000, self.freqency)

        # initial bar plot
        n_sensor = np.arange(8)
        data =  [20] * 8
        a.clear()
        a.set_ylim(0, 30)  # fixed y axis scala, valid only after clear()
        barcollection = a.bar(n_sensor, data)

    def freqency(self):
        self.freq_text.set(
            f"source_in update @ {self.calls} Hz\t{self.calls/DATA_POLLING_FREQ:.0%}"
        )
        self.calls = 0
        self.master.after(1000, self.freqency)

    def data_source(self):
        global sample_in
        self.calls += 1
        for idx in range(8):
            sample_in[idx] = random.gauss(20, 2)
        self.master.after(1000 // DATA_POLLING_FREQ, self.data_source)

    def mainloop(self):
        self.master.mainloop()


def animate(data):
    """
    Plot animation for live graph.
    """
    global sample_in
    global plot_bool
    global barcollection

    if plot_bool:
        data = sample_in
        for i, b in enumerate(barcollection):
            b.set_height(data[i])
    return barcollection

if __name__ == "__main__":
    # globals
    sample_in = [20] * 8
    plot_bool = PLOT_BOOL
    f = None
    a = None

    app = BioxApp()
    if plot_bool:
        ani = animation.FuncAnimation(
            f, animate, interval=1000 // DATA_DISPLAY_FREQ, blit=USE_BLIT
        )
    app.mainloop()