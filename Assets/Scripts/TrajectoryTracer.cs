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
}
