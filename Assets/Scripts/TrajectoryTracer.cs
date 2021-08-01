using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TrajectoryTracer : MonoBehaviour
{
    private Trajectory t;
    private int poseIndex;
    private MeshRenderer rd;
    [SerializeField]
    public Material onHovered;
    [SerializeField]
    public Material onExit;
    XRSimpleInteractable interactable;
   

    private void Start()
    {
        rd = GetComponent<MeshRenderer>();

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.firstHoverEntered.AddListener(SetColour);
        //interactable.firstHoverEntered.AddListener(OnHoverEnter);
        interactable.lastHoverExited.AddListener(ResumeColour);
    }

 
    public void SetAttributes(int index, Trajectory tra)
    {
        poseIndex = index;
        t = tra;
    }
    public void ResumeColour(HoverExitEventArgs interactor)
    {
        //rd.material = onExit;
    }

    public void SetColour(HoverEnterEventArgs interactor)
    {
        //rd.material = onHovered;
        t.UpdateCurrPoseIndex(poseIndex);
        t.UpdateSkeletonPos();
    }

    public void OnHoverEnter(HoverEnterEventArgs interactor)
    {
        t.UpdateCurrPoseIndex(poseIndex);
        t.UpdateSkeletonPos();
    }

    public Trajectory GetTrajectory()
    {
        return t;
    }
}
