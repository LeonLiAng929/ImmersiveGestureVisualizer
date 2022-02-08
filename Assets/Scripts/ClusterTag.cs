using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ClusterTag : XRTag
{
    // Start is called before the first frame update
    [SerializeField]
    protected GameObject featuretagPrefab;
    protected RectTransform textTagTrans;
    protected Cluster cluster;
    void Start()
    {
        textTag = Instantiate(featuretagPrefab, gameObject.transform);
        user = Camera.main.gameObject.transform;
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        textTag.GetComponent<RectTransform>().localScale = tagSize;

        cluster = GestureAnalyser.instance.GetClusterByID(gameObject.GetComponent<ClusterGameObject>().clusterID);
        string tag2Display = gameObject.transform.parent.name + "\n" +
            "Number of gestures: " + cluster.GestureCount().ToString() + "\n" +
            "Global Consensus: " + cluster.GetSimilarity().ToString() + "\n" +
            "Local Consensus: " + cluster.GetClusterConsensus().ToString();
        textTag.GetComponent<TextMeshPro>().text = tag2Display;
        textTagTrans = textTag.GetComponent<RectTransform>();
        simpleInteractable.firstHoverEntered.AddListener(OnHovered);
        simpleInteractable.lastHoverExited.AddListener(OnHoverExit);
        textTag.SetActive(false);
    }

    public void UpdateTag()
    {
        cluster = GestureAnalyser.instance.GetClusterByID(gameObject.GetComponent<ClusterGameObject>().clusterID);
        string tag2Display = gameObject.transform.parent.name + "\n" +
           "Number of gestures: " + cluster.GestureCount().ToString() + "\n" +
           "Global Consensus: " + cluster.GetSimilarity().ToString() + "\n" +
           "Local Consensus: " + cluster.GetClusterConsensus().ToString();
        textTag.GetComponent<TextMeshPro>().text = tag2Display;
    }
    // Update is called once per frame
    void Update()
    {
        if (cluster.GestureCount() > 0)
        {
            textTagTrans.LookAt(2 * textTagTrans.position - Camera.main.gameObject.transform.position);
            textTag.transform.position = gameObject.transform.position + new Vector3(0, offsetY * Mathf.Sqrt(cluster.GestureCount()), 0);
        }
    }
}
