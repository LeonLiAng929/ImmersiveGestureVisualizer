using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ChangeColor : MonoBehaviour
{
    XRSimpleInteractable xr;
    // Start is called before the first frame update
    void Start()
    {
        xr = GetComponent<XRSimpleInteractable>();
        xr.activated.AddListener(changeColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeColor(ActivateEventArgs arg)
    {
        GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
    }
}
