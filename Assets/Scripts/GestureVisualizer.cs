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
    protected GameObject trajectoryRendererPrefab;
    [SerializeField]
    protected GameObject tracerRef;
    [SerializeField]
    protected GameObject trajectoryPrefab;
    [SerializeField]
    protected GameObject gesVisPrefab;
    [SerializeField]
    protected GameObject skeletonModel;
    [SerializeField]
    protected GameObject clusterVisPrefab;

    private Dictionary<int, GameObject> clustersObjDic = new Dictionary<int, GameObject>();
    private List<Color> trajectoryColorSet = new List<Color>();
    private Dictionary<int, Color> clusterColorDic = new Dictionary<int, Color>();
    public InputDevice rightController;
    public List<GameObject> stackedObjects = new List<GameObject>();

    #region Singleton
    public static GestureVisualizer instance;
    #endregion

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

        if (rightHandDevices.Count == 1)
        {
            rightController = rightHandDevices[0];
        }
        else
        {
            Debug.LogError("You have more than one right controller connected!");
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
            Color rand = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            clusterColorDic.Add(pair.Key, rand);
            Color temp = rand;
            temp.a = (float)0.5;
            clusterTrans.gameObject.GetComponent<MeshRenderer>().material.color = temp;

            clustersObjDic.Add(pair.Key, newClusterObj);
        }

        //float y = 0;

        //Generate trajectory and tracer colors

        for (int j = 0; j < gestures[0].poses[0].num_of_joints; j++)
        {
            Color c = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            trajectoryColorSet.Add(c);
        }

        foreach (Gesture g in gestures)
        {
            // instantiate Gesture visualizations. A gesture visualziation has a trajectory view and a small-multiples view.
            GameObject newGesVis = Instantiate(gesVisPrefab, clustersObjDic[g.cluster].GetComponent<Transform>());
            newGesVis.name = g.gestureType + g.id.ToString();
            newGesVis.GetComponent<GestureGameObject>().gesture = g;
            newGesVis.GetComponent<GestureGameObject>().Initialize();
            Transform newGesVisTrans = newGesVis.GetComponent<Transform>();

            InstantiateTrajectory(newGesVis, g);
            UpdateGlowingFieldColour(newGesVis);

            // instantiate small-multiples 
            List<Pose> sampled = g.Resample(5);
            float x = 1;
            int samplePoseIndex = 1;
            foreach (Pose p in sampled)
            {
                GameObject skeleton = Instantiate(skeletonModel, newGesVisTrans.GetComponent<Transform>().Find("SmallMultiples").GetComponent<Transform>());
                //p.model = skeleton;
                skeleton.name = g.gestureType + g.id.ToString() + "-Pose" + samplePoseIndex.ToString();
                samplePoseIndex++;
                Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
                //Debug.Log(transforms.Length);
                transforms[0].localPosition = new Vector3(0, 0, x);
                x += 1;
                for (int i = 1; i < 21; i++)
                {
                    transforms[i].localPosition = p.joints[i-1].ToVector();
                }
            }
            newGesVisTrans.GetComponent<Transform>().Find("SmallMultiples").gameObject.SetActive(false);
            //y = y + float.Parse("1.5");
        }

  
        // instantiate barycentre visualization for each cluster
        foreach (KeyValuePair<int, GameObject> p in clustersObjDic)
        {
            GameObject newGesVis = Instantiate(gesVisPrefab, p.Value.GetComponent<Transform>());
            newGesVis.name = "AverageGesture";
            newGesVis.GetComponent<GestureGameObject>().gesture = GestureAnalyser.instance.GetClusterByID(p.Key).GetBaryCentre();
            Transform newGesVisTrans = newGesVis.GetComponent<Transform>();
            ClusterGameObject a = p.Value.GetComponentInChildren<ClusterGameObject>();
            a.baryCentreVis = newGesVisTrans;

            InstantiateTrajectory(newGesVis, GestureAnalyser.instance.GetClusterByID(a.clusterID).GetBaryCentre());
            UpdateGlowingFieldColour(newGesVis);
            newGesVis.GetComponent<GestureGameObject>().Initialize();
        }

        // arrange initial position for gestures under each cluster
        foreach (KeyValuePair<int, GameObject> pair in clustersObjDic)
        {
            pair.Value.SetActive(true);
            foreach(GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>())
            {
                gGO.ShowTracer();
                if (gGO.gameObject.name != "AverageGesture")
                {
                    gGO.gameObject.SetActive(false);
                }
            }
        }

        ArrangeLocationForGestures();

        // set size for the clusters in the scene
        foreach (KeyValuePair<int, GameObject> p in clustersObjDic)
        {
            ClusterGameObject a = p.Value.GetComponentInChildren<ClusterGameObject>();
            float count = Mathf.Sqrt(clusters[p.Key].GestureCount());
            a.UpdateClusterVisualization(new Vector3(count, count, count));
        }

        Destroy(tracerRef);
        Destroy(skeletonModel);
        Destroy(trajectoryPrefab);
        Destroy(gesVisPrefab);
    }
    private void Update()
    {
      if (ActionSwitcher.instance.GetCurrentAction() == Actions.ResumeStackedGestures)
        {
            foreach (GameObject obj in stackedObjects)
            {
                obj.GetComponent<GestureGameObject>().RevertStacking();
                MeshRenderer[] mr = obj.GetComponent<Transform>().Find("Trajectory").Find("Skeleton").GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer m in mr)
                {
                    Color temp = m.material.color;
                    temp.a = 1;
                    m.material.color = temp;
                }

                LineRenderer[] traj = obj.GetComponent<Transform>().Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<LineRenderer>();
                foreach (LineRenderer lr in traj)
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

    public void InstantiateTrajectory(GameObject gestureVis, Gesture g)
    {
        Transform newGesVisTrans = gestureVis.GetComponent<Transform>();

        // instantiate trajectory
        GameObject newTrajObj = Instantiate(trajectoryPrefab, newGesVisTrans);
        newTrajObj.name = "Trajectory";
        newTrajObj.AddComponent<Trajectory>();
        Trajectory traj = newTrajObj.GetComponent<Trajectory>();
        traj.SetAttributes(g, newTrajObj, trajectoryRendererPrefab, tracerRef);
        traj.DrawTrajectory(trajectoryColorSet);

        //instantiate skeletonReference for trajectories
        Transform trajTrans = traj.GetComponent<Transform>();
        GameObject skeleton = Instantiate(skeletonModel, trajTrans);
        skeleton.name = "Skeleton";
        foreach (MeshRenderer mr in skeleton.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material.color = clusterColorDic[g.cluster];
        }
        traj.skeletonRef = skeleton;
        Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
        for (int i = 1; i < 21; i++)
        {
            transforms[i].localPosition = traj.trajectoryRenderers[i - 1].GetPosition(0);
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
            gestureObjs[i].allocatedPos = newPos;
            gestureObjs[i].GetComponent<Transform>().localPosition = newPos;
        }
    }

    public void PrepareStack()
    {
        float a = 1.0f;
        a = a / stackedObjects.Count + 0.2f;

        foreach(GameObject obj in stackedObjects)
        {
            MeshRenderer[] mr = obj.GetComponent<Transform>().Find("Trajectory").Find("Skeleton").GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer m in mr)
            {
                Color temp = m.material.color;
                temp.a = a;
                m.material.color = temp;
            }

            LineRenderer[] traj = obj.GetComponent<Transform>().Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<LineRenderer>();
            foreach (LineRenderer lr in traj)
            {
                Color temp = lr.endColor;
                temp.a = a;
                lr.startColor = temp;
                lr.endColor = temp;
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

    public void UpdateGlowingFieldColour(GameObject gesVis)
    {
        int id = gesVis.GetComponent<GestureGameObject>().gesture.cluster;
        gesVis.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = clusterColorDic[id];
    }
}