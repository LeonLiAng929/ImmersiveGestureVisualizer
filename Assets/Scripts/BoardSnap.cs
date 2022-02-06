using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class BoardSnap : MonoBehaviour
{
    protected XRGrabInteractable xRGrab;
    // Start is called before the first frame update
    void Start()
    {
        xRGrab = transform.parent.gameObject.GetComponent<XRGrabInteractable>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (xRGrab.isSelected)
        {
            if (other.gameObject.name=="Grab")
            {
                other.gameObject.GetComponent<BoardSnap>().DisplaySnapOptions();
            }

        }
    }
    
    public void DisplaySnapOptions()
    {
        Debug.Log("haha");
    }
}
