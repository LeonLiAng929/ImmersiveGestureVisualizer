using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LookAtMe : MonoBehaviour
{
    Transform user;
    XRSimpleInteractable simpleInteractable;
    // Start is called before the first frame update
    void Start()
    {
        user = Camera.main.gameObject.transform;
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        //simpleInteractable.firstHoverEntered.AddListener(OnHovered);
        //simpleInteractable.lastHoverExited.AddListener(OnHoverExit);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(user.position);
    }

    public void OnHovered(HoverEnterEventArgs arg)
    {
        //gameObject.transform.parent.GetComponent<Revolving>().hovered = true;
    }

    public void OnHoverExit(HoverExitEventArgs arg)
    {
        //gameObject.transform.parent.GetComponent<Revolving>().hovered = false;
    }
}
