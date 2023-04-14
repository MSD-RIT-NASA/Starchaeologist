import matlab.engine as engine
import matplotlib.pyplot as plt
from PIL import Image
from datetime import datetime
import numpy as np
import os

file_path = os.path.realpath(__file__)
root = os.path.dirname(file_path)


def run(path, user, GreenScore, YellowScore, OrangeScore):

    now = datetime.now()
    eng = engine.start_matlab()
    eng.cd(root, nargout=0)
    [CometX, CometY, R_score_rt, FinalScore, mat] = eng.DataAnalysis(path, GreenScore, YellowScore, OrangeScore, nargout=15)

    if FinalScore != 0:
        fig, (comG, lineG, heatG) = plt.subplots(ncols=3, nrows=1, figsize=(16, 7))

        def comet(x, y=None, time=0.05, fill=False):
            x = np.asarray(x)
            plt.ion()
            plt.xlim(x.min(), x.max())
            plt.axis("off")
            if y is not None:
                y = np.asarray(y)
                plt.ylim(y.min(), y.max())
            else:
                plt.ylim(0, len(x))
            if y is not None:
                plot = plt.plot(x[0], y[0])[0]
            else:
                plot = plt.plot(x[0])[0]

            for i in range(len(x) + 1):
                if y is not None:
                    plot.set_data(x[0:i], y[0:i])
                else:
                    plot.set_xdata(x[0:i])
                plt.draw()
                plt.pause(time)

            if fill:
                plt.fill_between(x, y, zorder=100)
            plt.ioff()

        # Format figure
        fig.suptitle(user + '\nBASE Balance Score: ' + str(FinalScore) + "\n" + str(now.strftime("%d/%m/%Y %I:%M %p")), fontsize=16)
        fig.tight_layout()

        # Display stats
        plt.savefig(path+"/results.png")
        img = Image.open(path+"/results.png")
        img.show()
        return FinalScore
    else:
        return 0