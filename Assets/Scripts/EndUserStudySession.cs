using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EndUserStudySession : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(EndSession);
    }

    private void EndSession(SelectExitEventArgs args)
    {
        UserStudy.instance.EndSession();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
