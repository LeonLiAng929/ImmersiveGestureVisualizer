using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class FeatureTag : MonoBehaviour
{
    public Vector3 tagSize = new Vector3 (1,1,1);
    public float offsetY = 0;
    public bool focus;
    Transform user;
    XRSimpleInteractable simpleInteractable;
    public GameObject featuretagPrefab;
    private GameObject textTag;
    // Start is called before the first frame update
    void Start()
    {
        textTag= Instantiate(featuretagPrefab, gameObject.transform);
        user = Camera.main.gameObject.transform;
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        textTag.GetComponent<RectTransform>().localScale = tagSize;
        textTag.GetComponent<TextMeshPro>().text = gameObject.name;
        simpleInteractable.firstHoverEntered.AddListener(OnHovered);
        simpleInteractable.lastHoverExited.AddListener(OnHoverExit);
        textTag.SetActive(false);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (focus)
        {
            textTag.transform.LookAt(user.position);

            Quaternion temp = textTag.transform.localRotation;
            temp.y = 180;
            textTag.transform.localRotation = temp;
        }
       
        textTag.transform.position = gameObject.transform.position + new Vector3(0,offsetY,0);
    }

    public void OnHovered(HoverEnterEventArgs arg)
    {
        textTag.SetActive(true);
    }

    public void OnHoverExit(HoverExitEventArgs arg)
    {
        textTag.SetActive(false);
    }
}
