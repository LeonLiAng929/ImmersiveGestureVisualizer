using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TrajectoryTracer : MonoBehaviour
{
    private Trajectory t;
    
  

    private void Start()
    {
     
    }

 
    public void SetAttributes(Trajectory tra)
    {
        t = tra;
    }
    public void ResumeColour(HoverExitEventArgs interactor)
    {
        //rd.material = onExit;
    }

    public void SetColour(HoverEnterEventArgs interactor)
    {
        //rd.material = onHovered;
        //t.UpdateCurrPoseIndex(poseIndex);
        //t.UpdateSkeletonPos();
    }

    public void OnHoverEnter(HoverEnterEventArgs interactor)
    {
        //t.UpdateCurrPoseIndex(poseIndex);
        //t.UpdateSkeletonPos();
    }

    public Trajectory GetTrajectory()
    {
        return t;
    }
}
