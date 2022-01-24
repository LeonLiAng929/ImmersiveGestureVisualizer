using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CancelSearch : MonoBehaviour
{
    private Color init;
    private XRSimpleInteractable xRSimpleInteractable;
    private GameObject selectionIndicator;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Resume);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        selectionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Resume(SelectExitEventArgs arg)
    {
        Debug.Log("Reset!");
        selectionIndicator.SetActive(!selectionIndicator.activeSelf);

        foreach (GestureGameObject g in GestureVisualizer.instance.searchResult)
        {
            g.RevertComparing();
        }
        GestureVisualizer.instance.proposedGes = new Gesture();
        GestureVisualizer.instance.time = 0;
        GestureVisualizer.instance.searchResult = new List<GestureGameObject>();
        GameObject proposedGesObj = GestureVisualizer.instance.proposedGestureObj;
        proposedGesObj.transform.position = new Vector3(0, 0, 0);
        proposedGesObj.transform.localPosition = new Vector3(0, 0, 0);
        proposedGesObj.transform.rotation = new Quaternion(0, 0, 0, 0);
        proposedGesObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        proposedGesObj.transform.Find("WristLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("ElbowLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("ShoulderLeft").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("WristRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("ElbowRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
        proposedGesObj.transform.Find("ShoulderRight").GetComponent<TubeRenderer>().points = new Vector3[] { };
        selectionIndicator.SetActive(!selectionIndicator.activeSelf);
    }
}
