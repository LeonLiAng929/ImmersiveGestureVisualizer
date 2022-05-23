using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DisplayMap : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;
    public GameObject map;
    Color init;
    private GameObject selectionIndicator;
    // Start is called before the first frame update
    void Start()
    {
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.selectExited.AddListener(ShowMap);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
        map.SetActive(false);
        selectionIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMap(SelectExitEventArgs arg)
    {
        if (map.activeSelf)
        {
            map.SetActive(!map.activeSelf);
            selectionIndicator.SetActive(!selectionIndicator.activeSelf);
            gameObject.GetComponent<MeshRenderer>().material.color = init;
        }
        else {
            //filter.gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, 0.55f, Camera.main.gameObject.transform.position.z + 2);
            UserStudy.instance.IncrementCount(Actions.OverviewMap);
            map.SetActive(!map.activeSelf);
            selectionIndicator.SetActive(!selectionIndicator.activeSelf);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            }
    }
}
