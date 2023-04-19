import matlab.engine as engine
import matplotlib.pyplot as plt
from PIL import Image
from datetime import datetime
import os

file_path = os.path.realpath(__file__)
root = os.path.dirname(file_path)


def run(path, user, c_time, d_time, softleftbound, hardleftbound, softforwardbound, hardforwardbound):

    now = datetime.now()
    eng = engine.start_matlab()
    eng.cd(root, nargout=0)
    [score, l_lr_x, l_lr_y, b_lr_ell, b_lr_mll, b_lf_c,  b_lr_mrl, b_lr_erl, l_fb_x, l_fb_y, b_fb_ef, b_fb_mf, b_fb_c,  b_fb_mb, b_fb_eb] = eng.Planets_Analysis(path, c_time, d_time, softleftbound, hardleftbound, softforwardbound, hardforwardbound, nargout=15)

    if score != 0:
        fig, ((l_lr, l_fb), (b_lr, b_fb)) = plt.subplots(ncols=2, nrows=2, figsize=(16, 7))

        # BAR GRAPH LEFT_RIGHT
        bars_lr = ['Extreme Left', 'Slight Left', 'Centered', 'Slight Right', 'Extreme Right']
        counts_lr = [b_lr_ell, b_lr_mll, b_lf_c, b_lr_mrl, b_lr_erl]
        b_lr.bar(bars_lr, counts_lr)
        b_lr.set_ylabel('Percentage of Time Spent')
        b_lr.set_title('Time Spent at Severity of Left & Right Lean')

        # BAR GRAPH FRONT_BACK
        bars_fb = ['Extreme Forward', 'Slight Forward', 'Centered', 'Slight Back', 'Extreme Back']
        counts_fb = [b_fb_ef, b_fb_mf, b_fb_c,  b_fb_mb, b_fb_eb]
        b_fb.bar(bars_fb, counts_fb)
        b_fb.set_ylabel('Percentage of Time Spent')
        b_fb.set_title('Time Spent at Severity of Forward & Backward Lean')

        # LINE GRAPH LEFT_RIGHT
        l_lr.plot(l_lr_x, l_lr_y)
        l_lr.grid()
        l_lr.axhline(y=softleftbound, color=(0.9290, 0.6940, 0.1250), linestyle='-', alpha=0.5)
        l_lr.axhline(y=-1*softleftbound, color=(0.9290, 0.6940, 0.1250), linestyle='-', alpha=0.5)
        l_lr.axhline(y=hardleftbound, color=(0.6350, 0.0780, 0.1840), linestyle='-', alpha=0.5)
        l_lr.axhline(y=-1*hardleftbound, color=(0.6350, 0.0780, 0.1840), linestyle='-', alpha=0.5)

        # LINE GRAPH FRONT_BACK
        l_fb.plot(l_fb_x, l_fb_y)
        l_fb.grid()
        l_fb.axhline(y=softforwardbound, color=(0.9290, 0.6940, 0.1250), linestyle='-', alpha=0.5)
        l_fb.axhline(y=-1*softforwardbound, color=(0.9290, 0.6940, 0.1250), linestyle='-', alpha=0.5)
        l_fb.axhline(y=hardforwardbound, color=(0.6350, 0.0780, 0.1840), linestyle='-', alpha=0.5)
        l_fb.axhline(y=-1*hardforwardbound, color=(0.6350, 0.0780, 0.1840), linestyle='-', alpha=0.5)

        # Format figure
        fig.suptitle(user + '\nBalance Score: ' + str(score) + "\n" + str(now.strftime("%d/%m/%Y %I:%M %p")), fontsize=16)
        fig.tight_layout()

        # Display stats
        plt.savefig(path+"/results.png")
        img = Image.open(path+"/results.png")
        img.show()
        return score
    else:
        return 0