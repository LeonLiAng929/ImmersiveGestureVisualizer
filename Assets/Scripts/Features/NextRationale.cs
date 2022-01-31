using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NextRationale : MonoBehaviour
{
    public TextMesh text;
    protected XRSimpleInteractable xRSimpleInteractable;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Next);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Next(SelectExitEventArgs args)
    {
        if (GestureVisualizer.instance.clusteringRationale == ClusteringRationales.DBA)
        {
            GestureVisualizer.instance.clusteringRationale = ClusteringRationales.PCA;
            text.text = "Rationale: \n" + "PCA";
        }
        else if(GestureVisualizer.instance.clusteringRationale == ClusteringRationales.PCA)
        {
            GestureVisualizer.instance.clusteringRationale = ClusteringRationales.MDS;
            text.text = "Rationale: \n" + "MDS";
        }
        else if (GestureVisualizer.instance.clusteringRationale == ClusteringRationales.MDS)
        {
            GestureVisualizer.instance.clusteringRationale = ClusteringRationales.DBA;
            text.text = "Rationale: \n" + "DBA";
        }
    }
}
