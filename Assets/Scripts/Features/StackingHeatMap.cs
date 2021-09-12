using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class StackingHeatMap : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    Color init;
    public Material HeatMapMat;
    public Material RegularMat;

    private bool on = false;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.activated.AddListener(ActivateHeatMap);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActivateHeatMap(ActivateEventArgs arg)
    {
        List<GameObject> stackedGes = GestureVisualizer.instance.stackedObjects;
        if (!on)
        {
            on = true;
            foreach (GameObject obj in stackedGes)
            {
                foreach (MeshRenderer mr in obj.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
                {
                    Color matColor = mr.material.color;
                    mr.material = HeatMapMat;
                    mr.material.color = matColor;
                    //mr.material.shader = Shader.Find("Custom/Overdraw");
                }
            }
            //gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        else
        {
            on = false;
            foreach (GameObject obj in stackedGes)
            {
                foreach (MeshRenderer mr in obj.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
                {
                    Color matColor = mr.material.color;
                    mr.material = RegularMat;
                    mr.material.color = matColor;
                    //mr.material.shader = Shader.Find("Standard");
                }
            }
           //gameObject.GetComponent<MeshRenderer>().material.color = init;
        }
    }
}
