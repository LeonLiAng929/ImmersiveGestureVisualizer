using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Deploy : MonoBehaviour
{
    public GameObject XRRig;
    public GameObject avatar;
    protected XRSimpleInteractable xRSimpleInteractable;
    private GameObject selectionIndicator;


    private Vector3 previousLocation = new Vector3(0, 0, 0);
    #region Singleton
    public static Deploy instance;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(DeployRig);
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        selectionIndicator.SetActive(false);
  

    }

    // Update is called once per frame
    void Update()
    {
        if (XRRig.transform.localEulerAngles.x == 90)
        {
            if (!selectionIndicator.activeSelf)
            {
                selectionIndicator.SetActive(true);
                foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.clustersObjDic)
                {
                    foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
                    {
                        gGO.gameObject.transform.Find("Capsule").localRotation = Quaternion.Euler(90, 0, 0);
                        gGO.gameObject.transform.Find("Trajectory").localRotation = Quaternion.Euler(90, 0, 0);
                    }
                }
            }
            if (avatar.activeSelf)
                avatar.SetActive(false);
        }
        else
        {
            if (selectionIndicator.activeSelf)
            {
                selectionIndicator.SetActive(false);
                foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.clustersObjDic)
                {
                    foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
                    {
                        gGO.gameObject.transform.Find("Capsule").localRotation = Quaternion.Euler(0, 0, 0);
                        gGO.gameObject.transform.Find("Trajectory").localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
            if (!avatar.activeSelf)
                avatar.SetActive(true);
        }
    }

    public void DeployRig(SelectExitEventArgs args)
    {
        if (XRRig.transform.localEulerAngles.x == 90)
        {
            XRRig.transform.localPosition = previousLocation;
            XRRig.transform.localRotation = Quaternion.Euler(0, 0, 0);
            foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.clustersObjDic)
            {
                foreach(GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
                {
                    gGO.gameObject.transform.Find("Capsule").localRotation = Quaternion.Euler(0, 0, 0);
                    gGO.gameObject.transform.Find("Trajectory").localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
        else
        {
            previousLocation = XRRig.transform.localPosition;
            XRRig.transform.localPosition = new Vector3(0, 20, 0);
            XRRig.transform.localRotation = Quaternion.Euler(90, 0, 0);
            foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.clustersObjDic)
            {
                foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
                {
                    gGO.gameObject.transform.Find("Capsule").localRotation = Quaternion.Euler(90, 0, 0);
                    gGO.gameObject.transform.Find("Trajectory").localRotation = Quaternion.Euler(90, 0, 0);
                }
            }
        }
    }

    public void _DeployRig()
    {
        XRRig.transform.localPosition = new Vector3(0, 20, 0);
        XRRig.transform.localRotation = Quaternion.Euler(90, 0, 0);
        foreach (KeyValuePair<int, GameObject> pair in GestureVisualizer.instance.clustersObjDic)
        {
            foreach (GestureGameObject gGO in pair.Value.GetComponentsInChildren<GestureGameObject>(true))
            {
                gGO.gameObject.transform.Find("Capsule").localRotation = Quaternion.Euler(90, 0, 0);
                gGO.gameObject.transform.Find("Trajectory").localRotation = Quaternion.Euler(90, 0, 0);
            }
        }
    }
  
    public bool IsDeploying()
    {
        if (XRRig.transform.localEulerAngles.x == 90)
        { return true; }
        else
        {
           return false;
        }
    }
}
