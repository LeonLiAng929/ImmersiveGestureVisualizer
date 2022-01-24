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
        xRSimpleInteractable.selectExited.AddListener(MoveGes);
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        selectionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(Camera.main.transform.position);
    }

    private void MoveGes(SelectExitEventArgs arg)
    {
        if (GestureVisualizer.instance.adjustTranform)
        {
            GestureVisualizer.instance.adjustTranform = false;
            GestureVisualizer.instance.proposedGestureObj.transform.parent = null;
            selectionIndicator.SetActive(false);
            //GestureVisualizer.instance.proposedGestureObj.transform.localPosition = new Vector3(0, 0, 0);
            //GestureVisualizer.instance.proposedGestureObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            GestureVisualizer.instance.adjustTranform = true;
            GestureVisualizer.instance.proposedGestureObj.transform.parent = Camera.main.gameObject.transform.parent;
            
            selectionIndicator.SetActive(true);
        }
    }
}
