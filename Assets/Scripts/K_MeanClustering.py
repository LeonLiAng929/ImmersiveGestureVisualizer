"""
from scipy.io import loadmat
from pathlib import Path
import sys
import tqdm
from xml.etree import ElementTree as ET"""
import numpy as np
from tslearn.barycenters import dtw_barycenter_averaging
from tslearn.clustering import TimeSeriesKMeans
from tslearn.utils import to_time_series_dataset

import clr
#print(dir(clr))
import UnityEngine

gestureAnalyzer = UnityEngine.Object.FindObjectOfType(clr.GestureAnalyser)
gestures = gestureAnalyzer.GetGestures()

dataset_to_train = []
for g in gestures:
    series = []
    for i in range(0, g.num_of_poses):
        serie = []
        for joint in g.poses[i].joints:
            serie.append(joint.x)
            serie.append(joint.y)
            serie.append(joint.z)
        series.append(serie)
    dataset_to_train.append(series)

#print(dataset_to_train)

dataset_to_train = to_time_series_dataset(dataset_to_train)
print(dataset_to_train.shape)

kmeans = TimeSeriesKMeans(n_clusters=gestureAnalyzer.k, metric="dtw", verbose=1)
pred = kmeans.fit_predict(dataset_to_train)

print(pred)

for i in range(0, len(gestures)):
    gestures[i].cluster = int(pred[i])
