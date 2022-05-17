using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;



public class GestureVisualizer : MonoBehaviour
{
    // Trajectory visualization
    [SerializeField]
    public GameObject trajectoryRendererPrefab;
    //[SerializeField]
    //protected GameObject tracerRef;
    [SerializeField]
    public GameObject trajectoryPrefab;
    [SerializeField]
    public GameObject gesVisPrefab;
    [SerializeField]
    public GameObject skeletonModel;
    [SerializeField]
    public GameObject clusterVisPrefab;
    [SerializeField]
    public Material tubeRendererMat;
    [SerializeField]
    public GameObject TrajectoryFilterGameObject;
    [SerializeField]
    public Transform wristRight;
    [SerializeField]
    public Transform elbowRight;
    [SerializeField]
    public Transform shoulderRight;
    [SerializeField]
    public Transform wristLeft;
    [SerializeField]
    public Transform elbowLeft;
    [SerializeField]
    public Transform shoulderLeft;
    [SerializeField]
    public GameObject proposedGestureObj; // used for visualizing user proposed gesture when using the search feature.

    [HideInInspector]
    public Dictionary<int, GameObject> clustersObjDic = new Dictionary<int, GameObject>();
    [HideInInspector]
    public List<Color> trajectoryColorSet = new List<Color>();
    private Dictionary<int, Color> clusterColorDic = new Dictionary<int, Color>();
    [HideInInspector]
    public int arrangementMode = 3; // 0 = local, 1 = global, 2 = line-up, 3=PCA, 4=MDS
    public InputDevice rightController;
    public InputDevice leftController;
    [HideInInspector]
    public List<GameObject> stackedObjects = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> selectedGestures = new List<GameObject>();
    [HideInInspector]
    public List<int> freeId = new List<int>();

    // for close Comparison
    [HideInInspector]
    public bool leftHandSelected = false;
    [HideInInspector]
    public bool rightHandSelected = false;

    // for search
    [HideInInspector]
    public float time = 0;
    [HideInInspector]
    public Gesture proposedGes;
    [HideInInspector]
    public List<GestureGameObject> searchResult = new List<GestureGameObject>();
    [HideInInspector]
    public bool adjustTranform = false;


    // for exocentric view and 2D penal
    [SerializeField]
    public GameObject GesUiPrefab;
    [SerializeField]
    public GameObject UiPenalPrefab;
    [SerializeField]
    public Transform UiParent;
    [HideInInspector]
    public List<GameObject> uiRefList = new List<GameObject>();
    [HideInInspector]
    public int k = 1;
    [HideInInspector]
    public ClusteringRationales clusteringRationale = ClusteringRationales.DBA; // 0:DBA, 1:PCA, 2:MDS
    [HideInInspector]
    public ClusteringMethods clusteringMethod = ClusteringMethods.K_Means;
    [HideInInspector]
    public bool startup = true;
    private Dictionary<string, int> uiLinkDic = new Dictionary<string, int>();
    [HideInInspector]
    public List<GameObject> boardRecords = new List<GameObject>();
    [HideInInspector]
    public GameObject currBoard = null;
    [HideInInspector]
    public List<GestureGameObject> gestureGameObjs = new List<GestureGameObject>();



    #region Singleton
    public static GestureVisualizer instance;
    #endregion

    private void Awake()
    {
        instance = this;
        proposedGes = new Gesture();
        
    }

