using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HighlightConnection : MonoBehaviour
{
    // Start is called before the first frame update
    private XRSimpleInteractable xRSimpleInteractable;
    private GameObject selectionIndicator;
    private bool highlighted = false;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.activated.AddListener(Highlight);
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        selectionIndicator.SetActive(false);
    }

    public void Highlight(ActivateEventArgs args)
    {
        if (!highlighted)
        {
            GestureVisualizer.instance.ShowConnection();
            highlighted = true;
            selectionIndicator.SetActive(true);
        }
        else
        {
            highlighted = false;
            GestureVisualizer.instance.DestroyConnection();
            selectionIndicator.SetActive(false);
        }
    }
}
