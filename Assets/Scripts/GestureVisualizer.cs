using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;



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
    protected Transform GesVisContainer;
    [SerializeField]
    protected GameObject skeletonModel;
    [SerializeField]
    protected GameObject clusterVisPrefab;

    private Dictionary<int, GameObject> clustersObjDic = new Dictionary<int, GameObject>();
    private List<GameObject> trajectoryObjects = new List<GameObject>();
    private int k = 0;
    private List<Color> trajectoryColorSet = new List<Color>();
    private Dictionary<int, Color> clusterColorDic = new Dictionary<int, Color>();
    private void Start()
    {
        List<Gesture> gestures = GestureAnalyser.instance.GetGestures();

        //instantiate Clusters
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

        float y = 0;

        //set trajectory and tracer colors

        for (int j = 0; j < gestures[0].poses[0].num_of_joints; j++)
        {
            Color c = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            trajectoryColorSet.Add(c);
        }

        foreach (Gesture g in gestures)
        {
            // instantiate Gesture visualizations. A gesture visualziation has a trajectory view and a small-multiples view.
            GameObject newGesVis = Instantiate(gesVisPrefab, clustersObjDic[g.cluster].GetComponent<Transform>());
            //GameObject newGesVis = Instantiate(gesVisPrefab, GesVisContainer);
            /*newGesVis.AddComponent(typeof(GestureGameObject));
            newGesVis.GetComponent<GestureGameObject>().gesture = g;*/
            newGesVis.name = g.gestureType + g.id.ToString();
            Transform newGesVisTrans = newGesVis.GetComponent<Transform>();

            /* // instantiate trajectory
             GameObject newTrajObj = Instantiate(trajectoryPrefab, newGesVisTrans);
             newTrajObj.name = g.gestureType + "Trajectory";
             newTrajObj.AddComponent<Trajectory>();
             Trajectory traj = newTrajObj.GetComponent<Trajectory>();
             traj.SetAttributes(g, newTrajObj, trajectoryRendererPrefab, tracerRef);
             traj.DrawTrajectory();
             trajectoryObjects.Add(newTrajObj);
            */

            InstantiateTrajectory(newGesVis, g);

            // instantiate small-multiples 
            List<Pose> sampled = g.Resample(5);
            float x = 0;
            int samplePoseIndex = 1;
            foreach (Pose p in sampled)
            {
                GameObject skeleton = Instantiate(skeletonModel, newGesVisTrans.GetComponent<Transform>().Find("SmallMultiples").GetComponent<Transform>());
                //p.model = skeleton;
                skeleton.name = g.gestureType + g.id.ToString() + "-Pose" + samplePoseIndex.ToString();
                samplePoseIndex++;
                Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
                //Debug.Log(transforms.Length);
                transforms[0].localPosition = new Vector3(y, g.cluster, x);
                x += 1;
                for (int i = 1; i < 21; i++)
                {
                    transforms[i].localPosition = p.joints[i-1].ToVector();
                }
            }
            y = y + float.Parse("1.5");
        }

      
        /*
            for (int j = 0; j < trajectoryObjects[0].GetComponent<Trajectory>().trajectoryRenderers.Count; j++)
        {
            //Color c = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            //Color c = colorSet[j];
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.Lerp(c, Color.white, 0.5f), 0.0f), new GradientColorKey(Color.Lerp(c, Color.black, 0.5f), 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            for (int i = 0; i < trajectoryObjects.Count; i++)
            {
                trajectoryObjects[i].GetComponent<Trajectory>().trajectoryRenderers[j].colorGradient = gradient;
            }
        }*/


       /* //instantiate skeletonReference for trajectories
        int k = 0;
        foreach (GameObject trajObj in trajectoryObjects)
        {
            Trajectory t = trajObj.GetComponent<Trajectory>();
            Transform trajTrans = t.GetComponent<Transform>();
            GameObject skeleton = Instantiate(skeletonModel, trajTrans);
            t.skeletonRef = skeleton;
            Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
            //Debug.Log(transforms.Length);
            //transforms[0].localPosition = new Vector3(y, 0, x);
            trajObj.GetComponent<Transform>().localPosition = new Vector3(k, 0, -1);
            k += 1;
            for (int i = 1; i < 21; i++)
            {
                transforms[i].localPosition = t.trajectoryRenderers[i-1].GetPosition(0);
            }
        }
       */
        // set size for the clusters in the scene
        foreach (KeyValuePair<int, GameObject> p in clustersObjDic)
        {
            ClusterGameObject a = p.Value.GetComponentInChildren<ClusterGameObject>();
            float count = Mathf.Sqrt(clusters[p.Key].GestureCount());
            a.InitializeClusterVisualization(new Vector3(count, count, count));
        }

        // instantiate barycentre visualization for each cluster
        foreach (KeyValuePair<int, GameObject> p in clustersObjDic)
        {
            GameObject newGesVis = Instantiate(gesVisPrefab, p.Value.GetComponent<Transform>());
            //GameObject newGesVis = Instantiate(gesVisPrefab, GesVisContainer);
            /*newGesVis.AddComponent(typeof(GestureGameObject));
            newGesVis.GetComponent<GestureGameObject>().gesture = g;*/
            newGesVis.name = "AverageGesture";
            Transform newGesVisTrans = newGesVis.GetComponent<Transform>();
            ClusterGameObject a = p.Value.GetComponentInChildren<ClusterGameObject>();
            a.baryCentreVis = newGesVisTrans;
            InstantiateTrajectory(newGesVis, GestureAnalyser.instance.GetClusterByID(a.clusterID).GetBaryCentre());
        }

        Destroy(tracerRef);
        Destroy(skeletonModel);
        Destroy(trajectoryPrefab);
        Destroy(gesVisPrefab);
    }
    private void Update()
    {
        foreach (GameObject trajObj in trajectoryObjects)
        {
            Trajectory t = trajObj.GetComponent<Trajectory>();
            t.UpdateTrajectoryPos();
        }
        //trajectories[0].UpdateTrajectoryPos();
    }

    public void InstantiateTrajectory(GameObject gestureVis, Gesture g)
    {
        Transform newGesVisTrans = gestureVis.GetComponent<Transform>();

        // instantiate trajectory
        GameObject newTrajObj = Instantiate(trajectoryPrefab, newGesVisTrans);
        newTrajObj.name = g.gestureType + "Trajectory";
        newTrajObj.AddComponent<Trajectory>();
        Trajectory traj = newTrajObj.GetComponent<Trajectory>();
        traj.SetAttributes(g, newTrajObj, trajectoryRendererPrefab, tracerRef);
        traj.DrawTrajectory(trajectoryColorSet);
        trajectoryObjects.Add(newTrajObj);

        //instantiate skeletonReference for trajectories
        Transform trajTrans = traj.GetComponent<Transform>();
        GameObject skeleton = Instantiate(skeletonModel, trajTrans);
        foreach (MeshRenderer mr in skeleton.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material.color = clusterColorDic[g.cluster];
        }
        traj.skeletonRef = skeleton;
        Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
        //Debug.Log(transforms.Length);
        //transforms[0].localPosition = new Vector3(y, 0, x);
        newTrajObj.GetComponent<Transform>().localPosition = new Vector3(k, 0, -1);
        k += 1;
        for (int i = 1; i < 21; i++)
        {
            transforms[i].localPosition = traj.trajectoryRenderers[i - 1].GetPosition(0);
        }
    }
}