    public Color GetColorByCluster(int i)
    {
        return clusterColorDic[i];
    }
    private void Start()
    {
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        if (rightHandDevices.Count == 1)
        {
            rightController = rightHandDevices[0];
        }
        else
        {
            Debug.LogError("You have zero or more than one right controller connected! Make sure you have exactly one right controller connected.");
        }
        if (leftHandDevices.Count == 1)
        {
            leftController = leftHandDevices[0];
        }
        //InitializeClusterColor();

        //Generate trajectory colors and assign them to trajectory filter
        TrajectoryFilter[] filters = TrajectoryFilterGameObject.GetComponentsInChildren<TrajectoryFilter>(true);
        for (int j = 0; j < GestureAnalyser.instance.GetGestures()[0].poses[0].num_of_joints; j++)
        {
            Color c = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            trajectoryColorSet.Add(c);
            filters[j].gameObject.GetComponent<MeshRenderer>().material.color = c;
            filters[j].init = c;
        }


        // initialize colors for user proposed trajectories
        proposedGestureObj.transform.Find("WristLeft").GetComponent<MeshRenderer>().material.color = trajectoryColorSet[6];
        proposedGestureObj.transform.Find("ElbowLeft").GetComponent<MeshRenderer>().material.color = trajectoryColorSet[5];
        proposedGestureObj.transform.Find("ShoulderLeft").GetComponent<MeshRenderer>().material.color = trajectoryColorSet[4];
        proposedGestureObj.transform.Find("WristRight").GetComponent<MeshRenderer>().material.color = trajectoryColorSet[10];
        proposedGestureObj.transform.Find("ElbowRight").GetComponent<MeshRenderer>().material.color = trajectoryColorSet[9];
        proposedGestureObj.transform.Find("ShoulderRight").GetComponent<MeshRenderer>().material.color = trajectoryColorSet[8];
        proposedGestureObj.transform.Find("WristLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGestureObj.transform.Find("ElbowLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGestureObj.transform.Find("ShoulderLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGestureObj.transform.Find("WristRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGestureObj.transform.Find("ElbowRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGestureObj.transform.Find("ShoulderRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
        //Deploy.instance._DeployRig();
        //Initialize2DBoard();
        k = 1;
        //arrangementMode = 3;
        //InitializeVisualization();
    }
    public void Initialize2DBoard()
    {
        if(currBoard != null)
        {
            boardRecords.Add(currBoard);
        }
        uiRefList.Clear();
        GameObject temp = Instantiate(UiPenalPrefab, UiParent);
      
        int n = GestureAnalyser.instance.GetGestures().Count;
        for (int i = 0; i < n; i++)
        {
            uiRefList.Add(Instantiate(GesUiPrefab, temp.transform.Find("Contents")));
        }
        for (int i = 0; i < n; i++)
        {
            uiRefList[i].GetComponent<Transform>().localPosition = new Vector3((i % 8) * 0.1f, -(i / 8) * 0.1f, 0);
        }
        temp.transform.Find("Grab").localPosition = uiRefList[0].GetComponent<Transform>().localPosition - new Vector3(0.1f,0,0);
        GameObject grab = temp.transform.Find("Grab").gameObject;
        grab.GetComponent<BoardControl>().boardInfo.text = "Method: "+clusteringMethod.ToString() + "\nK=" + k.ToString() + "|Rationale:" + clusteringRationale.ToString();
        grab.GetComponent<BoardControl>().boardInfo.gameObject.SetActive(false);
        grab.GetComponent<MeshRenderer>().material.color = clusterColorDic[0];
        grab.GetComponent<BoardSnap>().SetBoundingBox();
        grab.GetComponent<BoardSnap>().ApplyBackground();
        temp.transform.localPosition += temp.transform.forward;
        currBoard = temp;

    }
    public void InitLink2DBoard()
    {
        gestureGameObjs.Clear();
        uiLinkDic.Clear();
        foreach(KeyValuePair<int, GameObject> pair in clustersObjDic)
        {
            foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
            {
                if (!gGO.averageGesture)
                {
                    gestureGameObjs.Add(gGO);
                }
            }
        }
        for(int i = 0; i < uiRefList.Count; i++)
        {
            Gesture2DObject uiRef = uiRefList[i].GetComponent<Gesture2DObject>();
            uiRef.gGO = gestureGameObjs[i];
            uiRef.Initialize2DGesture();
            uiLinkDic.Add(gestureGameObjs[i].gesture.id.ToString() + '-' + gestureGameObjs[i].gesture.trial, i);
            uiRefList[i].GetComponent<MeshRenderer>().material.color = GetColorByCluster(gestureGameObjs[i].gesture.cluster);
            gestureGameObjs[i].uiRef = uiRefList[i].GetComponent<Gesture2DObject>();
            uiRefList[i].GetComponent<Gesture2DObject>().GesInfo.text = gestureGameObjs[i].gesture.id.ToString() + "-" + gestureGameObjs[i].gesture.trial;
        }
        startup = false;
    }

    /// <summary>
    /// link the current board to the gestures
    /// </summary>
    public void Link2DBoard()
    {
        gestureGameObjs.Clear();
        foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
        {
            foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
            {
                if (!gGO.averageGesture)
                {
                    gestureGameObjs.Add(gGO);
                }
            }
        }
        for (int i = 0; i < uiRefList.Count; i++)
        {
            string key = gestureGameObjs[i].gesture.id.ToString() + '-' + gestureGameObjs[i].gesture.trial;
            Gesture2DObject uiref = uiRefList[uiLinkDic[key]].GetComponent<Gesture2DObject>();
            uiref.gGO = gestureGameObjs[i];
            uiref.gameObject.GetComponent<MeshRenderer>().material.color = GetColorByCluster(gestureGameObjs[i].gesture.cluster);
            gestureGameObjs[i].uiRef = uiref;
            uiref.Initialize2DGesture();
            uiref.GesInfo.text = gestureGameObjs[i].gesture.id.ToString() + "-" + gestureGameObjs[i].gesture.trial;
        }
        // re-link gestures to old boards
        foreach (GameObject board in boardRecords)
        {
            Gesture2DObject[] oldUiRefList = board.GetComponentsInChildren<Gesture2DObject>(true);
            for (int i = 0; i < uiRefList.Count; i++)
            {
                string key = gestureGameObjs[i].gesture.id.ToString() + '-' + gestureGameObjs[i].gesture.trial;
                Gesture2DObject uiref = oldUiRefList[uiLinkDic[key]];
                uiref.gGO = gestureGameObjs[i];
                //uiref.GesInfo.text = gestureGameObjects[i].gesture.id.ToString() + "-" + gestureGameObjects[i].gesture.trial;
            }
        }

    }

    public void oldBoardIndicatorUpdate(Gesture2DObject reference)
    {
        string key = reference.gGO.gesture.id.ToString() + '-' + reference.gGO.gesture.trial;
        foreach (GameObject board in boardRecords)
        {
            Gesture2DObject[] oldUiRefList = board.GetComponentsInChildren<Gesture2DObject>(true);
            Gesture2DObject uiref = oldUiRefList[uiLinkDic[key]];
            if(uiref.AnimationIndicator.activeSelf != reference.AnimationIndicator.activeSelf)
                uiref.AnimationIndicator.SetActive(reference.AnimationIndicator.activeSelf);
            if (uiref.ChangingClusterIndicator.activeSelf != reference.ChangingClusterIndicator.activeSelf)
                uiref.ChangingClusterIndicator.SetActive(reference.ChangingClusterIndicator.activeSelf);
            if (uiref.ComparisonIndicator.activeSelf != reference.ComparisonIndicator.activeSelf)
                uiref.ComparisonIndicator.SetActive(reference.ComparisonIndicator.activeSelf);
            if (uiref.HeatmapIndicator.activeSelf != reference.HeatmapIndicator.activeSelf)
                uiref.HeatmapIndicator.SetActive(reference.HeatmapIndicator.activeSelf);
            if (uiref.SlidimationIndicator.activeSelf != reference.SlidimationIndicator.activeSelf)
                uiref.SlidimationIndicator.SetActive(reference.SlidimationIndicator.activeSelf);
            if (uiref.SmallmultiplesIndicator.activeSelf != reference.SmallmultiplesIndicator.activeSelf)
                uiref.SmallmultiplesIndicator.SetActive(reference.SmallmultiplesIndicator.activeSelf);
            if (uiref.StackingIndicator.activeSelf != reference.StackingIndicator.activeSelf)
                uiref.StackingIndicator.SetActive(reference.StackingIndicator.activeSelf);
            if (uiref.arrow.activeSelf != reference.arrow.activeSelf)
                uiref.arrow.SetActive(reference.arrow.activeSelf);
        }
    }

    public void InitializeClusterColor()
    {
        for (int i = 0; i < k; i++)
        {
            if (!clusterColorDic.ContainsKey(i))
            {
                Color rand = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
                while (clusterColorDic.ContainsValue(rand))
                {
                    rand = Random.ColorHSV(0f, 1f, 1f, 1f, 0.25f, 1f);
                }
                clusterColorDic.Add(i, rand);
            }
        }
    }

    /// <summary>
    /// Used along side with ChangeCluster feature, analysts can instantiate a new cluster of 0 objects and then
    /// use ChangeCluster to add gestures to it. 
    /// </summary>
    public GameObject InstantiateNewCluster()
    {
        int id;
        if(freeId.Count != 0)
        {
            id = freeId[0];
            freeId.RemoveAt(0);
        }
        else{
            id = k;
            IncrementCluster.instance._Increment();
        }

        GestureAnalyser.instance.TryGetCluster(id);
        clusterVisPrefab.SetActive(true);
        GameObject newClusterObj = Instantiate(clusterVisPrefab);
        clusterVisPrefab.SetActive(false);
        newClusterObj.name = "Cluster" + id.ToString();
        Transform clusterTrans = newClusterObj.GetComponent<Transform>().Find("ClusterVisualization"); ;

        clusterTrans.gameObject.AddComponent<ClusterGameObject>();
        clusterTrans.gameObject.GetComponentInChildren<ClusterGameObject>().clusterID = id;
        clusterTrans.gameObject.GetComponentInChildren<ClusterGameObject>().baryCentreVis = null;

        // initialize a random color for each cluster and the gestures belonging to it.
        InitializeClusterColor();
        Color temp = clusterColorDic[id];
        temp.a = (float)0.3;
        clusterTrans.gameObject.GetComponent<MeshRenderer>().material.color = temp;
        if (clustersObjDic.ContainsKey(id))
        {
            clustersObjDic[id] = newClusterObj;
        }
        else
        {
            clustersObjDic.Add(id, newClusterObj);
        }
        
        return clustersObjDic[id];
    }
    public void InitializeVisualization()
    {
        skeletonModel.SetActive(true);
        trajectoryPrefab.SetActive(true);
        gesVisPrefab.SetActive(true);
        GesUiPrefab.SetActive(true);
        UiPenalPrefab.SetActive(true);

        if (clusteringMethod == ClusteringMethods.K_Means)
        {
            if (clusteringRationale == ClusteringRationales.DBA)
                GestureAnalyser.instance.InitializeClusters_DBA(k);
            else if (clusteringRationale == ClusteringRationales.PCA)
                GestureAnalyser.instance.InitializeClusters_PCA_Kmeans(k);
            else if (clusteringRationale == ClusteringRationales.MDS)
                GestureAnalyser.instance.InitializeClusters_MDS_Kmeans(k);
        }
        else if (clusteringMethod == ClusteringMethods.MeanShift) {
            if (clusteringRationale == ClusteringRationales.PCA)
                GestureAnalyser.instance.InitializeClusters_PCA_MeanShift();
            else if (clusteringRationale == ClusteringRationales.MDS)
                GestureAnalyser.instance.InitializeClusters_MDS_MeanShift();
            else if (clusteringRationale == ClusteringRationales.Raw)
                GestureAnalyser.instance.InitializeClusters_Raw_MeanShift();
        }

        List<Gesture> gestures = GestureAnalyser.instance.GetGestures();

        //instantiate Cluster visualizations
        Dictionary<int, Cluster> clusters = GestureAnalyser.instance.GetClusters();
        foreach (KeyValuePair<int, Cluster> pair in clusters)
        {
            GameObject newClusterObj = Instantiate(clusterVisPrefab);
            newClusterObj.name = "Cluster" + pair.Key.ToString();
            Transform clusterTrans = newClusterObj.GetComponent<Transform>().Find("ClusterVisualization"); ;


            clusterTrans.gameObject.AddComponent<ClusterGameObject>();
            clusterTrans.gameObject.GetComponentInChildren<ClusterGameObject>().clusterID = pair.Value.clusterID;

            // initialize a random color for each cluster and the gestures belonging to it.
            InitializeClusterColor();           
            Color temp = clusterColorDic[pair.Key];
            temp.a = (float)0.3;
            clusterTrans.gameObject.GetComponent<MeshRenderer>().material.color = temp;
            clustersObjDic.Add(pair.Key, newClusterObj);
        }

        //float y = 0;

        foreach (Gesture g in gestures)
        {
            // instantiate Gesture visualizations. A gesture visualziation has a trajectory view and a small-multiples view.
            GameObject newGesVis = Instantiate(gesVisPrefab, clustersObjDic[g.cluster].GetComponent<Transform>());
            newGesVis.name = g.gestureType + g.id.ToString() + "-Trial" + g.trial.ToString();
            newGesVis.GetComponent<GestureGameObject>().gesture = g;
            newGesVis.GetComponent<GestureGameObject>().Initialize();
            Transform newGesVisTrans = newGesVis.GetComponent<Transform>();

            InstantiateTrajectory(newGesVis, g);
            UpdateGlowingFieldColour(newGesVis);
            // instantiate small-multiples 
            List<Pose> sampled = g.Resample(5).poses;
            float x = 0.7f;
            int samplePoseIndex = 1;
            foreach (Pose p in sampled)
            {
                GameObject skeleton = Instantiate(skeletonModel, newGesVisTrans.GetComponent<Transform>().Find("SmallMultiples").GetComponent<Transform>());
                skeleton.name = g.gestureType + g.id.ToString() + "-Pose" + samplePoseIndex.ToString();
                samplePoseIndex++;
                Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
                transforms[0].localPosition = new Vector3(0, 0, x);
                x += 0.5f;
                
                for (int i = 1; i < p.num_of_joints + 1; i++)
                {
                    
                    transforms[i].localPosition = p.joints[i - 1].ToVector();
                }
            }
            newGesVisTrans.GetComponent<Transform>().Find("SmallMultiples").gameObject.SetActive(false);
            //y = y + float.Parse("1.5");
        }


        // instantiate barycentre visualization for each cluster
        foreach (KeyValuePair<int, GameObject> p in clustersObjDic)
        {
            InstantiateAverageGestureVis(p.Value, p.Key);
        }

        // arrange initial position for gestures under each cluster
        foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
        {
            pair.Value.SetActive(true);
            foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>())
            {
                gGO.ShowTracer();
                if (!gGO.averageGesture)
                {
                    gGO.gameObject.SetActive(false);
                }
            }
        }

        /*ArrangeLocationForGestures();

        // set size for the clusters in the scene
        foreach (KeyValuePair<int, GameObject> p in clustersObjDic)
        {
            ClusterGameObject a = p.Value.GetComponentInChildren<ClusterGameObject>();
            float count = Mathf.Sqrt(clusters[p.Key].GestureCount());
            a.InitializeClusterVisualizationScale(new Vector3(count, count, count));
        }*/

        //Destroy(tracerRef);
        //Destroy(skeletonModel);
        //Destroy(trajectoryPrefab);
        //Destroy(gesVisPrefab);
        skeletonModel.SetActive(false);
        trajectoryPrefab.SetActive(false);
        gesVisPrefab.SetActive(false);
        GesUiPrefab.SetActive(false);
        UiPenalPrefab.SetActive(false);
        AdjustClusterPosition();

        foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
        {
            List<GestureGameObject> temp = new List<GestureGameObject>(pair.Value.GetComponentsInChildren<GestureGameObject>(true));
            foreach (GestureGameObject gGO in temp)
            {
                if (!gGO.averageGesture)
                {
                    GameObject connection = new GameObject("Connection");
                    connection.transform.parent = gGO.gameObject.transform;
                    connection.AddComponent<ShowConnection>();
                    connection.SetActive(false);
                    //gGO.gameObject.AddComponent<ShowConnection>();
                }
            }
        }
        
        
        /*if (Deploy.instance.IsDeploying())
        {
            foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.clustersObjDic)
            {
                foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
                {
                    gGO.gameObject.transform.Find("Capsule").localRotation = Quaternion.Euler(90, 0, 0);
                    gGO.gameObject.transform.Find("Trajectory").localRotation = Quaternion.Euler(90, 0, 0);
                }
            }
        }*/
        
        if (startup)
        {
            GesUiPrefab.SetActive(true);
            UiPenalPrefab.SetActive(true);
            Initialize2DBoard();
            InitLink2DBoard();
            //Transform a = Instantiate(Ui, Ui.transform.parent)
            GesUiPrefab.SetActive(false);
            UiPenalPrefab.SetActive(false);
        }
        else
        {
            GesUiPrefab.SetActive(true);
            UiPenalPrefab.SetActive(true);
            Initialize2DBoard();
            Link2DBoard();
            GesUiPrefab.SetActive(false);
            UiPenalPrefab.SetActive(false);
        }
    }

    public void DestroyBoardRecord(GameObject board2Delete)
    {
        boardRecords.Remove(board2Delete);
        Destroy(board2Delete);
    }
    public void ShowSearchResult(List<Gesture> result)
    {
        List<GestureGameObject> gestureGameObjects = new List<GestureGameObject>();
        foreach (Gesture g in result) {
            foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
            {
                foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>())
                {
                    if (gGO.gesture.id == g.id && gGO.gesture.trial == g.trial)
                    {
                        gestureGameObjects.Add(gGO);
                    }
                }
            } 
        }

        for (int i = 0; i < gestureGameObjects.Count;i++)
        {
            gestureGameObjects[i].initPos = gestureGameObjects[i].gameObject.transform.localPosition;
            gestureGameObjects[i].gameObject.transform.localPosition = new Vector3(Camera.main.gameObject.transform.position.x + 0.7f*i, gestureGameObjects[i].gameObject.transform.localPosition.y, Camera.main.gameObject.transform.position.z);
            gestureGameObjects[i].gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
            if (i == 0)
            {
                proposedGestureObj.transform.position = gestureGameObjects[i].transform.position;
                proposedGestureObj.transform.localPosition = new Vector3(0,0,0);
                proposedGestureObj.transform.position = proposedGestureObj.transform.position - new Vector3(0.7f, 0, 0);
                proposedGestureObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
                proposedGestureObj.transform.rotation = gestureGameObjects[i].gameObject.transform.rotation;
            }
        }
        searchResult = gestureGameObjects;      
    }
    public void CloseComparison()
    {
        selectedGestures[0].transform.localPosition = new Vector3(Camera.main.gameObject.transform.position.x - 0.25f, selectedGestures[0].transform.localPosition.y, Camera.main.gameObject.transform.position.z);
        selectedGestures[1].transform.localPosition = new Vector3(Camera.main.gameObject.transform.position.x + 0.25f, selectedGestures[1].transform.localPosition.y, Camera.main.gameObject.transform.position.z);
        selectedGestures[0].transform.localRotation = new Quaternion(0, 0, 0, 0);
        selectedGestures[1].transform.localRotation = new Quaternion(0, 0, 0, 0);
        foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
        {
            foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>())
            {
                gGO.gameObject.SetActive(false);
            }
        }
        selectedGestures[0].gameObject.SetActive(true);
        selectedGestures[1].gameObject.SetActive(true);
    }
    public void AdjustClusterPosition()
    {
        if (arrangementMode == 1)
        {
            ArrangeLocationForGestures();
        }
        else if(arrangementMode == 0)
        {
            foreach(KeyValuePair<int,GameObject> pair in clustersObjDic)
            {
                ClusterGameObject cobj = pair.Value.transform.Find("ClusterVisualization").GetComponent<ClusterGameObject>();
                cobj.ArrangeLocationForChildren();
            }
        }
        else if(arrangementMode == 2)
        {
            List<int> rank = GestureAnalyser.instance.Sort();
            int distance = 3;
            foreach (int i in rank)
            {
                // line up clusters
                Transform avgGes = clustersObjDic[i].transform.Find("AverageGesture");
                avgGes.localPosition = new Vector3(distance, avgGes.localPosition.y, 0);
                distance += 4;

                // line up gestures
                clustersObjDic[i].transform.Find("ClusterVisualization").GetComponent<ClusterGameObject>().LineUp();

            }
        }
        else if (arrangementMode == 3)
        {
            foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
            {
                pair.Value.transform.Find("ClusterVisualization").GetComponent<ClusterGameObject>().PCA_Arrangement();
            }
        }
        else if (arrangementMode == 4)
        {
            foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
            {
                pair.Value.transform.Find("ClusterVisualization").GetComponent<ClusterGameObject>().MDS_Arrangement();
            }
        }
        Dictionary<int, Cluster> clusters = GestureAnalyser.instance.GetClusters();

        // set size for the clusters in the scene
        if (arrangementMode == 4 || arrangementMode == 3)
        {
            foreach (KeyValuePair<int, GameObject> p in clustersObjDic)
            {
                if (p.Value != null)
                {
                    ClusterGameObject a = p.Value.GetComponentInChildren<ClusterGameObject>();
                    float count = Mathf.Sqrt(Mathf.Sqrt(clusters[p.Key].GestureCount()));
                    a.InitializeClusterVisualizationScale(new Vector3(count, count, count));
                }
            }
        }
        else
        {
            foreach (KeyValuePair<int, GameObject> p in clustersObjDic)
            {
                if (p.Value != null)
                {
                    ClusterGameObject a = p.Value.GetComponentInChildren<ClusterGameObject>();
                    float count = Mathf.Sqrt(Mathf.Sqrt(clusters[p.Key].GestureCount()));
                    a.InitializeClusterVisualizationScale(new Vector3(count, count, count));
                }
            }
        }
    }
    public void DestroyClusterObjectById(int id)
    {
        Destroy(clustersObjDic[id]);
    }

