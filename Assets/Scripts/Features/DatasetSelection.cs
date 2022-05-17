using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DatasetSelection : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    private GameObject selectionIndicator;
    private GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(ShowMenu);
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        selectionIndicator.SetActive(false);
        menu = transform.Find("Menu").gameObject;
        menu.SetActive(false);
    }

    private void ShowMenu(SelectExitEventArgs args)
    {
        selectionIndicator.SetActive(!selectionIndicator.activeSelf);
        menu.SetActive(!menu.activeSelf);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
