using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResumeFromComparison : MonoBehaviour
{
    private Color init;
    private XRSimpleInteractable xRSimpleInteractable;
    private bool comparison = true;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Resume);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }

    private void Resume(SelectExitEventArgs arg)
    {
        if (comparison)
        {
            comparison = false;
            gameObject.GetComponent<MeshRenderer>().material.color = init;
            foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.GetClusterObjs())
            {
                foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
                {
                    gGO.gameObject.SetActive(true);
                }
            }
            GestureVisualizer.instance.leftHandSelected = false;
            GestureVisualizer.instance.rightHandSelected = false;
            foreach (GameObject g in GestureVisualizer.instance.selectedGestures)
            {
                g.GetComponent<GestureGameObject>().RevertComparing();
            }
            GestureVisualizer.instance.EmptySelected();
        }
        else
        {
            comparison = true;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

        }
    }
}
