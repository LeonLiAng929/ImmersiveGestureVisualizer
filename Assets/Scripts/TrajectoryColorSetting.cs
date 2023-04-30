using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryColorSetting : MonoBehaviour
{
    [SerializeField, Tooltip("HipCentre = 0, Spine = 1; ShoulderCenter =2; Head = 3; ShoulderLeft=4; ElbowLeft=5; WristLeft=6; HandLeft=7;ShoulderRight=8,ElbowRight=9,WristRight=10;HandRight=11;HipLeft=12;KneeLeft=13;AnkleLeft=14;FootLeft=15;HipRight=16;KneeRight=17;AnkleRight=18;FootRight=19")]
    public List<Color> colors = new List<Color>();
    public static TrajectoryColorSetting instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
}
