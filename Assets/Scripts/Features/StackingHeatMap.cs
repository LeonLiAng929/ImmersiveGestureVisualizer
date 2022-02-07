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
                if(!obj.GetComponent<GestureGameObject>().averageGesture)
                    obj.GetComponent<GestureGameObject>().uiRef.HeatmapIndicator.SetActive(true);
                foreach (MeshRenderer mr in obj.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>(true))
                {
                    Color matColor = mr.material.color;
                    mr.material = HeatMapMat;
                    int clusterID = obj.GetComponent<GestureGameObject>().gesture.cluster;
                    Color clusterColor = GestureVisualizer.instance.GetColorByCluster(clusterID);
                    mr.material.color = clusterColor;
                    //mr.material.shader = Shader.Find("Custom/Overdraw");
                }
            }
            selectionIndicator.SetActive(!selectionIndicator.activeSelf);
            
        }
        else
        {
            on = false;
            List<Color> colorSet = GestureVisualizer.instance.trajectoryColorSet;
            foreach (GameObject obj in stackedGes)
            {
                if (!obj.GetComponent<GestureGameObject>().averageGesture)
                    obj.GetComponent<GestureGameObject>().uiRef.HeatmapIndicator.SetActive(false);
                MeshRenderer[] mrs = obj.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>(true);
                
                for(int i =0;i< mrs.Length;i++)
                {
                    //Color matColor = mr.material.color;
                    mrs[i].material = RegularMat;
                    mrs[i].material.color = colorSet[i];
                    
                    //mr.material.shader = Shader.Find("Standard");
                }
            }
            selectionIndicator.SetActive(!selectionIndicator.activeSelf);

        }
    }
}
