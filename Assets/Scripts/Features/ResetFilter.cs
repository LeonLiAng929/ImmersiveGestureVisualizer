using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetFilter : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;
    private bool reset = true;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localPosition = gameObject.transform.parent.Find("Head").localPosition + new Vector3(0, 0.1f, 0);
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.selectExited.AddListener(Reset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Reset(SelectExitEventArgs arg)
    {
        TrajectoryFilter[] filters = gameObject.transform.parent.GetComponentsInChildren<TrajectoryFilter>();
        if (reset)
        {
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
            reset = false;
        }
        else
        {
            foreach (TrajectoryFilter filter in filters)
            {
                if (filter.shown)
                {
                    filter.shown = false;
                    filter.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
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
            reset = true;
        }
    }
}
