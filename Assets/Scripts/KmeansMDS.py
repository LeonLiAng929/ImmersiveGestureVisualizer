"""
from scipy.io import loadmat
from pathlib import Path
import sys
import tqdm
from xml.etree import ElementTree as ET"""
import numpy as np
from tslearn.barycenters import dtw_barycenter_averaging
from sklearn.cluster import KMeans
from tslearn.utils import to_time_series_dataset

import clr
#print(dir(clr))
import UnityEngine

gestureAnalyzer = UnityEngine.Object.FindObjectOfType(clr.GestureAnalyser)
gestures = gestureAnalyzer.GetGestures()

dataset_to_train = []
for g in gestures:
    coord = []
    coord.append(g.MDS_Coordinate[0])
    coord.append(g.MDS_Coordinate[1])
    dataset_to_train.append(coord)

dataset_to_train = np.asarray(dataset_to_train)
#print(dataset_to_train.shape)

#dataset_to_train = to_time_series_dataset(dataset_to_train)
#print(dataset_to_train.shape)

kmeans = KMeans(n_clusters=gestureAnalyzer.k, init='k-means++', verbose=1).fit(dataset_to_train)
#print(kmeans.labels_)
#print(kmeans.cluster_centers_.shape)

for i in range(0, len(gestures)):
    gestures[i].cluster = int(kmeans.labels_[i])

gestureAnalyzer.InitializePythonResult(len(kmeans.cluster_centers_), len(kmeans.cluster_centers_[0]))
for i in range(0, len(kmeans.cluster_centers_)):
    for j in range(0, len(kmeans.cluster_centers_[i])):
         gestureAnalyzer.LogPythonResult(i,j,kmeans.cluster_centers_[i][j])
