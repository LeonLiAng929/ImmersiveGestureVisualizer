using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetFeatureCount : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(ResetCount);
    }

    private void ResetCount(SelectExitEventArgs args)
    {
        UserStudy.instance.ClearCounts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
