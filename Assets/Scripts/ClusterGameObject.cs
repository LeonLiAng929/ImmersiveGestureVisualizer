using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClusterGameObject : MonoBehaviour
{
    public int clusterID;
    public Transform baryCentreVis;
    private Vector3 initialPos;
    private bool selected = false;
    private float riseUpHeightConstant = 3;
    private XRGrabInteractable interactable = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // keep bary centre visualization at the centroid of the cluster.
        this.GetComponent<Transform>().localPosition = baryCentreVis.localPosition;
        baryCentreVis.localScale = GetComponent<Transform>().localScale * (float)0.8;
        /*if (selected) { 
            RiseUp();
        }
        else
        FallDown();*/
    }

    public void InitializeClusterVisualization(Vector3 scale)
    {
        // set cluster size 
        Transform trans = GetComponent<Transform>();
        trans.localScale = scale;
        //trans.localPosition = new Vector3(scale.x*3, scale.y / 2, 0);

        if (interactable == null)
        {
            interactable = GetComponent<XRGrabInteractable>();
            interactable.activated.AddListener(GetSelected);
        }

        // register initial position
        initialPos = trans.localPosition;

    }

    public void RiseUp()
    {
        Transform trans = GetComponent<Transform>();
        Vector3 start_pos = trans.localPosition;
        Vector3 end_pos = new Vector3(initialPos.x, initialPos.y + riseUpHeightConstant, initialPos.z);
        if (!Reached(start_pos, end_pos))
        {
            //Debug.Log("start: "+start_pos.ToString());
            //Debug.Log("end:" + end_pos.ToString());
         
            trans.localPosition += Vector3.up * Time.deltaTime;
        }
    }

    public void FallDown()
    {
        Transform trans = GetComponent<Transform>();
        Vector3 start_pos = trans.localPosition;
        Vector3 end_pos = initialPos;
        if (!Reached(end_pos, start_pos))
        {
            trans.localPosition += Vector3.down * Time.deltaTime;
        }
    }

 
   
    public bool Reached(Vector3 curr, Vector3 destination)
    {
        return curr == destination;
    }
    public void GetSelected(ActivateEventArgs interactor)
    {
        if (selected)
        {
            selected = false;
        }
        else
        {
            selected = true;
        }
    }

    public void InstantiateInCircle(List<GestureGameObject> gestureObjs, Vector3 location)
    {
        int howMany = gestureObjs.Count;
        float angleSection = Mathf.PI * 2f / howMany;
        
        for (int i = 0; i < howMany; i++)
        {
            float angle = i * angleSection;
            
            float radius = gestureObjs[i].gesture.GetGlobalSimilarity();
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

        List<GestureGameObject> gestures = new List<GestureGameObject>(clusterGameObj.GetComponentsInChildren<GestureGameObject>());
        gestures.Remove(averageGes);
        Transform baryTrans = baryCentreVis.GetComponent<Transform>();

        //InstantiateInCircle(gestures, baryTrans.localPosition);
        InstantiateInCircle(gestures, new Vector3(0,0,0));
    } 
    
}
