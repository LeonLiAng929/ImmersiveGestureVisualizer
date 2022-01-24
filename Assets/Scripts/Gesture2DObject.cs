using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Gesture2DObject : MonoBehaviour
{
    [HideInInspector]
    public GestureGameObject gGO;
    public GameObject AnimationIndicator;
    public GameObject SlidimationIndicator;
    public GameObject SmallmultiplesIndicator;
    public GameObject StackingIndicator;
    public GameObject ChangingClusterIndicator;
    public GameObject HeatmapIndicator;
    public GameObject ComparisonIndicator;
    public TextMesh GesInfo;
    public GameObject arrow;
    public XRSimpleInteractable xRSimpleInteractable { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(PerformAction);
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

    }

    public void PerformAction(SelectExitEventArgs arg)
    {
        Actions curr = ActionSwitcher.instance.GetCurrentAction();
        if (curr == Actions.Animate) { gGO.ActivateAnimate(); }
        else if (curr == Actions.ChangeCluster) { gGO.ChangeCluster(); }
        else if (curr == Actions.ShowSmallMultiples) { gGO.ShowSmallMultiples(); }
        else if (curr == Actions.Slidimation)
        {
            gGO.ActivateSwinging();
            GestureVisualizer.instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out gGO.lastQuat);
        }
        else if (curr == Actions.StackGestures) { gGO.StackGesture(); }
        else if (curr == Actions.CloseComparison)
        {
            if (arg.interactor.gameObject.name == "LeftHand Controller")
                gGO.CloseComparison(true);
            else if (arg.interactor.gameObject.name == "RightHand Controller")
            {
                gGO.CloseComparison(false);
            }
        }
        else if (curr == Actions.Idle)
        {
            arrow.SetActive(!arrow.activeSelf);
            gGO.IdleSelect();
        }
    }

    private void OnDestroy()
    {

        xRSimpleInteractable.selectExited.RemoveAllListeners();
    }
}
