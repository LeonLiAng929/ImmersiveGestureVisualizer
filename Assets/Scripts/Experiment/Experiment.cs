using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Scripting.Python;
using UnityEngine;
using System.Linq;
using System.IO;

public class Experiment : MonoBehaviour
{
    /// <summary>
    /// The expriment aims to find out which conbination of clustering methods and rationales makes more sense.
    /// Compare methods and rationales raised in our paper with the original method (K-means + DBA) in GestureMap
    /// 
    /// 
    /// /// Steps of the Expriment:
    /// 1. Load gesture data for all referents (15 referents in total)
    /// 2. Perform the following comparisons
    /// 
    /// Pairs of Comparisons: 
    /// 1. K-means+DBA (k=15) -> mean-shift + Raw (k=i) -----if i!=k---> K-means+DBA(k=i)
    /// 2.                    -> mean-shift + PCA (k=j) -----if j!=k---> K-means+DBA(k=j)
    /// 3.                    -> mean-shift + MDS (k=l) -----if l!=k---> K-means+DBA(k=l)
    /// 4. K-means+PCA (k=15) -> mean-shift + Raw (k=i) -----if i!=k---> K-means+PCA(k=i)
    /// 5.                    -> mean-shift + PCA (k=j) -----if j!=k---> K-means+PCA(k=j)
    /// 6.                    -> mean-shift + MDS (k=l) -----if l!=k---> K-means+PCA(k=l)
    /// 7. K-means+MDS (k=15) -> mean-shift + Raw (k=i) -----if i!=k---> K-means+MDS(k=i)
    /// 8.                    -> mean-shift + PCA (k=j) -----if j!=k---> K-means+MDS(k=j)
    /// 9.                    -> mean-shift + MDS (k=l) -----if l!=k---> K-means+MDS(k=l)
    /// 10. Repeat step 1-9 ten times. As the clustering methods are non-deterministic
    /// 
    /// For each clustering above, we record 
    /// 1. the consensus for each resulting group
    /// 2. how many referents each cluster contains
    /// 3. how many gestures belongs to each referent presented in each cluster
    /// 
    /// To calculate the within-cluster consensus, we simplified the consensus equation of the Dissimilarity-Consensus approach for
    /// Non-repeated Elicitation. The original function is Cr = (Σ(N,i=1),Σ(N,j=i+1)[∆(gi,gj) <= τ])/(0.5N(N-1)), where N denotes the 
    /// number of gestures in the cluster, τ denotes the tolerance value, ∆ denotes a similarity function such as DTW.
    /// In our simplified approach, to determine whether 2 gestures are similar, we compare the referent labels of each pair of gestures
    /// instead of comparing the similarity score with the tolerance. 
    /// The new formula becomes: Cr = (Σ(N,i=1),Σ(N,j=i+1)[E(L(gi),L(gj))])/(0.5N(N-1)), where L denotes the label of a gesture, E represents 
    /// a function that takes in labels of 2 gestures and returns 1 if the labels are the same, 0 if they are not. 
    /// 
    /// </summary>

    [HideInInspector]
    public int k;  // k for k-means clustering
    [HideInInspector]
    public List<List<List<float>>> pythonArguments = new List<List<List<float>>>();
    [HideInInspector]
    public List<List<float>> pythonResult = new List<List<float>>();
    [HideInInspector]
    public float similarityScore = 0;
    private Dictionary<int, Cluster> clusters = new Dictionary<int, Cluster>();
    private IO xmlLoader = new IO();
    [HideInInspector]
    public List<Gesture> gestures = new List<Gesture>();
    public int repetition = 0;

    #region Singleton
    public static Experiment instance;
    #endregion
    public class Record
    {
        /// <summary>
        /// Each record contains summarised info about a cluster.
        /// </summary>
        public float consensus;
        public List<Tuple<string,int>> referents = new List<Tuple<string, int>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadAllData();
        PCA_Arrangement();
        MDS_Arrangement();

