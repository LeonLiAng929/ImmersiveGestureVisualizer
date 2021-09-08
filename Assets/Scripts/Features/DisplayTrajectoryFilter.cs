using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DisplayTrajectoryFilter : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;
    public GameObject filter;
    Color init;
    // Start is called before the first frame update
    void Start()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.activated.AddListener(ShowFilter);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowFilter(ActivateEventArgs arg)
    {
        if (filter.activeSelf)
        {
            filter.SetActive(!filter.activeSelf);
            gameObject.GetComponent<MeshRenderer>().material.color = init;
        }
        else {
            filter.gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, 0.55f, Camera.main.gameObject.transform.position.z + 2);
            filter.SetActive(!filter.activeSelf);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            }
    }
}
