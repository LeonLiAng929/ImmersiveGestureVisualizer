using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConfirmComparison : MonoBehaviour
{
    private XRSimpleInteractable xRSimpleInteractable;
    private GameObject selectionIndicator;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(ConfirmCompare);
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        selectionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfirmCompare(SelectExitEventArgs args)
    {
        GestureVisualizer.instance.CloseComparison();
    }
}
