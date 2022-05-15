using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PrevRationale : MonoBehaviour
{
    public TextMesh text;
    protected XRSimpleInteractable xRSimpleInteractable;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Prev);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Prev(SelectExitEventArgs args)
    {
        if (GestureVisualizer.instance.clusteringRationale == ClusteringRationales.DBA)
        {
            GestureVisualizer.instance.clusteringRationale = ClusteringRationales.K_Means_MDS;
            text.text = "Rationale: \n" + "MDS";
        }
        else if(GestureVisualizer.instance.clusteringRationale == ClusteringRationales.K_Means_PCA)
        {
            GestureVisualizer.instance.clusteringRationale = ClusteringRationales.DBA;
            text.text = "Rationale: \n" + "DBA";
        }
        else if (GestureVisualizer.instance.clusteringRationale == ClusteringRationales.K_Means_MDS)
        {
            GestureVisualizer.instance.clusteringRationale = ClusteringRationales.K_Means_PCA;
            text.text = "Rationale: \n" + "PCA";
        }
    }
}