    public void DestroyAllClusters()
    {
        foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
        {
            GameObject.Destroy(pair.Value);
        }
        
        clustersObjDic.Clear();
        GestureAnalyser.instance.GetClusters().Clear();
    }
    private void Update()
    {
      if (ActionSwitcher.instance.GetCurrentAction() == Actions.ResumeStackedGestures)
        {
            foreach (GameObject obj in stackedObjects)
            {
                obj.transform.Find("Capsule").gameObject.SetActive(true);
                obj.GetComponent<GestureGameObject>().RevertStacking();
                MeshRenderer[] mr = obj.GetComponent<Transform>().Find("Trajectory").Find("Skeleton").GetComponentsInChildren<MeshRenderer>(true);
                foreach (MeshRenderer m in mr)
                {
                    Color temp = m.material.color;
                    temp.a = 1;
                    m.material.color = temp;
                }

                //For old trajectory with linerenderers, also need to change the correspoding section in PrepareStack()
                /*LineRenderer[] traj = obj.GetComponent<Transform>().Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<LineRenderer>();
                foreach (LineRenderer lr in traj)
                {
                    Color temp = lr.endColor;
                    temp.a = 1;
                    lr.startColor = temp;
                    lr.endColor = temp;
                }*/

                // for new trajecotry with tube renderers
                MeshRenderer[] traj = obj.GetComponent<Transform>().Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer lr in traj)
                {
                    Color temp = lr.material.color;
                    temp.a = 1;
                    lr.material.color = temp;
                }

                LineRenderer[] skeltonRd = obj.GetComponent<Transform>().Find("Trajectory").Find("Skeleton").Find("LineRenderers").GetComponentsInChildren<LineRenderer>();
                foreach (LineRenderer lr in skeltonRd)
                {
                    Color temp = lr.endColor;
                    temp.a = 1;
                    lr.startColor = temp;
                    lr.endColor = temp;
                }
            }
            stackedObjects.Clear();
        }
    }

    private void FixedUpdate()
    {
        if (ActionSwitcher.instance.GetCurrentAction() == Actions.Search)
        {
            bool leftGripped;
            bool rightGripped;
            leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out leftGripped);
            rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out rightGripped);
            if (rightGripped && leftGripped && !adjustTranform)
            {
                proposedGestureObj.transform.localPosition = new Vector3(0, 0, 0);
                proposedGestureObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
                Pose p = new Pose();
                time += Time.deltaTime;
                p.timestamp = time;
                p.num_of_joints = 6;
                
                Joint leftShoulderJoint = CreateNewJointByType("ShoulderLeft", shoulderLeft);
                p.joints.Add(leftShoulderJoint);
                Joint leftElbowJoint = CreateNewJointByType("ElbowLeft", elbowLeft);
                p.joints.Add(leftElbowJoint);
                Joint leftWristJoint = CreateNewJointByType("WristLeft", wristLeft);
                p.joints.Add(leftWristJoint);
                Joint rightShoulderJoint = CreateNewJointByType("ShoulderRight", shoulderRight);
                p.joints.Add(rightShoulderJoint);
                Joint rightElbowJoint = CreateNewJointByType("ElbowRight", elbowRight);
                p.joints.Add(rightElbowJoint);
                Joint rightWristJoint = CreateNewJointByType("WristRight", wristRight);
                p.joints.Add(rightWristJoint);
                proposedGes.poses.Add(p);

                UpdateUserProposedTrajectoryByType("ShoulderLeft", p.joints[0].ToVector());
                UpdateUserProposedTrajectoryByType("ElbowLeft", p.joints[1].ToVector());
                UpdateUserProposedTrajectoryByType("WristLeft", p.joints[2].ToVector());
                UpdateUserProposedTrajectoryByType("ShoulderRight", p.joints[3].ToVector());
                UpdateUserProposedTrajectoryByType("ElbowRight", p.joints[4].ToVector());
                UpdateUserProposedTrajectoryByType("WristRight", p.joints[5].ToVector());
            }
        }
    }

    public Joint CreateNewJointByType(string type, Transform trans)
    {
        Joint joint = new Joint();
        joint.jointType = type;
        joint.x = trans.position.x;
        joint.y = trans.position.y;
        joint.z = trans.position.z;
        return joint;
    }

    public void UpdateUserProposedTrajectoryByType(string type, Vector3 joint)
    {
        TubeRenderer tr = proposedGestureObj.transform.Find(type).GetComponent<TubeRenderer>();
        Vector3[] old = tr.points;
        Vector3[] update = new Vector3[] { joint };
        Vector3[] newPoints = new Vector3[old.Length + update.Length];
        old.CopyTo(newPoints, 0);
        update.CopyTo(newPoints, old.Length);
        tr.points = newPoints;
    }
    public void InstantiateAverageGestureVis(GameObject clusterObj, int clusterId)
    {
        GameObject newGesVis = Instantiate(gesVisPrefab, clusterObj.GetComponent<Transform>());  
        newGesVis.SetActive(true);
        newGesVis.GetComponent<GestureGameObject>().averageGesture = true;
        newGesVis.name = "AverageGesture";
        newGesVis.GetComponent<GestureGameObject>().gesture = GestureAnalyser.instance.GetClusterByID(clusterId).GetBaryCentre();
        Transform newGesVisTrans = newGesVis.GetComponent<Transform>();
        ClusterGameObject a = clusterObj.GetComponentInChildren<ClusterGameObject>();
        a.baryCentreVis = newGesVisTrans;

        InstantiateTrajectory(newGesVis, GestureAnalyser.instance.GetClusterByID(a.clusterID).GetBaryCentre());
        UpdateGlowingFieldColour(newGesVis);
        // instantiate small-multiples 
        Gesture g = newGesVis.GetComponent<GestureGameObject>().gesture;
        List<Pose> sampled = g.Resample(5).poses;
        float x = 0.7f;
        int samplePoseIndex = 1;
        foreach (Pose p in sampled)
        {
            GameObject skeleton = Instantiate(skeletonModel, newGesVisTrans.GetComponent<Transform>().Find("SmallMultiples").GetComponent<Transform>());
            skeleton.name = g.gestureType + g.id.ToString() + "-Pose" + samplePoseIndex.ToString();
            samplePoseIndex++;
            Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
            transforms[0].localPosition = new Vector3(0, 0, x);
            x += 0.5f;

            for (int i = 1; i < p.num_of_joints + 1; i++)
            {

                transforms[i].localPosition = p.joints[i - 1].ToVector();
            }
        }
        newGesVisTrans.GetComponent<Transform>().Find("SmallMultiples").gameObject.SetActive(false);
        newGesVis.GetComponent<GestureGameObject>().Initialize();
    }
    public void InstantiateTrajectory(GameObject gestureVis, Gesture g)
    {
        Transform newGesVisTrans = gestureVis.GetComponent<Transform>();

        // instantiate trajectory
        GameObject newTrajObj = Instantiate(trajectoryPrefab, newGesVisTrans);
        newTrajObj.SetActive(true);
        newTrajObj.name = "Trajectory";
    
        newTrajObj.AddComponent<TrajectoryTR>();
        TrajectoryTR traj = newTrajObj.GetComponent<TrajectoryTR>();
        traj.SetAttributes(g, newTrajObj, trajectoryRendererPrefab);
        traj.DrawTrajectory(trajectoryColorSet);

        //instantiate skeletonReference for trajectories
        Transform trajTrans = traj.GetComponent<Transform>();
        GameObject skeleton = Instantiate(skeletonModel, trajTrans);
        skeleton.SetActive(true);
        skeleton.name = "Skeleton";
        foreach (MeshRenderer mr in skeleton.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material.color = clusterColorDic[g.cluster];
        }
        traj.skeletonRef = skeleton;
        Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
        for (int i = 1; i < traj.trajectoryRenderers.Count +1; i++)
        {
            //transforms[i].localPosition = traj.trajectoryRenderers[i - 1].GetPosition(0);
            transforms[i].localPosition = traj.trajectoryRenderers[i - 1].points.ToArray()[0];
        }
    }

    public GameObject GetClusterGameObjectById(int id)
    {
        return clustersObjDic[id];
    }

    public void ArrangeLocationForGestures()
    {
        List<GestureGameObject> gesGameObjLi = new List<GestureGameObject>();
        foreach(KeyValuePair<int, GameObject> pair in clustersObjDic)
        {
            List<GestureGameObject> temp = new List<GestureGameObject>(pair.Value.GetComponentsInChildren<GestureGameObject>(true));
            if(pair.Value.gameObject != null)
                gesGameObjLi = gesGameObjLi.Concat<GestureGameObject>(temp).ToList();
        }
        //InstantiateInCircle(gestures, baryTrans.localPosition);
        InstantiateInCircle(gesGameObjLi, new Vector3(0, 0, 0));
    }

    public void InstantiateInCircle(List<GestureGameObject> gestureObjs, Vector3 location)
    {
        int howMany = gestureObjs.Count;
        float angleSection = Mathf.PI * 2f / howMany;

        for (int i = 0; i < howMany; i++)
        {
            float angle = i * angleSection;
            float radius = gestureObjs[i].gesture.GetGlobalSimilarity();
            Vector3 newPos = location + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius * 2;
            //newPos.y = yPosition;
            //gestureObjs[i].allocatedPos = newPos;
            gestureObjs[i].GetComponent<Transform>().localPosition = newPos;
            gestureObjs[i].GetComponent<Transform>().localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    public void PrepareStack()
    {
        float a = 1.0f;
        a = a / stackedObjects.Count;

        foreach(GameObject obj in stackedObjects)
        {
            obj.transform.Find("Capsule").gameObject.SetActive(false);
            MeshRenderer[] mr = obj.transform.Find("Trajectory").Find("Skeleton").GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer m in mr)
            {
                Color temp = m.material.color;
                temp.a = a;
                m.material.color = temp;
            }

            MeshRenderer[] traj = obj.GetComponent<Transform>().Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer lr in traj)
            {
                Color temp = lr.material.color;
                temp.a = a;
                lr.material.color = temp;
            }
            LineRenderer[] skeltonRd = obj.GetComponent<Transform>().Find("Trajectory").Find("Skeleton").Find("LineRenderers").GetComponentsInChildren<LineRenderer>();
            foreach (LineRenderer lr in skeltonRd)
            {
                Color temp = lr.endColor;
                temp.a = a;
                lr.startColor = temp;
                lr.endColor = temp;
            }
        }
    }

    public Color UpdateGlowingFieldColour(GameObject gesVis)
    {
        int id = gesVis.GetComponent<GestureGameObject>().gesture.cluster;
        gesVis.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = clusterColorDic[id];
        return clusterColorDic[id];
    }

    public void UpdateGestureColour(GameObject gesVis)
    {
        foreach(MeshRenderer mr in gesVis.transform.Find("Trajectory").Find("Skeleton").GetComponentsInChildren<MeshRenderer>())
        {
            mr.material.color = clusterColorDic[gesVis.GetComponent<GestureGameObject>().gesture.cluster];
        }
    }

    public void EmptySelected()
    {
        foreach (GameObject obj in selectedGestures)
        {
            obj.GetComponent<GestureGameObject>().selected = false;
            UpdateGlowingFieldColour(obj);
        }

        selectedGestures.Clear();
    }

    public Dictionary<int, GameObject> GetClusterObjs()
    {
        return clustersObjDic;
    }

}