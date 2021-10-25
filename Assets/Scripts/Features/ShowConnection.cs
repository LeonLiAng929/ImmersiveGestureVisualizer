using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowConnection : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3[] path = new Vector3[2];
    private int currArrangement;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<LineRenderer>();
        lr = GetComponent<LineRenderer>();
        lr.SetWidth(0.1f, 0.1f);
        lr.material = new Material(Shader.Find("Standard"));
        path[0] = transform.Find("Capsule").position;
        path[1] = transform.parent.Find("AverageGesture").Find("Capsule").position;
        path[0] -= new Vector3(0, path[0].y, 0);
        path[1] -= new Vector3(0, path[1].y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (GestureVisualizer.instance.arrangementMode != currArrangement)
        {
            currArrangement = GestureVisualizer.instance.arrangementMode;
            path[0] = transform.Find("Capsule").position;
            path[1] = transform.parent.Find("AverageGesture").Find("Capsule").position;
            path[0] -= new Vector3(0, path[0].y, 0);
            path[1] -= new Vector3(0, path[1].y, 0);
            lr.material.color = transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color;
            lr.SetPositions(path);
        }
        
    }
}
