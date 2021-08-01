using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint
{
    public string jointType;
    public float x;
    public float y;
    public float z;

    public Joint() { }
    public Joint(float X, float Y, float Z, string type)
    {
        x = X;
        y = Y;
        z = Z;
        jointType = type;
    }
    public float[] ToArray()
    {
        float[] a = { x, y, z };
        return a;
    }

    public Vector3 ToVector()
    {
        Vector3 a = new Vector3(x, y, z);
        return a;
    }
}
