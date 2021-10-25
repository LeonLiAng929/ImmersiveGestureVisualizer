using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoveGesture : MonoBehaviour
{
    private XRSimpleInteractable xRSimpleInteractable;
    private GameObject selectionIndicator;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.activated.AddListener(MoveGes);
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        selectionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(Camera.main.transform.position);
    }

    private void MoveGes(ActivateEventArgs arg)
    {
        if (GestureVisualizer.instance.adjustTranform)
        {
            GestureVisualizer.instance.adjustTranform = false;
            GestureVisualizer.instance.proposedGestureObj.transform.parent = null;
            selectionIndicator.SetActive(false);
        }
        else
        {
            GestureVisualizer.instance.adjustTranform = true;
            GestureVisualizer.instance.proposedGestureObj.transform.parent = Camera.main.gameObject.transform.parent;
            selectionIndicator.SetActive(true);
        }
    }
}
