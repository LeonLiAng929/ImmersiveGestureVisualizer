using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ChangeArrangement : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    //[SerializeField]
    //protected Material selected;
    //[SerializeField]
    //protected Material deselected;

    private bool on = false;
    Color init;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.activated.AddListener(Rearrange);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Rearrange(ActivateEventArgs arg)
    {
        Dictionary<int, GameObject> clusterObjs = GestureVisualizer.instance.GetClusterObjs();
        if (!on)
        {
            on = true;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            GestureVisualizer.instance.globalArrangement = false;
            GestureVisualizer.instance.AdjustClusterPosition();

        }
        else
        {
            on = false;
            gameObject.GetComponent<MeshRenderer>().material.color = init;
            GestureVisualizer.instance.globalArrangement = true;
            GestureVisualizer.instance.AdjustClusterPosition();
        }
    }
}
