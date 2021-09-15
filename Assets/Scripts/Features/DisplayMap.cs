using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DisplayMap : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;
    public GameObject map;
    Color init;
    // Start is called before the first frame update
    void Start()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.activated.AddListener(ShowMap);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
        map.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMap(ActivateEventArgs arg)
    {
        if (map.activeSelf)
        {
            map.SetActive(!map.activeSelf);
            gameObject.GetComponent<MeshRenderer>().material.color = init;
        }
        else {
            //filter.gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, 0.55f, Camera.main.gameObject.transform.position.z + 2);
            map.SetActive(!map.activeSelf);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            }
    }
}
