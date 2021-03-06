using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    //private static List<Color> colors = new List<Color> {new Color(137, 49, 239), new Color(255, 0, 189), new Color(0, 87, 233), new Color(135, 233, 17), new Color(225, 24, 69) };
    private Gesture gesture;
    public List<LineRenderer> trajectoryRenderers = new List<LineRenderer>();
    //private List<Vector3[]> trajectoryData = new List<Vector3[]>();
    private GameObject trajecotryGameObject;
    private GameObject tracerPrefab;
    private GameObject lr;
    //private Transform TrajectoryRendererContainer;
    private Vector3 currPos;
    private int currPoseIndex;
    public GameObject skeletonRef;

    public void SetAttributes(Gesture ges, GameObject trajRef, GameObject renderer, GameObject tracer)
    {
        gesture = ges;
        lr = renderer;
        lr.GetComponent<LineRenderer>().SetWidth(0.04f, 0.04f);
        trajecotryGameObject = trajRef;
        //TrajectoryRendererContainer = container;
        currPos = ges.GetCentroid();
        tracerPrefab = tracer;
    }
    
    private LineRenderer DrawTrajectoryByJointType(Vector3[] points, string jointType, Color c)
    {
        //trajectoryReference.GetComponent<Transform>().Find("LineRanderers");
        GameObject newLineGen = Instantiate(lr, trajecotryGameObject.GetComponent<Transform>().Find("LineRanderers").GetComponent<Transform>());
        // Get reference to newLineGen's LineRenderer.
        LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();
        
        // Set amount of LineRenderer positions = amount of line point positions.
        lRend.positionCount = points.Length;
        // Set positions of LineRenderer using linePoints array.
        lRend.SetPositions(points);

        lRend.name = jointType;

        //set trajectory color 
    
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(c, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0f), new GradientAlphaKey(alpha, 1.0f) }
        );
          
        lRend.colorGradient = gradient;
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
                
                // instantiate tracer
                GameObject newTracer = Instantiate(tracerPrefab, trajecotryGameObject.GetComponent<Transform>().Find("Tracers").GetComponent<Transform>().Find(gesture.poses[0].joints[j].jointType));
                //newTracer.GetComponent<MeshRenderer>().material.color = colorSet[j];
                Transform tracerTrans = newTracer.GetComponent<Transform>();
                tracerTrans.localPosition = gesture.poses[i].joints[j].ToVector();
                newTracer.GetComponent<TrajectoryTracer>().SetAttributes(this);
            }

            trajectoryRenderers.Add(DrawTrajectoryByJointType(points.ToArray(), gesture.poses[0].joints[j].jointType, colorSet[j]));

            //trajectoryData.Add(points.ToArray());
        }
    }
/*
    public void IncrementCurrPoseIndex()
    {
        currPoseIndex += 1;
    }

    public void DecrementCurrPoseIndex()
    {
        currPoseIndex -= 1;
    }

    public int GetCurrPoseIndex()
    {
        return currPoseIndex;
    }
    public void UpdateSkeletonPos()
    {
        Transform[] transforms = skeletonRef.GetComponentsInChildren<Transform>();
        for (int i = 1; i < 21; i++)
        {
            transforms[i].localPosition = gesture.poses[currPoseIndex].joints[i-1].ToVector();
        }
    }*/

    public void UpdateTrajectoryPos()
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
    }
    private void Update()
    {
        UpdateTrajectoryPos();
    }
}
