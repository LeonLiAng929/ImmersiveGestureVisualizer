using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IncrementCluster : MonoBehaviour
{
    public TextMesh text;
    protected XRSimpleInteractable xRSimpleInteractable;

    #region Singleton
    public static IncrementCluster instance;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Increment);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Increment(SelectExitEventArgs args)
    {
        if (GestureVisualizer.instance.k < GestureAnalyser.instance.GetGestureCount())
        {
            GestureVisualizer.instance.k += 1;
            text.text = "No. of Clusters: \n" + GestureVisualizer.instance.k.ToString();
        }
    }
    public void _Increment()
    {
            GestureVisualizer.instance.k += 1;
            text.text = "No. of Clusters: \n" + GestureVisualizer.instance.k.ToString();
    }
}
