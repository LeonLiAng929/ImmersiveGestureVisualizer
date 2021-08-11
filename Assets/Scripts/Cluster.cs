using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cluster
{
    private Gesture baryCentre;
    private int num_of_items;

    private float consensus; // consensus of gestures in this cluster
    private float similarity; // similarity of avg gesture of this cluster to the avg gesture of the entire dataset. 
    public int clusterID;
    private List<Gesture> gestures = new List<Gesture>();

    public void AddGesture(Gesture g, bool init = false)
    {
        if (!gestures.Contains(g))
        {
            gestures.Add(g);
            if (!init)
            {
                UpdateBarycentre();
                UpdateConsensus();
            }
        }
    }

    public void RemoveGesture(Gesture g)
    {
        if (gestures.Contains(g))
        {
            gestures.Remove(g);
            UpdateBarycentre();
            UpdateConsensus();
        }
    }

    public void UpdateBarycentre()
    {
        GestureAnalyser gestureAnalyser = GestureAnalyser.instance;
        gestureAnalyser.CalculateBaryCentre(gestures);
        baryCentre = gestureAnalyser.Python2CSharp();
        baryCentre.SetBoundingBox();
        baryCentre.SetGlobalSimilarity(gestureAnalyser.DTW_GestureWise(baryCentre, gestureAnalyser.GetGlobalAverageGesture()));
    }

    /// <summary>
    /// Based on consensus varience in GestureMap(https://arxiv.org/abs/2103.00912)
    /// Measures the DTW distance of every gesture proposal 𝑔∗
    /// for a referent 𝑅 to the computed average gesture (i.e. barycenter) g𝐷𝐵𝐴 for 𝑅. 
    /// Finally, report the variance of these DTW distances as a measure of consensus.
    /// </summary>
    /// <returns></returns>
    public void UpdateConsensus()
    {
        if (gestures.Count > 1)
        {
            GestureAnalyser gestureAnalyser = GestureAnalyser.instance;
            float sum = 0;
            foreach (Gesture g in gestures)
            {
                float newSimilarity = gestureAnalyser.DTW_GestureWise(g, baryCentre);
                g.SetLocalSimilarity(newSimilarity);
                sum += Mathf.Pow(newSimilarity, 2);
            }
            consensus = sum / gestures.Count;
        }
        else
        {
            foreach (Gesture g in gestures)
            {
                g.SetLocalSimilarity(-1);
            }
            consensus = 0;
        }
    }

    public int GestureCount()
    {
        return gestures.Count;
    }

    public List<Gesture> GetGestures()
    {
        return gestures;
    }

    public void SetBaryCentre(Gesture g)
    {
        baryCentre = g;
    }

    public Gesture GetBaryCentre()
    {
        return baryCentre;
    }

    public float GetClusterConsensus()
    {
        return consensus;
    }

    public void SetSimilarity(float s)
    {
        similarity = s;
    }

    public float GetSimilarity()
    {
        return similarity;
    }
}
