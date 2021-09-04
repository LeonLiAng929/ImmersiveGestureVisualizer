using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRTag : MonoBehaviour
{
    [SerializeField]
    protected float offsetY = 0;
    [SerializeField]
    protected bool focus;
    [SerializeField]
    protected Vector3 tagSize = new Vector3(1, 1, 1);
    protected Transform user;
    protected XRSimpleInteractable simpleInteractable;
    protected GameObject textTag;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHovered(HoverEnterEventArgs arg)
    {
        textTag.SetActive(true);
    }

    public void OnHoverExit(HoverExitEventArgs arg)
    {
        textTag.SetActive(false);
    }
}
