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
    [CometX, CometY, R_score_rt, FinalScore, mat] = eng.DataAnalysis(path, GreenScore, YellowScore, OrangeScore, nargout=5)

    if FinalScore != 0:
        return FinalScore
    else:
        return 0