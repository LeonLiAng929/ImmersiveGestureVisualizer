using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClusterGameObject : MonoBehaviour
{
    public int clusterID;
    public Transform baryCentreVis;
    private XRSimpleInteractable interactable = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!baryCentreVis.GetComponent<GestureGameObject>().IsStacked())
        {
            // keep bary centre visualization at the centroid of the cluster.
            this.GetComponent<Transform>().localPosition = baryCentreVis.localPosition;
            baryCentreVis.localScale = GetComponent<Transform>().localScale * (float)0.8;
        }
    }
    private void OnDestroy()
    {
        interactable.activated.RemoveAllListeners();
    }

    public void InitializeClusterVisualizationScale(Vector3 scale)
    {
        // set cluster size 
        Transform trans = GetComponent<Transform>();
        trans.localScale = scale;

        if (interactable == null)
        {
            interactable = GetComponent<XRSimpleInteractable>();
            interactable.activated.AddListener(PerformAction);
        }

        if (scale.y > 1 && GestureVisualizer.instance.globalArrangement)
        {
            Vector3 size = baryCentreVis.GetComponent<GestureGameObject>().gesture.GetBoundingBoxSize();
            float initHeight = size.y;
            float currHeight = size.y * scale.y;
            Vector3 temp = baryCentreVis.transform.localPosition;
            temp.y += (currHeight - initHeight)/2;
            //Debug.Log(baryCentreVis.parent.name + "init: " + baryCentreVis.transform.localPosition.ToString() + " now: " + temp.ToString());
            baryCentreVis.transform.localPosition = temp;
            baryCentreVis.GetComponent<GestureGameObject>().allocatedPos = temp;
        }
    }

    public void UpdateClusterVisualizationScale()
    {
        float count = Mathf.Sqrt(GestureAnalyser.instance.GetClusterByID(clusterID).GestureCount());
        InitializeClusterVisualizationScale(new Vector3(count, count, count));
    }

    public void PerformAction(ActivateEventArgs arg)
    {
        Actions curr = ActionSwitcher.instance.GetCurrentAction();
        if (curr == Actions.StackGestures) { StackAll(); }
        else if (curr == Actions.UnfoldCluster) { UnfoldCluster(); }
        else if (curr == Actions.ChangeCluster) { ChangeCluster(); }
        else if (curr == Actions.Animate) { ActivateAnimate(); }
    }

    public void ActivateAnimate()
    {
        foreach (GestureGameObject gGO in gameObject.transform.parent.gameObject.GetComponentsInChildren<GestureGameObject>(true))
        {
            gGO.ActivateAnimate();
        }
    }

    public void ChangeCluster()
    {
        Cluster c = GestureAnalyser.instance.GetClusterByID(clusterID);
        List<GameObject> selectedGestures = GestureVisualizer.instance.selectedGestures;
        List<GameObject> originalClusterObjs = new List<GameObject>();
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
            original.RemoveGesture(gGO.GetComponent<GestureGameObject>().gesture);
            c.AddGesture(gGO.GetComponent<GestureGameObject>().gesture);
            if (!originalClusterObjs.Contains(obj))
            {
                originalClusterObjs.Add(obj);
            }
        }
        foreach(GameObject g in originalClusterObjs)
        {
            if (g != null)
            {
                ClusterGameObject clusterObj = g.GetComponentInChildren<ClusterGameObject>();
                Destroy(clusterObj.baryCentreVis.gameObject);
                GestureVisualizer.instance.InstantiateAverageGestureVis(g, clusterObj.clusterID);
                //clusterObj.UpdateClusterVisualizationScale();
                ClusterTag ctag = g.GetComponentInChildren<ClusterTag>();
                ctag.UpdateTag();
                GestureTag[] gestureTags = g.GetComponentsInChildren<GestureTag>();
                foreach(GestureTag gtag in gestureTags)
                {
                    gtag.UpdateTag();
                }
            }
        }
        Destroy(baryCentreVis.gameObject);
        GestureVisualizer.instance.InstantiateAverageGestureVis(gameObject.transform.parent.gameObject, clusterID);
        /*GestureVisualizer.instance.ArrangeLocationForGestures();
        foreach (GameObject g in originalClusterObjs)
        {
            if (g != null)
            {
                ClusterGameObject clusterObj = g.GetComponentInChildren<ClusterGameObject>();
                clusterObj.UpdateClusterVisualizationScale();
            }
        }
        UpdateClusterVisualizationScale();*/
        ClusterTag clusterTag = GetComponentInChildren<ClusterTag>();
        clusterTag.UpdateTag();
        GestureTag[] gTags = GetComponentsInChildren<GestureTag>();
        foreach (GestureTag gtag in gTags)
        {
            gtag.UpdateTag();
        }
        GestureVisualizer.instance.AdjustClusterPosition();
        GestureVisualizer.instance.EmptySelected();
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
            //Vector3 newPos = location + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 newPos = location + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            //newPos.y = yPosition;
            //Instantiate(gestureObj, newPos, gestureObj.rotation);
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
        //InstantiateInCircle(gestures, new Vector3(0,0,0));
    } 
    
}
