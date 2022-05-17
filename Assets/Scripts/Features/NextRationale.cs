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
        ClusteringRationales rationale = GestureVisualizer.instance.clusteringRationale;
        ClusteringMethods method = GestureVisualizer.instance.clusteringMethod;
        if (method == ClusteringMethods.K_Means)
        {
            if (rationale == ClusteringRationales.DBA)
            {
                GestureVisualizer.instance.clusteringRationale = ClusteringRationales.PCA;
                text.text = "Rationale: \n" + "PCA";
            }
            else if (rationale == ClusteringRationales.PCA)
            {
                GestureVisualizer.instance.clusteringRationale = ClusteringRationales.MDS;
                text.text = "Rationale: \n" + "MDS";
            }
            else if (rationale == ClusteringRationales.MDS)
            {
                GestureVisualizer.instance.clusteringRationale = ClusteringRationales.DBA;
                text.text = "Rationale: \n" + "DBA";
            }
        }
        else if (method == ClusteringMethods.MeanShift)
        {
            if (rationale == ClusteringRationales.Raw)
            {
                GestureVisualizer.instance.clusteringRationale = ClusteringRationales.PCA;
                text.text = "Rationale: \n" + "PCA";
            }
            else if (rationale == ClusteringRationales.PCA)
            {
                GestureVisualizer.instance.clusteringRationale = ClusteringRationales.MDS;
                text.text = "Rationale: \n" + "MDS";
            }
            else if (rationale == ClusteringRationales.MDS)
            {
                GestureVisualizer.instance.clusteringRationale = ClusteringRationales.Raw;
                text.text = "Rationale: \n" + "Raw";
            }
        }
    }
}
