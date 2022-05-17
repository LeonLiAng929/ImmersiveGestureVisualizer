using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PrevMethod : MonoBehaviour
{
    public TextMesh text;
    public GameObject k;
    public GameObject increment;
    public GameObject decrement;
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
        ClusteringRationales rationale = GestureVisualizer.instance.clusteringRationale;
        ClusteringMethods method = GestureVisualizer.instance.clusteringMethod;
        if (method == ClusteringMethods.K_Means && rationale == ClusteringRationales.DBA)
        {
            GestureVisualizer.instance.clusteringRationale = ClusteringRationales.Raw;
            TextMesh t = this.transform.parent.Find("Rationale").GetComponent<TextMesh>();
            t.text = "Rationale: \n" + "Raw";
        }
        if (method == ClusteringMethods.MeanShift && rationale == ClusteringRationales.Raw)
        {
            GestureVisualizer.instance.clusteringRationale = ClusteringRationales.DBA;
            TextMesh t = this.transform.parent.Find("Rationale").GetComponent<TextMesh>();
            t.text = "Rationale: \n" + "DBA";
        }
        if (method == ClusteringMethods.K_Means)
        {
            bool b = k.activeSelf;
            k.SetActive(!b);
            increment.SetActive(!b);
            decrement.SetActive(!b);
            text.text = "Method: MeanShift";
            GestureVisualizer.instance.clusteringMethod = ClusteringMethods.MeanShift;
        }
        else if (method == ClusteringMethods.MeanShift)
        {
            bool b = k.activeSelf;
            k.SetActive(!b);
            increment.SetActive(!b);
            decrement.SetActive(!b);
            text.text = "Method: K-Means";
            GestureVisualizer.instance.clusteringMethod = ClusteringMethods.K_Means;
        }
    }
}
