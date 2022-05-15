import numpy as np
from sklearn.decomposition import PCA
from sklearn.cluster import MeanShift, estimate_bandwidth
import clr
#print(dir(clr))
import UnityEngine

gestureAnalyzer = UnityEngine.Object.FindObjectOfType(clr.GestureAnalyser)
dataset_to_train = gestureAnalyzer.pythonArguments
gestures = gestureAnalyzer.GetGestures()

flattened = []
for gesture in dataset_to_train:
    li = []
    for pose in gesture:
        for k in pose:
            li.append(k)
    flattened.append(li)

flattened = np.asarray(flattened)

print(flattened.shape)
ms = MeanShift()
ms.fit(flattened)
labels = ms.labels_

labels_unique = np.unique(labels)
n_clusters_ = len(labels_unique)
print(n_clusters_)
if gestureAnalyzer.estimationOnly:
    gestureAnalyzer.kPrediction = n_clusters_
else:
    gestureAnalyzer.k=n_clusters_
    for i in range(0, len(gestures)):
        gestures[i].cluster = int(ms.labels_[i])

    gestureAnalyzer.InitializePythonResult(len(ms.cluster_centers_), len(ms.cluster_centers_[0]))
    for i in range(0, len(ms.cluster_centers_)):
        for j in range(0, len(ms.cluster_centers_[i])):
             gestureAnalyzer.LogPythonResult(i,j,ms.cluster_centers_[i][j])