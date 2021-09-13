using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetFilter : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localPosition = gameObject.transform.parent.Find("Head").localPosition + new Vector3(0, 0.1f, 0);
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.activated.AddListener(Reset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Reset(ActivateEventArgs arg)
    {
        TrajectoryFilter[] filters = gameObject.transform.parent.GetComponentsInChildren<TrajectoryFilter>();
        foreach (TrajectoryFilter filter in filters)
        {
            if (!filter.shown)
            {
                filter.shown = true;
                filter.gameObject.GetComponent<MeshRenderer>().material.color = filter.init;
                foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.GetClusterObjs())
                {
                    GestureGameObject[] gestureGameObjects = pair.Value.GetComponentsInChildren<GestureGameObject>(true);
                    foreach (GestureGameObject gGO in gestureGameObjects)
                    {
                        gGO.gameObject.transform.Find("Trajectory").Find("LineRanderers").Find(filter.gameObject.name).gameObject.SetActive(filter.shown);

                    }
                }
            }
        }
    }
}
