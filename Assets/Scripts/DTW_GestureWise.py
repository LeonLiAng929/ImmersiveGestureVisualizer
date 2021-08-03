"""
from scipy.io import loadmat
from pathlib import Path
import sys
import tqdm
from xml.etree import ElementTree as ET"""
import numpy as np
# from tslearn.barycenters import dtw_barycenter_averaging
from tslearn.utils import to_time_series
from tslearn.metrics import dtw

import clr
import UnityEngine

gestureAnalyzer = UnityEngine.Object.FindObjectOfType(clr.GestureAnalyser)
dataset_to_train = gestureAnalyzer.pythonArguments
g1 = dataset_to_train[0]
g2 = dataset_to_train[1]
g1 = to_time_series(g1, remove_nans=True)
g2 = to_time_series(g2, remove_nans=True)

score = dtw(g1, g2)

gestureAnalyzer.similarityScore = score

print(score)