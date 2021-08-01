using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cluster
{
    private Gesture baryCentre;
    private int num_of_items;
    //public GameObject visualrepr;

    public int clusterID;
    private List<Gesture> gestures = new List<Gesture>();

    public void AddGesture(Gesture g)
    {
        if (!gestures.Contains(g))
        {
            gestures.Add(g);
        }
    }
    
    public void RemoveGesture(Gesture g)
    {
        if (gestures.Contains(g))
        {
            gestures.Remove(g);
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


}
