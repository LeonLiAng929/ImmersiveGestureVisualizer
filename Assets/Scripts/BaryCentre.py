from array import array

from tslearn.barycenters import dtw_barycenter_averaging
# from tslearn.clustering import TimeSeriesKMeans
from tslearn.utils import to_time_series_dataset
import clr
#print(dir(clr))
import UnityEngine
import numpy as np


gestureAnalyzer = UnityEngine.Object.FindObjectOfType(clr.GestureAnalyser)
#gestures = gestureAnalyzer.GetGestures()

dataset_to_train = gestureAnalyzer.pythonArguments

#print(dataset_to_train)

dataset_to_train = to_time_series_dataset(dataset_to_train)
print(dataset_to_train.shape)
barycentre = dtw_barycenter_averaging(dataset_to_train)
print(barycentre.shape)
barycentre = barycentre.tolist()
print(str(len(barycentre)) + ' '+ str(len(barycentre[0])))
gestureAnalyzer.InitializePythonResult(len(barycentre), len(barycentre[0]))

for i in range(0, len(barycentre)):
    for j in range(0, len(barycentre[i])):
         gestureAnalyzer.LogPythonResult(i,j,barycentre[i][j])

"""
# write barycentre to a csv
# open the file in the write mode
f = open('D/baryCentre.csv', 'w')

# create the csv writer
writer = csv.writer(f)

for i in range(0, len(barycentre)):
    #for j in range(0, len(barycentre[i])):
         #gestureAnalyzer.pythonResult[i][j]=barycentre[i][j]
# write a row to the csv file
    writer.writerow(barycentre[i])

# close the file
f.close()
"""
#print(barycentre.shape)
#print(barycentre)
