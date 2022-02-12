using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TrajectoryFilter : MonoBehaviour
{
    [HideInInspector]
    public Color init;
    private XRSimpleInteractable xRSimpleInteractable;
    [HideInInspector]
    public bool shown = true;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Filter);
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }

    private void Filter(SelectExitEventArgs arg)
    {
        if (shown)
        {
            shown = false;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        else
        {
            shown = true;
            gameObject.GetComponent<MeshRenderer>().material.color = init;
        }
        foreach(KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.GetClusterObjs())
        {
            GestureGameObject[] gestureGameObjects = pair.Value.GetComponentsInChildren<GestureGameObject>(true);
            foreach (GestureGameObject gGO in gestureGameObjects)
            {
                gGO.gameObject.transform.Find("Trajectory").Find("LineRanderers").Find(gameObject.name).gameObject.SetActive(shown);

            }
        }
    }
}
