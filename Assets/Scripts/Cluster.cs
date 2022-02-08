using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    private Gesture baryCentre = null;
    private float consensus = -1; // consensus of gestures in this cluster
    private float similarity = -1; // similarity of avg gesture of this cluster to the avg gesture of the entire dataset. 
    public int clusterID;
    private List<Gesture> gestures = new List<Gesture>();

    public void SetBaryCentrePCA_Coord(Vector2 coord)
    {
        baryCentre.PCA_Coordinate = coord;
    }

    public void SetBaryCentreMDS_Coord(Vector2 coord)
    {
        baryCentre.MDS_Coordinate = coord;
    }
    public void AddGesture(List<Gesture> gestureList, bool init = false)
    {
        foreach (Gesture g in gestureList)
        {
            if (!gestures.Contains(g))
            {
                g.cluster = clusterID;
                gestures.Add(g);
            }
        }
        if (!init)
        {
            if (gestures.Count > 1)
            {
                UpdateBarycentre();
                UpdateConsensus();
            }
            else if (gestures.Count == 1)
            {
                SetBaryCentre(GetGestures()[0]);
                UpdateConsensus();
            }
            GestureAnalyser.instance.PCA_Arrangement();
            GestureAnalyser.instance.MDS_Arrangement();
        }
    }

    public void RemoveGesture(List<Gesture> gLi)
    {
        foreach (Gesture g in gLi)
        {
            if (gestures.Contains(g))
            {
                gestures.Remove(g);
            }
        }
        if (gestures.Count > 1)
        {
            UpdateBarycentre();
            UpdateConsensus();
        }
        else if (gestures.Count == 1)
        {
            SetBaryCentre(GetGestures()[0]);
            UpdateConsensus();
        }
        else
        {
            // destroy the cluster
            DecrementCluster.instance._Decrement();
            GestureVisualizer.instance.freeId.Add(clusterID);
            GestureVisualizer.instance.DestroyClusterObjectById(clusterID);
            baryCentre = null;
            consensus = -1;

        }
        GestureAnalyser.instance.PCA_Arrangement();
        GestureAnalyser.instance.MDS_Arrangement();
    }

    public void UpdateBarycentre()
    {
        GestureAnalyser gestureAnalyser = GestureAnalyser.instance;
        gestureAnalyser.CalculateBaryCentre(gestures);
        baryCentre = gestureAnalyser.Python2CSharp();
        baryCentre.SetBoundingBox();
        baryCentre.cluster = clusterID;
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
        if (baryCentre == null)
        {
            return similarity;
        }
        else 
            return baryCentre.GetGlobalSimilarity();
    }

}
