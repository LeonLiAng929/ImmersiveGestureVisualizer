using System.Collections.Generic;
using UnityEditor.Scripting.Python;
using UnityEngine;

public class GestureAnalyser : MonoBehaviour
{
    [SerializeField]
    public int k;  // k for k-means clustering

    public List<List<List<float>>> pythonArguments = new List<List<List<float>>>();
    public List<List<float>> pythonResult = new List<List<float>>();
    public float similarityScore = 0;
    private List<Gesture> gestures = new List<Gesture>();
    private Dictionary<int,Cluster> clusters = new Dictionary<int,Cluster>();
    private Gesture averageGesture; //average gesture of the entire dataset
    private float globalConsensus;
    #region Singleton
    public static GestureAnalyser instance;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        // loading and wranglng raw data
        IO xmlLoader = new IO();

        //temporarily hardcoded to angrylike a bear.
        gestures = xmlLoader.LoadXML("angry like a bear");

        foreach (Gesture g in gestures)
        {
            g.SetBoundingBox();
            g.SetCentroid();
            g.TranslateToOrigin();
            g.NormalizeHeight();
        }

    }

    private void Start()
    {
        //PCA_Arrangement();
        //PythonRunner.RunFile("Assets/Scripts/K_Means_PCA.py");
        
    }
    public void InitializeClusters_DBA(int _k)
    {
        k = _k;
        //clustering
        PythonRunner.RunFile("Assets/Scripts/K_MeanClustering.py");

        //Calculate the barycentre for the dataset
        CSharp2Python(gestures);
        PythonRunner.RunFile("Assets/Scripts/BaryCentre.py");
        averageGesture = Python2CSharp();
        averageGesture.SetBoundingBox();
        averageGesture.SetCentroid();

        // initialize clusters
        foreach (Gesture g in gestures)
        {
            Cluster temp = TryGetCluster(g.cluster);
            temp.AddGesture(g, true);
        }

        // calculate barycenter for each cluster

        foreach (KeyValuePair<int, Cluster> pair in clusters)
        {
            if (pair.Value.GestureCount() > 1)
            {
                //CalculateBaryCentre(pair.Value.GetGestures());
                //pair.Value.SetBaryCentre(Python2CSharp());
                pair.Value.UpdateBarycentre();
                pair.Value.UpdateConsensus();
            }
            else
            {
                pair.Value.SetBaryCentre(pair.Value.GetGestures()[0]);
                pair.Value.UpdateConsensus();
            }
        }
        CalculateGlobalConsensus();
        PCA_Arrangement();
    }

    public void InitializeClusters_PCA(int _k)
    {
        k = _k;
        PCA_Arrangement();
        //clustering
        PythonRunner.RunFile("Assets/Scripts/KmeansPCA.py");
        List<List<float>> clusterCentres = new List<List<float>>();
        foreach(List<float> coordinate in pythonResult)
        {
            clusterCentres.Add(coordinate);
        }

        //Calculate the barycentre for the dataset
        CSharp2Python(gestures);
        PythonRunner.RunFile("Assets/Scripts/BaryCentre.py");
        averageGesture = Python2CSharp();
        averageGesture.SetBoundingBox();
        averageGesture.SetCentroid();

        // initialize clusters
        foreach (Gesture g in gestures)
        {
            Cluster temp = TryGetCluster(g.cluster);
            temp.AddGesture(g, true);
        }

        // calculate barycenter for each cluster

        foreach (KeyValuePair<int, Cluster> pair in clusters)
        {
            if (pair.Value.GestureCount() > 1)
            {
                //CalculateBaryCentre(pair.Value.GetGestures());
                //pair.Value.SetBaryCentre(Python2CSharp());
                pair.Value.UpdateBarycentre();
                pair.Value.UpdateConsensus();
            }
            else
            {
                pair.Value.SetBaryCentre(pair.Value.GetGestures()[0]);
                pair.Value.UpdateConsensus();
            }
            pair.Value.SetBaryCentrePCA_Coord(new Vector2(clusterCentres[pair.Key][0], clusterCentres[pair.Key][1]));
        }
        CalculateGlobalConsensus();
    }
    public int GetGestureCount()
    {
        return gestures.Count;
    }
    public Cluster TryGetCluster(int id)
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
        if (gList.Count > 1)
        {
            CSharp2Python(gList);

            PythonRunner.RunFile("Assets/Scripts/BaryCentre.py");
        }
    }

    /// <summary>
    /// used for converting barycentre caluculated in python back to C#
    /// </summary>
    /// <returns></returns>
    public Gesture Python2CSharp()
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
        pythonResult.Clear();
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

    /// <summary>
    /// calculates the pairwise similairity between any two gestures
    /// </summary>
    /// <param name="g1"></param>
    /// <param name="g2"></param>
    /// <returns></returns>
   public float DTW_GestureWise(Gesture g1, Gesture g2)
    {
        List<Gesture> temp = new List<Gesture> { g1, g2 };
        CSharp2Python(temp);

        PythonRunner.RunFile("Assets/Scripts/DTW_GestureWise.py");
        return similarityScore;
    }

    public void CalculateGlobalConsensus()
    {
        float sum = 0;
        foreach (Gesture g in gestures)
        {
            float newSimilarity = DTW_GestureWise(g, averageGesture);
            g.SetGlobalSimilarity(newSimilarity);
            sum += Mathf.Pow(newSimilarity, 2);
        }
        globalConsensus = sum / gestures.Count;
    }

    public void CalculateClusterSimilarity()
    {
        //float sum = 0;
        foreach (KeyValuePair<int, Cluster> pair in clusters)
        {
            pair.Value.SetSimilarity(
            DTW_GestureWise(
            pair.Value.GetBaryCentre(),
            averageGesture));
            //sum += Mathf.Pow(pair.Value.GetSimilarity(),2);
        }
        //Debug.Log(sum / gestures.Count);
    }

    public Gesture GetGlobalAverageGesture()
    {
        return averageGesture;
    }

    public float GetGlobalConsensus()
    {
        return globalConsensus;
    }

    /// <summary>
    /// return a list of int representing clusters by their similarity to the entire dataset.
    /// </summary>
    /// <returns></returns>
    public List<int> Sort()
    {
        List<Cluster> temp = new List<Cluster>();
        foreach(KeyValuePair<int, Cluster> pair in clusters)
        {
            if (GestureVisualizer.instance.clustersObjDic[pair.Key].gameObject != null)
            { temp.Add(pair.Value); }
        }
        List<int> result = new List<int>();

        while (temp.Count > 0)
        {
            Cluster cluster = null;
            float min = float.PositiveInfinity;
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].GetSimilarity() < min)
                {
                    min = temp[i].GetSimilarity();
                    cluster = temp[i]; 
                }
            }
            result.Add(cluster.clusterID);
            temp.Remove(cluster);
        }
        return result;
    }

    public List<Gesture> PrepareGestureForSearch()
    {
        List<Gesture> results = new List<Gesture>();
        foreach(Gesture g in gestures)
        {
            if(GestureVisualizer.instance.clustersObjDic[g.cluster].transform.Find(g.gestureType + g.id.ToString() + "-Trial" + g.trial.ToString()).gameObject.activeSelf == true) {
                Gesture tempG = new Gesture();
                tempG.num_of_poses = g.num_of_poses;
                tempG.id = g.id;
                tempG.trial = g.trial;
                tempG.cluster = g.cluster;
                foreach(Pose p in g.poses)
                {
                    Pose temp_p = new Pose();
                    temp_p.timestamp = p.timestamp;
                    List<Joint> leftJoints = p.joints.GetRange(4, 3);
                    List<Joint> rightJoints = p.joints.GetRange(8, 3);
                    leftJoints.InsertRange(3, rightJoints);
                    temp_p.joints = leftJoints;
                    temp_p.num_of_joints = 6;
                    tempG.poses.Add(temp_p);
                }
                results.Add(tempG);
            }
        }
        return results;
    }

    public void PCA_Arrangement()
    {
        List<Gesture> tempGesLi = new List<Gesture>();
        foreach (Gesture g in gestures) {
            tempGesLi.Add(g.Resample(10));
        }
        // get centroids
        int centroidCount = clusters.Count;
        for (int i =0;i<centroidCount;i++)
        {
            Gesture centroid = clusters[i].GetBaryCentre();
            tempGesLi.Add(centroid.Resample(10));
        }
        CSharp2Python(tempGesLi); 
        PythonRunner.RunFile("Assets/Scripts/PCA.py");

        int gestureCount = gestures.Count;
        
        for (int i =0; i< gestureCount; i++)
        {
            gestures[i].PCA_Coordinate = new Vector2(pythonResult[i][0], pythonResult[i][1]);
        }
        for (int i = 0; i < centroidCount; i++)
        {
            Gesture centroid = clusters[i].GetBaryCentre();
            centroid.PCA_Coordinate = new Vector2(pythonResult[i + gestureCount][0], pythonResult[i + gestureCount][1]);
            clusters[i].SetBaryCentre(centroid);
        }
    }
}
