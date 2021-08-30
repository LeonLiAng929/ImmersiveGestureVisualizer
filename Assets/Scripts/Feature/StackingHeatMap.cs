using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class StackingHeatMap : MonoBehaviour
{
    [SerializeField]
    protected XRSimpleInteractable xRSimpleInteractable;
    [SerializeField]
    protected Material selected;
    [SerializeField]
    protected Material deselected;

    private bool on = false;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.activated.AddListener(ActivateHeatMap);
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
                    mr.material.shader = Shader.Find("Custom/Overdraw");
                }
            }
            gameObject.GetComponent<MeshRenderer>().material = selected;
        }
        else
        {
            on = false;
            foreach (GameObject obj in stackedGes)
            {
                foreach (MeshRenderer mr in obj.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
                {
                    mr.material.shader = Shader.Find("Standard");
                }
            }
            gameObject.GetComponent<MeshRenderer>().material = deselected;
        }
    }
}
