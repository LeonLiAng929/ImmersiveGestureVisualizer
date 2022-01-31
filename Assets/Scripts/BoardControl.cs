using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoardControl : MonoBehaviour
{
    protected XRGrabInteractable xRGrab;
    public TextMesh boardInfo;
    public GameObject ControlObjects;
    // Start is called before the first frame update
    void Start()
    {
        xRGrab = transform.parent.gameObject.GetComponent<XRGrabInteractable>();
        xRGrab.firstHoverEntered.AddListener(ShowBoardInfo);
        xRGrab.lastHoverExited.AddListener(HideBoardInfo);
        xRGrab.activated.AddListener(BoardControlPenal);
        ControlObjects.SetActive(false);
    }

    public void ShowBoardInfo(HoverEnterEventArgs args)
    {
        boardInfo.gameObject.SetActive(true);
    }

    public void HideBoardInfo(HoverExitEventArgs args)
    {
        boardInfo.gameObject.SetActive(false);
    }

    public void BoardControlPenal(ActivateEventArgs args)
    {
        ControlObjects.SetActive(!ControlObjects.activeSelf);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
