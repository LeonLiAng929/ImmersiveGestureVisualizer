using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReCluster : MonoBehaviour
{
    #region Singleton
    public static ReCluster instance;
    #endregion
    protected XRSimpleInteractable xRSimpleInteractable;
    public bool reclusterOngoing = false;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Recluster);
        

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Recluster(SelectExitEventArgs args)
    {
        FadeScreen.instance.FadeOut();
        reclusterOngoing = true;
        GestureVisualizer.instance.DestroyAllClusters();
      
      
        StartCoroutine(GestureVisualizer.instance.InitializeVisualization());
        //FadeScreen.instance.FadeIn();
        reclusterOngoing = false;
    }
}
