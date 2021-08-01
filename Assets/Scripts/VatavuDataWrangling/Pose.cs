using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pose 
{ 
    public double timestamp;
    public List<Joint> joints = new List<Joint>();
    public int num_of_joints;
    //public GameObject model;
}
