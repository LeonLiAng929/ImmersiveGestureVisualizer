using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShowHideBoardContents : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    public GameObject contents;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(ShowHideContents);
    }


    public void ShowHideContents(SelectExitEventArgs args)
    {
        contents.SetActive(!contents.activeSelf);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