        repetition = 1;
        LaunchExperiment();
        Debug.Log("Experiment Finished!");
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void LaunchExperiment()
    {
        int rep = repetition;
        const int startingNumberOfCluster = 15;
        int resultingNumberOfClusters = -1;
        while (rep != 0)
        {
            rep -= 1;
            // 1. K-means+DBA (k=15) -> mean-shift + Raw (k=i) -----if i!=k---> K-means+DBA(k=i)
            InitializeClusters_DBA(startingNumberOfCluster);
            RegisterOutput(CollectClusteringOutputs(), "K-means", "DBA", startingNumberOfCluster);

            clusters.Clear();
            InitializeClusters_Raw_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "Raw", resultingNumberOfClusters);
            if(resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_DBA(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "DBA", resultingNumberOfClusters);
            }

            // 2.                    -> mean-shift + PCA (k=j) -----if j!=k---> K-means+DBA(k=j)
            clusters.Clear();
            InitializeClusters_PCA_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "PCA", resultingNumberOfClusters);
            if (resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_DBA(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "DBA", resultingNumberOfClusters);
            }

            // 3.                    -> mean-shift + MDS (k=l) -----if l!=k---> K-means+DBA(k=l)
            clusters.Clear();
            InitializeClusters_MDS_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "MDS", resultingNumberOfClusters);
            if (resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_DBA(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "DBA", resultingNumberOfClusters);
            }

            // 4. K-means+PCA (k=15) -> mean-shift + Raw (k=i) -----if i!=k---> K-means+PCA(k=i)
            InitializeClusters_PCA_Kmeans(startingNumberOfCluster);
            RegisterOutput(CollectClusteringOutputs(), "K-means", "PCA", startingNumberOfCluster);

