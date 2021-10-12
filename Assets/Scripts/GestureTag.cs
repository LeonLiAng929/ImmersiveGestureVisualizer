using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class GestureTag : XRTag
{
    // Start is called before the first frame update
    [SerializeField]
    protected GameObject featuretagPrefab;
    protected RectTransform textTagTrans;
    void Start()
    {
        if (gameObject.name != "AverageGesture" && gameObject.name != "Gesture" && gameObject.name != "Gesture(Clone)")
        {
            textTag = Instantiate(featuretagPrefab, gameObject.transform);
            //user = Camera.main.gameObject.transform;
            simpleInteractable = GetComponent<XRSimpleInteractable>();
            textTag.GetComponent<RectTransform>().localScale = tagSize;

            user = Camera.main.gameObject.transform;
            Gesture gesture = gameObject.GetComponent<GestureGameObject>().gesture;
            string tag2Display =
               "Gesture Type: " + gesture.gestureType.ToString() + "\n" +
               "UserID: " + gesture.id.ToString() + "\n" +
               "Trial: " + gesture.trial.ToString() + "\n" +
               "Cluster: " + gesture.cluster.ToString() + "\n" +
               "Number of Poses: " + gesture.num_of_poses.ToString() + "\n" +
               "Global Consensus: " + gesture.GetGlobalSimilarity().ToString() + "\n" +
               "Local Consensus: " + gesture.GetLocalSimilarity().ToString();

            textTag.GetComponent<TextMeshPro>().text = tag2Display;
            textTagTrans = textTag.GetComponent<RectTransform>();
            simpleInteractable.firstHoverEntered.AddListener(OnHovered);
            simpleInteractable.lastHoverExited.AddListener(OnHoverExit);
            textTag.SetActive(false);
        }
        else
        {
            //Destroy(this);
        }
    }

    public void UpdateTag()
    {
        if (gameObject.name != "AverageGesture")
        {
            Gesture gesture = gameObject.GetComponent<GestureGameObject>().gesture;
            string tag2Display =
               "Gesture Type: " + gesture.gestureType.ToString() + "\n" +
               "UserID: " + gesture.id.ToString() + "\n" +
               "Trial: " + gesture.trial.ToString() + "\n" +
               "Cluster: " + gesture.cluster.ToString() + "\n" +
               "Number of Poses: " + gesture.num_of_poses.ToString() + "\n" +
               "Global Consensus: " + gesture.GetGlobalSimilarity().ToString() + "\n" +
               "Local Consensus: " + gesture.GetLocalSimilarity().ToString();

            textTag.GetComponent<TextMeshPro>().text = tag2Display;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "AverageGesture" || gameObject.name == "Gesture" || gameObject.name == "Gesture(Clone)")
        {
        }
        else
        {
            textTagTrans.LookAt(2 * textTagTrans.position - Camera.main.gameObject.transform.position);
            textTag.transform.position = gameObject.transform.position + new Vector3(0, offsetY, 0);
        }
    }
}
