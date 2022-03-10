using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DecrementCluster : MonoBehaviour
{
    public TextMesh text;
    protected XRSimpleInteractable xRSimpleInteractable;

    #region Singleton
    public static DecrementCluster instance;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Decrement);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Decrement(SelectExitEventArgs args)
    {
        if (GestureVisualizer.instance.k > 1)
        {
            GestureVisualizer.instance.k -= 1;
            text.text = "No. of Clusters: \n" + GestureVisualizer.instance.k.ToString();
        }
    }

    public void _Decrement()
    {
        GestureVisualizer.instance.k -= 1;
        text.text = "No. of Clusters: \n" + GestureVisualizer.instance.k.ToString();
    }
}
