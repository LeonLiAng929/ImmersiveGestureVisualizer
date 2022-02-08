using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class NewCluster : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    Transform mainCam;

    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(CreateNewCluster);
        mainCam = Camera.main.transform;
    }

    public void CreateNewCluster(SelectExitEventArgs args)
    {

        GameObject newCluster = GestureVisualizer.instance.InstantiateNewCluster();
        newCluster.transform.position = mainCam.position;
        newCluster.transform.position += mainCam.forward;
        Vector3 pos = newCluster.transform.localPosition;
        newCluster.transform.localPosition = new Vector3(pos.x, 0.55f, pos.z);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
