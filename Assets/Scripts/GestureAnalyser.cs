using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Scripting.Python;
using UnityEngine;

public class GestureAnalyser : MonoBehaviour
{
    [SerializeField]
    public int k;

    public List<List<List<float>>> pythonArguments = new List<List<List<float>>>();
    public List<List<float>> pythonResult = new List<List<float>>();
    
    private List<Gesture> gestures = new List<Gesture>();
    private Dictionary<int,Cluster> clusters = new Dictionary<int,Cluster>();
    
    #region Singleton
    public static GestureAnalyser instance;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        // loading and wranglng raw data
        IO xmlLoader = new IO();

        gestures = xmlLoader.LoadXML("angry like a bear-1");

        foreach (Gesture g in gestures)
        {
            g.SetBoundingBox();
            g.SetCentroid();
            g.TranslateToOrigin();
            g.NormalizeHeight();
        }

        //clustering
        PythonRunner.RunFile("Assets/Scripts/K_MeanClustering.py");

        //Calculate the barycentre for the dataset
        //PythonRunner.RunFile("Assets/Scripts/BaryCentre.py");

        // initialize clusters
        foreach (Gesture g in gestures)
        {
            Cluster temp = GetCluster(g.cluster);
            temp.AddGesture(g);
        }

        // calculate barycenter for each cluster

        foreach (KeyValuePair<int, Cluster> pair in clusters)
        {
            if (pair.Value.GestureCount() > 1){
                CalculateBaryCentre(pair.Value.GetGestures());
                pair.Value.SetBaryCentre(Python2CSharp()); }
            else
            {
                pair.Value.SetBaryCentre(pair.Value.GetGestures()[0]);
            }
        }
      
    }
    Cluster GetCluster(int id)
    {
        if (clusters.ContainsKey(id))
            return clusters[id];
        else
        {
            Cluster tempCluster = new Cluster();
            tempCluster.clusterID = id;
            clusters.Add(id, tempCluster);
            return clusters[id];
        }
    }
    public Cluster GetClusterByID(int id)
    {
       return clusters[id];
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Gesture> GetGestures()
    {
        return gestures;
    }
    
    public Dictionary<int,Cluster> GetClusters()
    {
        return clusters;
    }

    public void CSharp2Python(List<Gesture> gList)
    {
        pythonArguments.Clear();
        foreach(Gesture g in gList)
        {
            pythonArguments.Add(g.DataWranglingForPython());  
        }
    }

    public void CalculateBaryCentre(List<Gesture> gList)
    {
        CSharp2Python(gList);

        PythonRunner.RunFile("Assets/Scripts/BaryCentre.py");
    }

    Gesture Python2CSharp()
    {
        Gesture temp = new Gesture();
        temp.num_of_poses = pythonResult.Count;
        //Debug.Log(pythonResult[0][0]);
        float timestamp = 0;
        for(int i =0; i< temp.num_of_poses; i++)
        {
            Pose pose = new Pose();
            pose.num_of_joints = 20;
            pose.timestamp = timestamp + i * 100;
            for (int j =0; j< pose.num_of_joints *3; j+=3)
            {
                float x = pythonResult[i][j];
                float y = pythonResult[i][j + 1];
                float z = pythonResult[i][j + 2];
                Joint joint = new Joint(x, y, z, gestures[0].poses[0].joints[j/3].jointType);
                pose.joints.Add(joint);
            }
            temp.poses.Add(pose);
        }
        return temp;
    }

   public void InitializePythonResult(int x, int y)
    {
        for (int i = 0; i < x; i++)
        {
            pythonResult.Add(new List<float>());
            for(int j = 0; j < y; j++)
            {
                pythonResult[i].Add(j);
            }
        }
    }

   public void LogPythonResult(int i, int j, float value)
    {
        pythonResult[i][j] = value;
    }
}