            clusters.Clear();
            InitializeClusters_Raw_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "Raw", resultingNumberOfClusters);
            if (resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_PCA_Kmeans(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "PCA", resultingNumberOfClusters);
            }

            // 5.                    -> mean-shift + PCA (k=j) -----if j!=k---> K-means+PCA(k=j)
            clusters.Clear();
            InitializeClusters_PCA_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "PCA", resultingNumberOfClusters);
            if (resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_PCA_Kmeans(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "PCA", resultingNumberOfClusters);
            }

            // 6.                    -> mean-shift + MDS (k=l) -----if l!=k---> K-means+PCA(k=l)
            clusters.Clear();
            InitializeClusters_MDS_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "MDS", resultingNumberOfClusters);
            if (resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_PCA_Kmeans(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "PCA", resultingNumberOfClusters);
            }

            // 7. K-means+MDS (k=15) -> mean-shift + Raw (k=i) -----if i!=k---> K-means+MDS(k=i)
            InitializeClusters_MDS_Kmeans(startingNumberOfCluster);
            RegisterOutput(CollectClusteringOutputs(), "K-means", "MDS", startingNumberOfCluster);

            clusters.Clear();
            InitializeClusters_Raw_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "Raw", resultingNumberOfClusters);
            if (resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_MDS_Kmeans(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "MDS", resultingNumberOfClusters);
            }

            // 8.                    -> mean-shift + PCA (k=j) -----if j!=k---> K-means+MDS(k=j)
            clusters.Clear();
            InitializeClusters_PCA_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "PCA", resultingNumberOfClusters);
            if (resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_MDS_Kmeans(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "MDS", resultingNumberOfClusters);
            }

            // 9.                    -> mean-shift + MDS (k=l) -----if l!=k---> K-means+MDS(k=l)
            clusters.Clear();
            InitializeClusters_MDS_MeanShift();
            resultingNumberOfClusters = clusters.Count;
            RegisterOutput(CollectClusteringOutputs(), "MeanShit", "MDS", resultingNumberOfClusters);
            if (resultingNumberOfClusters != startingNumberOfCluster)
            {
                clusters.Clear();
                InitializeClusters_MDS_Kmeans(resultingNumberOfClusters);
                RegisterOutput(CollectClusteringOutputs(), "K-means", "MDS", resultingNumberOfClusters);
            }
        }
    }

    public List<Record> CollectClusteringOutputs()
    {
        List<Record> records = new List<Record>();
        foreach (KeyValuePair<int, Cluster> pair in clusters)
        {
            Record r = WithinClusterDataProcessing(pair.Value);
            records.Add(r);
        }
        var output = records.OrderByDescending(x => x.consensus).ToList();
        return output;
    }
    public void RegisterOutput(List<Record> output, string method, string rationale, int k)
    {
        string filename = Application.dataPath + "/Scripts/Experiment/ExperimentResult.csv";
        if (!File.Exists(filename))
        {
            TextWriter tww = new StreamWriter(filename, false);

            string header = "Round#,Method,Rationale,k,ClusterConsensus,ClusterDetail";

            tww.WriteLine(header);
            tww.Close();
        }

        string content = "";
        content += repetition.ToString();
        content += ",";
        content += method;
        content += ",";
        content += rationale;
        content += ",";
        content += k.ToString();

        foreach (Record r in output)
        {
            string cell = ",";
            cell += r.consensus.ToString();
            string tuples = "";
            foreach (Tuple<string,int> t in r.referents)
            {
                tuples += t.ToString() + ",";
            }
            cell += "," + string.Format("{0}", "\"" + tuples + "\"");
            content += cell;
        }
        TextWriter tw = new StreamWriter(filename, true);

        tw.WriteLine(content);
        tw.Close();
    }
    public void LoadAllData()
    {
        gestures = xmlLoader.LoadAllXML();

        foreach (Gesture g in gestures)
        {
            g.SetBoundingBox();
            g.SetCentroid();
            g.TranslateToOrigin();
            g.NormalizeHeight();
        }
    }
 
    public Record WithinClusterDataProcessing(Cluster c)
    {
        List<Gesture>gestures = c.GetGestures();
        if (gestures.Count > 1)
        {
            Dictionary<string, int> count = new Dictionary<string, int>(); // count of gestures in each refent presented in this cluster
            int numOfGestures = c.GestureCount();
            int similarPairs = 0;
            for (int i = 0; i < numOfGestures - 1; i++)
            {
                string key = gestures[i].gestureType;
                if (count.ContainsKey(key))
                {
                    count[key] += 1;
                }
                else
                {
                    count.Add(key, 1);
                }
                for (int j = i + 1; j < numOfGestures; j++)
                {
                    if (gestures[i].gestureType == gestures[j].gestureType)
                    {
                        similarPairs += 1;
                    }
                }
            }
            string keyy = gestures[numOfGestures - 1].gestureType;
            if (count.ContainsKey(keyy))
            {
                count[keyy] += 1;
            }
            else
            {
                count.Add(keyy, 1);
            }
            float consensus = similarPairs / (0.5f * numOfGestures * (numOfGestures - 1));
            Record r = new Record();
            r.consensus = consensus;
            List<Tuple<string, int>> referents = new List<Tuple<string, int>>();
            foreach (KeyValuePair<string, int> pair in count)
            {
                referents.Add(new Tuple<string, int>(pair.Key, pair.Value));
            }
            var result = referents.OrderByDescending(x => x.Item2).ToList();
            r.referents = result;
            return r;
        }
        else
        {
            Record r = new Record();
            r.consensus = -1;
            var li = new List<Tuple<string, int>>();
            li.Add(new Tuple<string, int>(gestures[0].gestureType, 1));
            r.referents = new List<Tuple<string, int>>();
            return r;
        }
    }
    public void InitializeClusters_DBA(int _k)
    {
        k = _k;
        //clustering
        if (k == 1)
        {
            foreach (Gesture g in gestures)
            {
                g.cluster = 0;
            }
        }
        else
        {
            PythonRunner.RunFile("Assets/Scripts/K_MeanClustering.py");
        }

        // initialize clusters
        foreach (Gesture g in gestures)
        {
            Cluster temp = TryGetCluster(g.cluster);
            List<Gesture> gli = new List<Gesture>();
            gli.Add(g);
            temp.AddGesture(gli, true);
        }

        /*// calculate barycenter for each cluster

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
        PCA_Arrangement();
        MDS_Arrangement();*/
    }

    public void InitializeClusters_PCA_Kmeans(int _k)
    {
        k = _k;
        //PCA_Arrangement();
        //clustering
        if (k == 1)
        {
            foreach (Gesture g in gestures)
            {
                g.cluster = 0;
            }
        }
        else
        {
            PythonRunner.RunFile("Assets/Scripts/KmeansPCA.py");
        }
        /*List<List<float>> clusterCentres = new List<List<float>>();
        foreach (List<float> coordinate in pythonResult)
        {
            clusterCentres.Add(coordinate);
        }*/

        // initialize clusters
        foreach (Gesture g in gestures)
        {
            Cluster temp = TryGetCluster(g.cluster);
            List<Gesture> gli = new List<Gesture>();
            gli.Add(g);
            temp.AddGesture(gli, true);
        }

        /*// calculate barycenter for each cluster

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
        MDS_Arrangement();*/
    }

    public void InitializeClusters_MDS_Kmeans(int _k)
    {
        k = _k;
        //MDS_Arrangement();
        //clustering
        if (k == 1)
        {
            foreach (Gesture g in gestures)
            {
                g.cluster = 0;
            }
        }
        else
        {
            PythonRunner.RunFile("Assets/Scripts/KmeansMDS.py");
        }
        /*List<List<float>> clusterCentres = new List<List<float>>();
        foreach (List<float> coordinate in pythonResult)
        {
            clusterCentres.Add(coordinate);
        }
        */
        // initialize clusters
        foreach (Gesture g in gestures)
        {
            Cluster temp = TryGetCluster(g.cluster);
            List<Gesture> gli = new List<Gesture>();
            gli.Add(g);
            temp.AddGesture(gli, true);
        }

        /*// calculate barycenter for each cluster

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
            pair.Value.SetBaryCentreMDS_Coord(new Vector2(clusterCentres[pair.Key][0], clusterCentres[pair.Key][1]));
        }
        PCA_Arrangement();*/
    }

    public void InitializeClusters_MDS_MeanShift()
    {
        //MDS_Arrangement();
        //clustering

        PythonRunner.RunFile("Assets/Scripts/MeanShiftMDS.py");
        /*(List<List<float>> clusterCentres = new List<List<float>>();
        foreach (List<float> coordinate in pythonResult)
        {
            clusterCentres.Add(coordinate);
        }*/

        // initialize clusters
        foreach (Gesture g in gestures)
        {
            Cluster temp = TryGetCluster(g.cluster);
            List<Gesture> gli = new List<Gesture>();
            gli.Add(g);
            temp.AddGesture(gli, true);
        }

        /*// calculate barycenter for each cluster

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
            pair.Value.SetBaryCentreMDS_Coord(new Vector2(clusterCentres[pair.Key][0], clusterCentres[pair.Key][1]));
        }
        PCA_Arrangement();
        GestureVisualizer.instance.k = k;*/
    }

    public void InitializeClusters_PCA_MeanShift()
    {
        //PCA_Arrangement();
        //clustering

        PythonRunner.RunFile("Assets/Scripts/MeanShiftPCA.py");
        /*List<List<float>> clusterCentres = new List<List<float>>();
        foreach (List<float> coordinate in pythonResult)
        {
            clusterCentres.Add(coordinate);
        }
        */
        // initialize clusters
        foreach (Gesture g in gestures)
        {
            Cluster temp = TryGetCluster(g.cluster);
            List<Gesture> gli = new List<Gesture>();
            gli.Add(g);
            temp.AddGesture(gli, true);
        }

        /*// calculate barycenter for each cluster

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
            pair.Value.SetBaryCentreMDS_Coord(new Vector2(clusterCentres[pair.Key][0], clusterCentres[pair.Key][1]));
        }
        MDS_Arrangement();
        GestureVisualizer.instance.k = k;*/
    }

    public void InitializeClusters_Raw_MeanShift()
    {

        //clustering
        List<Gesture> g = new List<Gesture>();
        foreach (Gesture a in gestures)
        {
            g.Add(a.Resample(10));
        }
        CSharp2Python(g);

        PythonRunner.RunFile("Assets/Scripts/MeanShiftNormalisedRaw.py");
        /*List<List<float>> clusterCentres = new List<List<float>>();
        foreach (List<float> coordinate in pythonResult)
        {
            clusterCentres.Add(coordinate);
        }*/

        // initialize clusters
        foreach (Gesture ges in gestures)
        {
            Cluster temp = TryGetCluster(ges.cluster);
            List<Gesture> gli = new List<Gesture>();
            gli.Add(ges);
            temp.AddGesture(gli, true);
        }

        /*// calculate barycenter for each cluster

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
            pair.Value.SetBaryCentreMDS_Coord(new Vector2(clusterCentres[pair.Key][0], clusterCentres[pair.Key][1]));
        }
        PCA_Arrangement();
        MDS_Arrangement();
        GestureVisualizer.instance.k = k;*/
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

    public void RemoveClusterByID(int id)
    {
        clusters.Remove(id);
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

    public Dictionary<int, Cluster> GetClusters()
    {
        return clusters;
    }

    public void CSharp2Python(List<Gesture> gList)
    {
        pythonArguments.Clear();
        foreach (Gesture g in gList)
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
        for (int i = 0; i < temp.num_of_poses; i++)
        {
            Pose pose = new Pose();
            pose.num_of_joints = 20;
            pose.timestamp = timestamp + i * 100;
            for (int j = 0; j < pose.num_of_joints * 3; j += 3)
            {
                float x = pythonResult[i][j];
                float y = pythonResult[i][j + 1];
                float z = pythonResult[i][j + 2];
                Joint joint = new Joint(x, y, z, gestures[0].poses[0].joints[j / 3].jointType);
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
            for (int j = 0; j < y; j++)
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

    public void PCA_Arrangement()
    {
        List<Gesture> tempGesLi = new List<Gesture>();
        foreach (Gesture g in gestures)
        {
            tempGesLi.Add(g.Resample(10));
        }
        /*// get centroids
        List<int> freeIDs = GestureVisualizer.instance.freeId;
        int centroidCount = clusters.Count + freeIDs.Count;
        for (int i = 0; i < centroidCount; i++)
        {
            if (!freeIDs.Contains(i))
            {
                Gesture centroid = clusters[i].GetBaryCentre();
                if (centroid != null)
                    tempGesLi.Add(centroid.Resample(10));
            }
        }*/
        CSharp2Python(tempGesLi);
        PythonRunner.RunFile("Assets/Scripts/PCA.py");

        int gestureCount = gestures.Count;

        for (int i = 0; i < gestureCount; i++)
        {
            gestures[i].PCA_Coordinate = new Vector2(pythonResult[i][0], pythonResult[i][1]);
        }
        /*int index = 0;
        for (int i = 0; i < centroidCount; i++)
        {
            if (!freeIDs.Contains(i))
            {
                Gesture centroid = clusters[i].GetBaryCentre();
                if (centroid != null)
                {
                    centroid.PCA_Coordinate = new Vector2(pythonResult[index + gestureCount][0], pythonResult[index + gestureCount][1]);
                    clusters[i].SetBaryCentre(centroid);
                    index += 1;
                }
            }
        }*/
    }

    public void MDS_Arrangement()
    {
        List<Gesture> tempGesLi = new List<Gesture>();
        foreach (Gesture g in gestures)
        {
            tempGesLi.Add(g.Resample(10));
        }
        /*// get centroids
        List<int> freeIDs = GestureVisualizer.instance.freeId;
        int centroidCount = clusters.Count + freeIDs.Count;
        for (int i = 0; i < centroidCount; i++)
        {
            if (!freeIDs.Contains(i))
            {
                Gesture centroid = clusters[i].GetBaryCentre();
                if (centroid != null)
                    tempGesLi.Add(centroid.Resample(10));
            }
        }*/
        CSharp2Python(tempGesLi);
        PythonRunner.RunFile("Assets/Scripts/MDS.py");

        int gestureCount = gestures.Count;

        for (int i = 0; i < gestureCount; i++)
        {
            gestures[i].MDS_Coordinate = new Vector2(pythonResult[i][0], pythonResult[i][1]);
        }
        /*int index = 0;
        for (int i = 0; i < centroidCount; i++)
        {
            if (!freeIDs.Contains(i))
            {
                Gesture centroid = clusters[i].GetBaryCentre();
                if (centroid != null)
                {
                    centroid.MDS_Coordinate = new Vector2(pythonResult[index + gestureCount][0], pythonResult[index + gestureCount][1]);
                    clusters[i].SetBaryCentre(centroid);
                    index += 1;
                }
            }
        }*/
    }
}
