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
from tslearn.utils import to_time_series
from sklearn.manifold import MDS
import clr
#print(dir(clr))
import UnityEngine

gestureAnalyzer = UnityEngine.Object.FindObjectOfType(clr.Experiment)
dataset_to_train = gestureAnalyzer.pythonArguments
gestures = gestureAnalyzer.GetGestures()
num_of_joints = gestures[0].poses[0].num_of_joints
#print(dataset_to_train)

flattened = []
for gesture in dataset_to_train:
    li = []
    for pose in gesture:
        for k in pose:
            li.append(k)
    flattened.append(li)

flattened = np.asarray(flattened)

#print(flattened.shape)

mds = MDS(n_components=2)
#mds.fit(flattened)
output = mds.fit_transform(flattened)
#print(output.shape)
#print(output)
gestureAnalyzer.InitializePythonResult(len(output), len(output[0]))

for i in range(0, len(output)):
    for j in range(0, len(output[i])):
         gestureAnalyzer.LogPythonResult(i,j,output[i][j])
#for i in range(0, len(gestures)):
 #   gestures[i].cluster = int(pred[i])
