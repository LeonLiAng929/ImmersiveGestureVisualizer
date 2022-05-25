using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Gesture2DObject : MonoBehaviour
{
    [HideInInspector]
    public GestureGameObject gGO = null;
    public GameObject AnimationIndicator;
    public GameObject SlidimationIndicator;
    public GameObject SmallmultiplesIndicator;
    public GameObject StackingIndicator;
    public GameObject ChangingClusterIndicator;
    public GameObject HeatmapIndicator;
    public GameObject ComparisonIndicator;
    public TextMesh GesInfo;
    public GameObject arrow;

    public GameObject TwoDModel;
    
    private Transform preview = null;
    
    public XRSimpleInteractable xRSimpleInteractable { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(PerformAction);
        xRSimpleInteractable.activated.AddListener(TeleportToObject);
        //xRSimpleInteractable.firstHoverEntered.AddListener(PreviewGesture);
        //xRSimpleInteractable.lastHoverExited.AddListener(DestroyPreviewGesture);
        AnimationIndicator.SetActive(false);
        SlidimationIndicator.SetActive(false);
        SmallmultiplesIndicator.SetActive(false);
        StackingIndicator.SetActive(false);
        ChangingClusterIndicator.SetActive(false);
        HeatmapIndicator.SetActive(false);
        ComparisonIndicator.SetActive(false);
        arrow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        GesInfo.color = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    public void Initialize()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(PerformAction);
        //xRSimpleInteractable.firstHoverEntered.AddListener(PreviewGesture);

    }

    public void Initialize2DGesture()
    {
        Gesture TwoDimensionalGesture = gGO.gesture;
        MeshRenderer[] transforms = TwoDModel.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < transforms.Length; i++)
        {
            Vector3 pos = TwoDimensionalGesture.poses[0].joints[i].ToVector();

            transforms[i].transform.localPosition = pos - new Vector3(0,0,pos.z);
        }
        MeshRenderer[] mrs = TwoDModel.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in mrs)
        {
            mr.material.color = GestureVisualizer.instance.GetColorByCluster(gGO.gesture.cluster);
        }
    }
    public void TeleportToObject(ActivateEventArgs args)
    {
        GameObject rig = Deploy.instance.XRRig;
        rig.transform.localPosition = gGO.gameObject.transform.localPosition - Vector3.forward;
        rig.transform.localRotation = gGO.gameObject.transform.localRotation;
    }
    public void PreviewGesture(HoverEnterEventArgs args)
    {
        if (gGO != null)
        {
            preview = Instantiate(gGO.gameObject.transform, transform);
            preview.gameObject.name = "Preview";
            preview.localPosition = transform.localPosition;
            preview.localScale = transform.localScale;
            preview.localRotation = transform.localRotation;
            preview.GetComponent<GestureGameObject>().ActivateAnimate(false);
            preview.Rotate(new Vector3(0, 0, 1));
            gameObject.SetActive(false);
        }
    }

    public void DestroyPreviewGesture(HoverExitEventArgs args)
    {
        if (gGO != null && preview != null)
        {
            Destroy(preview.gameObject);
            gameObject.SetActive(true);
        }
    }

    public void PerformAction(SelectExitEventArgs arg)
    {
        Actions curr = ActionSwitcher.instance.GetCurrentAction();
        if (curr == Actions.Animate) { gGO.ActivateAnimate(false); }
        else if (curr == Actions.ChangeCluster) { gGO.ChangeCluster(); }
        else if (curr == Actions.SmallMultiples) { gGO.ShowSmallMultiples(false); }
        else if (curr == Actions.Swing)
        {
            gGO.ActivateSwinging(false);
            GestureVisualizer.instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out gGO.lastQuat);
        }
        else if (curr == Actions.StackGestures) { gGO.StackGesture(false); }
        else if (curr == Actions.CloseComparison)
        {
            if (arg.interactor.gameObject.name == "LeftHand Controller")
                gGO.CloseComparison(true);
            else if (arg.interactor.gameObject.name == "RightHand Controller")
            {
                gGO.CloseComparison(false);
            }
        }
        else if (curr == Actions.Mark)
        {
            //arrow.SetActive(!arrow.activeSelf);
            gGO.IdleSelect();
        }
    }

    private void OnDestroy()
    {

        xRSimpleInteractable.selectExited.RemoveAllListeners();
    }
}
