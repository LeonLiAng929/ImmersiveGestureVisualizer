using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using System;

public class BoardSnap : MonoBehaviour
{
    protected XRGrabInteractable xRGrab;
    public GameObject background;
    public GameObject SnapOptions;
    public Vector2 lengthByWidth = new Vector2(); //e.g. (2,3) means the board is 2f of length and 3f of width;
    // Start is called before the first frame update
    void Start()
    {
        xRGrab = transform.parent.gameObject.GetComponent<XRGrabInteractable>();
        SnapOptions.SetActive(false);
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
        SnapOptions.SetActive(true);
    }

    public void HideSnapOptions()
    {
        SnapOptions.SetActive(false);
    }

    public void SetBoundingBox()
    {
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        Gesture2DObject[] uiRefList = transform.parent.Find("Contents").GetComponentsInChildren<Gesture2DObject>(true);
        foreach (Gesture2DObject obj in uiRefList)
        {
            if (obj.transform.localPosition.x < minX)
            {
                minX = obj.transform.localPosition.x;
            }
            if (obj.transform.localPosition.x > maxX)
            {
                maxX = obj.transform.localPosition.x;
            }
            if (obj.transform.localPosition.y < minY)
            {
                minY = obj.transform.localPosition.y;
            }
            if (obj.transform.localPosition.y > maxY)
            {
                maxY = obj.transform.localPosition.y;
            }
        }
        lengthByWidth = new Vector2(Math.Abs(maxX - minX), Math.Abs(maxY - minY));
    }

    public void ApplyBackground()
    {
        background.transform.localScale = new Vector3(lengthByWidth.x + 0.1f, lengthByWidth.y +0.1f, 1); 
    }
}
