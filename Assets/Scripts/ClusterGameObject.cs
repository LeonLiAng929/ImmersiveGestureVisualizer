using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClusterGameObject : MonoBehaviour
{
    public int clusterID;
    public Transform baryCentreVis = null;
    private XRSimpleInteractable interactable = null;
    
    // Start is called before the first frame update
    void Start()
    {
        if (interactable == null)
        {
            interactable = GetComponent<XRSimpleInteractable>();
            interactable.selectExited.AddListener(PerformAction);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((!ReCluster.instance.reclusterOngoing) && (baryCentreVis != null))
        {
            if (!baryCentreVis.GetComponent<GestureGameObject>().IsStacked())
            {
                // keep bary centre visualization at the centroid of the cluster.
                this.GetComponent<Transform>().localPosition = baryCentreVis.localPosition;
                baryCentreVis.localScale = GetComponent<Transform>().localScale * (float)0.8;
            }
        }
    }
    private void OnDestroy()
    {
        interactable.selectExited.RemoveAllListeners();
    }

    public void InitializeClusterVisualizationScale(Vector3 scale)
    {
        // set cluster size 
        Transform trans = GetComponent<Transform>();
        trans.localScale = scale;
        Vector3 pos = baryCentreVis.transform.localPosition;
        baryCentreVis.transform.localPosition = new Vector3(pos.x, 0, pos.z);

        if (scale.y > 1)
        {
            Vector3 size = baryCentreVis.GetComponent<GestureGameObject>().gesture.GetBoundingBoxSize();
            float initHeight = size.y;
            float currHeight = size.y * scale.y;
            Vector3 temp = baryCentreVis.transform.localPosition;
            temp.y = (currHeight - initHeight)/2;
            //Debug.Log(baryCentreVis.parent.name + "init: " + baryCentreVis.transform.localPosition.ToString() + " now: " + temp.ToString());
            baryCentreVis.transform.localPosition = temp;
            //baryCentreVis.GetComponent<GestureGameObject>().allocatedPos = temp;
        }
    }

    public void UpdateClusterVisualizationScale()
    {
        float count = Mathf.Sqrt(GestureAnalyser.instance.GetClusterByID(clusterID).GestureCount());
        InitializeClusterVisualizationScale(new Vector3(count, count, count));
    }

    public void PerformAction(SelectExitEventArgs arg)
    {
        Actions curr = ActionSwitcher.instance.GetCurrentAction();
        if (curr == Actions.StackGestures) { StackAll(); }
        else if (curr == Actions.UnfoldCluster) { UnfoldCluster(); }
        else if (curr == Actions.ChangeCluster) { ChangeCluster(); }
        else if (curr == Actions.Animate) { ActivateAnimate(); }
        else if (curr == Actions.ShowSmallMultiples) { ShowSmallMultiples(); }
        else if (curr == Actions.Slidimation) { SwingAll(); }
    }

    public void DisplayConnection()
    {
        GameObject clusterGameObj = GestureVisualizer.instance.GetClusterGameObjectById(clusterID);

        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>());
        foreach (GestureGameObject gGO in gestures)
        {
            gGO.DisplayConnection();
        }
    }
    public void SwingAll()
    {
        GameObject clusterGameObj = GestureVisualizer.instance.GetClusterGameObjectById(clusterID);

        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>(true));
        foreach (GestureGameObject gGO in gestures)
        {
            gGO.ActivateSwinging();
        }
    }
    public void ShowSmallMultiples()
    {
        GameObject clusterGameObj = GestureVisualizer.instance.GetClusterGameObjectById(clusterID);
        //GestureGameObject averageGes = clusterGameObj.GetComponent<Transform>().Find("AverageGesture").gameObject.GetComponent<GestureGameObject>();

        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>(true));
        //gestures.Remove(averageGes);
        foreach(GestureGameObject gGO in gestures)
        {
            gGO.ShowSmallMultiples();
        }
    }
    public void ActivateAnimate()
    {
        GameObject clusterGameObj = GestureVisualizer.instance.GetClusterGameObjectById(clusterID);
        
        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>(true));
        foreach (GestureGameObject gGO in gestures)
        {
            gGO.ActivateAnimate();
        }
    }

    public void ChangeCluster()
    {
        Cluster c = GestureAnalyser.instance.GetClusterByID(clusterID);
        List<GameObject> selectedGestures = GestureVisualizer.instance.selectedGestures;
        List<GameObject> originalClusterObjs = new List<GameObject>();
        List<Gesture> gestureLi = new List<Gesture>();
        List<Cluster> originalClusterLi = new List<Cluster>();
        foreach (GameObject gGO in selectedGestures)
        {
            Cluster original = GestureAnalyser.instance.GetClusterByID(gGO.GetComponent<GestureGameObject>().gesture.cluster);
            if (original.clusterID == clusterID)
            {
                Debug.LogError("Gesture " + gGO.name + " is already in cluster " + clusterID.ToString() + "!");
                continue;
            }
            GameObject obj = GestureVisualizer.instance.GetClusterGameObjectById(original.clusterID);
            gGO.transform.parent = gameObject.transform.parent;
            
            gestureLi.Add(gGO.GetComponent<GestureGameObject>().gesture);
            if (!originalClusterObjs.Contains(obj))
            {
                originalClusterObjs.Add(obj);
            }
        }
        c.AddGesture(gestureLi);
        foreach (GameObject originalClusterObject in originalClusterObjs)
        {
            ClusterGameObject clusterObj = originalClusterObject.GetComponentInChildren<ClusterGameObject>();
            Cluster original = GestureAnalyser.instance.GetClusterByID(clusterObj.clusterID);
            original.RemoveGesture(gestureLi);
        }
        foreach (GameObject originalClusterObject in originalClusterObjs)
        {
            ClusterGameObject clusterObj = originalClusterObject.GetComponentInChildren<ClusterGameObject>();
            Cluster original = GestureAnalyser.instance.GetClusterByID(clusterObj.clusterID);
            if (original.GestureCount() >0)
            {
                //ClusterGameObject clusterObj = originalClusterObject.GetComponentInChildren<ClusterGameObject>();
                Destroy(clusterObj.baryCentreVis.gameObject);
                GestureVisualizer.instance.InstantiateAverageGestureVis(originalClusterObject, clusterObj.clusterID);
                //clusterObj.UpdateClusterVisualizationScale();
                ClusterTag ctag = originalClusterObject.GetComponentInChildren<ClusterTag>(true);
                ctag.UpdateTag();
                GestureTag[] gestureTags = originalClusterObject.GetComponentsInChildren<GestureTag>(true);
                foreach(GestureTag gtag in gestureTags)
                {
                    gtag.UpdateTag();
                }
            }
        }
        if (baryCentreVis != null)
        {
            Destroy(baryCentreVis.gameObject);
        }
        GestureVisualizer.instance.InstantiateAverageGestureVis(gameObject.transform.parent.gameObject, clusterID);
        /*
        foreach (GameObject g in originalClusterObjs)
        {
            if (g != null)
            {
                ClusterGameObject clusterObj = g.GetComponentInChildren<ClusterGameObject>();
                clusterObj.UpdateClusterVisualizationScale();
            }
        }
        UpdateClusterVisualizationScale();*/
        ClusterTag clusterTag = GetComponentInChildren<ClusterTag>(true);
        clusterTag.UpdateTag();
        GestureTag[] gTags = this.transform.parent.GetComponentsInChildren<GestureTag>(true);

        foreach (GestureTag gtag in gTags)
        {
            gtag.UpdateTag();
        }
        GestureVisualizer.instance.AdjustClusterPosition();
        //ChangeArrangement.instance._Rearragne();
        //GestureVisualizer.instance.EmptySelected();   Disabled to show where the newly assigned gestures are in the new cluster.
    }
    public void StackAll()
    {
        foreach (GestureGameObject gGO in gameObject.transform.parent.gameObject.GetComponentsInChildren<GestureGameObject>(true))
        {
            gGO.StackGesture();
        }
    }

    public void UnfoldCluster()
    {
        foreach (GestureGameObject gGO in gameObject.transform.parent.gameObject.GetComponentsInChildren<GestureGameObject>(true))
        {
            bool unfold = !gGO.gameObject.activeSelf;
            if (gGO.gameObject.name != "AverageGesture")
            {
                gGO.gameObject.SetActive(unfold);
            }
        }
    }

    public void InstantiateInCircle(List<GestureGameObject> gestureObjs, Vector3 location)
    {
        int howMany = gestureObjs.Count;
        float angleSection = Mathf.PI * 2f / howMany;
        
        for (int i = 0; i < howMany; i++)
        {
            float angle = i * angleSection;
            
            float radius = gestureObjs[i].gesture.GetLocalSimilarity();
            Vector3 newPos = location + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            gestureObjs[i].GetComponent<Transform>().localPosition = newPos;
        }
    }

    public void ArrangeLocationForChildren()
    {
        GameObject clusterGameObj = GestureVisualizer.instance.GetClusterGameObjectById(clusterID);
        GestureGameObject averageGes = clusterGameObj.GetComponent<Transform>().Find("AverageGesture").gameObject.GetComponent<GestureGameObject>();

        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>(true));
        gestures.Remove(averageGes);
        Transform baryTrans = baryCentreVis.GetComponent<Transform>();
        Vector3 temp = baryTrans.localPosition;
        temp.y = 0;
        InstantiateInCircle(gestures, temp);
    } 
    
    public List<GestureGameObject> Sort()
    {
        GameObject clusterGameObj = GestureVisualizer.instance.GetClusterGameObjectById(clusterID);
        GestureGameObject averageGes = clusterGameObj.GetComponent<Transform>().Find("AverageGesture").gameObject.GetComponent<GestureGameObject>();

        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>(true));
        gestures.Remove(averageGes);
        List<GestureGameObject> result = new List<GestureGameObject>();
        while (gestures.Count > 0)
        {
            float min = float.PositiveInfinity;
            GestureGameObject minGes = null;
            foreach(GestureGameObject gGO in gestures)
            {
                if (gGO.gesture.GetLocalSimilarity() < min)
                {
                    min = gGO.gesture.GetLocalSimilarity();
                    minGes = gGO;
                }
            }
            result.Add(minGes);
            gestures.Remove(minGes);
        }
        return result;
    }

    public void LineUp()
    {
        List<GestureGameObject> sorted = Sort();
        float x = baryCentreVis.localPosition.x;
        
        for(int i =0; i < sorted.Count; i++)
        {
            sorted[i].gameObject.transform.localPosition = new Vector3(x, 0, i + 1.5f);
            sorted[i].gameObject.transform.localRotation = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)); 
        }
    }

    public void PCA_Arrangement()
    {
        GameObject clusterGameObj = GestureVisualizer.instance.GetClusterGameObjectById(clusterID);
        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>(true));
        foreach(GestureGameObject gGO in gestures)
        {
            gGO.gameObject.transform.localPosition = new Vector3(gGO.gesture.PCA_Coordinate.x*10, gGO.gameObject.transform.localPosition.y, gGO.gesture.PCA_Coordinate.y*10);
        }
    }

    public void MDS_Arrangement()
    {
        GameObject clusterGameObj = GestureVisualizer.instance.GetClusterGameObjectById(clusterID);
        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>(true));
        foreach (GestureGameObject gGO in gestures)
        {
            gGO.gameObject.transform.localPosition = new Vector3(gGO.gesture.MDS_Coordinate.x * 5, gGO.gameObject.transform.localPosition.y, gGO.gesture.MDS_Coordinate.y * 5);
        }
    }
}
