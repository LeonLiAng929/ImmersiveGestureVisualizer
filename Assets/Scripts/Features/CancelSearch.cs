using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CancelSearch : MonoBehaviour
{
    private Color init;
    private XRSimpleInteractable xRSimpleInteractable;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.activated.AddListener(Resume);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Resume(ActivateEventArgs arg)
    {
        Debug.Log("reset!");

        foreach (GestureGameObject g in GestureVisualizer.instance.searchResult)
        {
            g.RevertComparing();
        }
        GestureVisualizer.instance.proposedGes = new Gesture();
        GestureVisualizer.instance.time = 0;
        GestureVisualizer.instance.searchResult = new List<GestureGameObject>();
        GameObject proposedGesObj = GestureVisualizer.instance.proposedGestureObj;
        proposedGesObj.transform.Find("WristLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("ElbowLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("ShoulderLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("WristRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("ElbowRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("ShoulderRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
    }
}
