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

    private int mode = 0; //0 = local, 1=global, 2 = lineUp
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
        if (mode == 0)
        {
            mode = 2;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            GestureVisualizer.instance.arrangementMode = 0;
            GestureVisualizer.instance.AdjustClusterPosition();

        }
        else if (mode == 1){
            mode = 0;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            GestureVisualizer.instance.arrangementMode = 1;
            GestureVisualizer.instance.AdjustClusterPosition();
        }
        else if ( mode == 2)
        {
            mode = 1;
            gameObject.GetComponent<MeshRenderer>().material.color = init;
            GestureVisualizer.instance.arrangementMode = 2;
            GestureVisualizer.instance.AdjustClusterPosition();
        }
    }
}
