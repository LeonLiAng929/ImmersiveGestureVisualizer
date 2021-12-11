using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DisplayTrajectoryFilter : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;
    public GameObject filter;
    Color init;
    private GameObject selectionIndicator;
    // Start is called before the first frame update
    void Start()
    {
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.selectExited.AddListener(ShowFilter);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
        selectionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowFilter(SelectExitEventArgs arg)
    {
        if (filter.activeSelf)
        {
            filter.SetActive(!filter.activeSelf);
            gameObject.GetComponent<MeshRenderer>().material.color = init;
            selectionIndicator.SetActive(!selectionIndicator.activeSelf);
        }
        else {
            //filter.gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, 0.55f, Camera.main.gameObject.transform.position.z + 2);
            filter.SetActive(!filter.activeSelf);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            selectionIndicator.SetActive(!selectionIndicator.activeSelf);
        }
    }
}
