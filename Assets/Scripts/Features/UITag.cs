using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class UITag : XRTag
{
    //public Vector3 tagSize = new Vector3 (1,1,1);
    //public float offsetY = 0;
    //public bool focus;
    //Transform user;
    //XRSimpleInteractable simpleInteractable;
    //public GameObject featuretagPrefab;
    // private GameObject textTag;
    // Start is called before the first frame update
    [SerializeField]
    protected GameObject featuretagPrefab;
    void Start()
    {
        textTag= Instantiate(featuretagPrefab, gameObject.transform);
        user = Camera.main.gameObject.transform;
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        textTag.GetComponent<RectTransform>().localScale = tagSize;
        textTag.GetComponent<TextMeshPro>().text = gameObject.name;
        //simpleInteractable.firstHoverEntered.AddListener(OnHovered);
        //simpleInteractable.lastHoverExited.AddListener(OnHoverExit);
        textTag.SetActive(true);
        textTag.transform.localPosition = new Vector3(-0.01f, 0.48f, 0);
        textTag.transform.Rotate(new Vector3(0, 180, 0), Space.Self);

    }
    

    // Update is called once per frame
    void Update()
    {
        //textTag.transform.LookAt(Camera.main.transform, -1*textTag.transform.up);

        //textTag.transform.localPosition = textTag.transform.up.normalized * offsetY;
    }
}
