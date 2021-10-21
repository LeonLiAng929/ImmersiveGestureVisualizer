using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Search : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;
    public float tolerance;
    Color init;
    // Start is called before the first frame update
    void Start()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.activated.AddListener(ConfirmSearch);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    public void ConfirmSearch(ActivateEventArgs arg)
    {
        Debug.Log(GestureVisualizer.instance.proposedGes.poses.Count);
        List<Gesture> gestures = GestureAnalyser.instance.PrepareGestureForSearch();
        Gesture proposedGes = GestureVisualizer.instance.proposedGes;
        proposedGes.num_of_poses = proposedGes.poses.Count;
        proposedGes.SetBoundingBox();
        proposedGes.SetCentroid();
        proposedGes.TranslateToOrigin();
        proposedGes.NormalizeHeight();
        proposedGes.NormalizeTimestamp();
        List<Gesture> unsortedResult = new List<Gesture>();
        foreach(Gesture g in gestures)
        {
            float variance = GestureAnalyser.instance.DTW_GestureWise(proposedGes, g);
            if (variance <= tolerance)
            {
                unsortedResult.Add(g);
            }
        }
        List<Gesture> sortedResult = new List<Gesture>();
        while (unsortedResult.Count > 0)
        {
            float min = float.PositiveInfinity;
            Gesture minGes = null;
            foreach (Gesture g in unsortedResult)
            {
                float v = GestureAnalyser.instance.DTW_GestureWise(proposedGes, g);
                if (v < min)
                {
                    min = v;
                    minGes = g;
                }
            }
            sortedResult.Add(minGes);
            unsortedResult.Remove(minGes);
        }

        GestureVisualizer.instance.ShowSearchResult(sortedResult);
        //Debug.Log(sortedResult.Count);
    }
}