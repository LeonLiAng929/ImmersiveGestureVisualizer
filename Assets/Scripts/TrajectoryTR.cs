using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryTR : MonoBehaviour
{
    private Gesture gesture;
    public List<TubeRenderer> trajectoryRenderers = new List<TubeRenderer>();

    private GameObject trajecotryGameObject;
    //private Vector3 currPos;
    //private int currPoseIndex;
    private GameObject lr;
    public GameObject skeletonRef;

    public void SetAttributes(Gesture ges, GameObject trajRef, GameObject renderer)
    {
        gesture = ges;
        trajecotryGameObject = trajRef;
        lr = renderer;
        //currPos = ges.GetCentroid();
    }

    private TubeRenderer DrawTrajectoryByJointType(Vector3[] points, string jointType, Color c)
    {
        
        GameObject newLineGen = Instantiate(lr, trajecotryGameObject.GetComponent<Transform>().Find("LineRanderers").GetComponent<Transform>());
        // Get reference to newLineGen's LineRenderer.
        TubeRenderer lRend = newLineGen.GetComponent<TubeRenderer>();

        // Set amount of LineRenderer positions = amount of line point positions.
        lRend.points = points;
 
        lRend.name = jointType;

        //set trajectory color 
        c.a = 0.9f;
        newLineGen.GetComponent<MeshRenderer>().material = GestureVisualizer.instance.tubeRendererMat;
        newLineGen.GetComponent<MeshRenderer>().material.color = c;
        //lRend.useWorldSpace = false;
        return lRend;
    }

    public void DrawTrajectory(List<Color> colorSet)
    {
        int joints_count = gesture.poses[0].num_of_joints;
        for (int j = 0; j < joints_count; j++)
        {

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < gesture.poses.Count; i++)
            {
                points.Add(gesture.poses[i].joints[j].ToVector());
            }

            trajectoryRenderers.Add(DrawTrajectoryByJointType(points.ToArray(), gesture.poses[0].joints[j].jointType, colorSet[j]));
            Debug.Log(gesture.poses[0].joints[j].jointType + j.ToString());
            //trajectoryData.Add(points.ToArray());
        }
    }
    
    /*public void UpdateTrajectoryPos()
    {
        Transform t = trajecotryGameObject.GetComponent<Transform>();
        if (currPos != t.position)
        {
            int joints_count = gesture.poses[0].num_of_joints;
            for (int j = 0; j < joints_count; j++)
            {
                Transform[] tracersTrans = trajecotryGameObject.GetComponent<Transform>().Find("Tracers").GetComponent<Transform>().Find(gesture.poses[0].joints[j].jointType).GetComponentsInChildren<Transform>();
                List<Vector3> newPos = new List<Vector3>();
                for (int i = 1; i < tracersTrans.Length; i++)
                {
                    newPos.Add(tracersTrans[i].position);

                }
                trajectoryRenderers[j].SetPositions(newPos.ToArray());
            }
        }
    }*/
    private void Update()
    {
        //UpdateTrajectoryPos();
    }
}
