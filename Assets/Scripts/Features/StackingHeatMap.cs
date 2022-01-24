using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class StackingHeatMap : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    public Material HeatMapMat;
    public Material RegularMat;
    private GameObject selectionIndicator;
    private bool on = false;
    // Start is called before the first frame update
    void Start()
    {
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(ActivateHeatMap);
        selectionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActivateHeatMap(SelectExitEventArgs arg)
    {
        List<GameObject> stackedGes = GestureVisualizer.instance.stackedObjects;
        if (!on)
        {
            on = true;
            foreach (GameObject obj in stackedGes)
            {
                obj.GetComponent<GestureGameObject>().uiRef.HeatmapIndicator.SetActive(true);
                foreach (MeshRenderer mr in obj.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>(true))
                {
                    Color matColor = mr.material.color;
                    mr.material = HeatMapMat;
                    mr.material.color = matColor;
                    //mr.material.shader = Shader.Find("Custom/Overdraw");
                }
            }
            selectionIndicator.SetActive(!selectionIndicator.activeSelf);
            
        }
        else
        {
            on = false;
            foreach (GameObject obj in stackedGes)
            {
                obj.GetComponent<GestureGameObject>().uiRef.HeatmapIndicator.SetActive(false);
                foreach (MeshRenderer mr in obj.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>(true))
                {
                    Color matColor = mr.material.color;
                    mr.material = RegularMat;
                    mr.material.color = matColor;
                    //mr.material.shader = Shader.Find("Standard");
                }
            }
            selectionIndicator.SetActive(!selectionIndicator.activeSelf);

        }
    }
}
