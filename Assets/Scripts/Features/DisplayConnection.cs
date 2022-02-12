using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DisplayConnection : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    private GameObject selectionIndicator;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(DisplayClusterConnection);
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        selectionIndicator.SetActive(false);
    }

    private void DisplayClusterConnection(SelectExitEventArgs args)
    {
        selectionIndicator.SetActive(!selectionIndicator.activeSelf);
        foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.GetClusterObjs())
        {
            pair.Value.transform.Find("ClusterVisualization").GetComponent<ClusterGameObject>().DisplayConnection();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
