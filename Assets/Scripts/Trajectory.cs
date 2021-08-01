using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    private static List<Color> colors = new List<Color> {new Color(137, 49, 239), new Color(255, 0, 189), new Color(0, 87, 233), new Color(135, 233, 17), new Color(225, 24, 69) };
    private Gesture gesture;
    public List<LineRenderer> trajectoryRenderers = new List<LineRenderer>();
    private List<Vector3[]> trajectoryData = new List<Vector3[]>();
    private GameObject trajectoryReference;
    private GameObject tracerRef;
    private GameObject lr;
    //private Transform TrajectoryRendererContainer;
    private Vector3 currPos;
    private int currPoseIndex;
    public GameObject skeletonRef;

    

    /*public Trajectory(Gesture ges, GameObject trajRef, GameObject renderer, Transform container)
    {
        gesture = ges;
        lr = renderer;
        trajectoryReference = trajRef;
        TrajectoryRendererContainer = container;
        currPos = ges.GetCentroid();
    }*/

    public void SetAttributes(Gesture ges, GameObject trajRef, GameObject renderer, GameObject tracer)
    {
        gesture = ges;
        lr = renderer;
        lr.GetComponent<LineRenderer>().SetWidth(0.01f, 0.01f);
        trajectoryReference = trajRef;
        //TrajectoryRendererContainer = container;
        currPos = ges.GetCentroid();
        tracerRef = tracer;
    }
    
    private LineRenderer DrawTrajectoryByJointType(Vector3[] points, string jointType, Color c)
    {
        //trajectoryReference.GetComponent<Transform>().Find("LineRanderers");
        GameObject newLineGen = Instantiate(lr, trajectoryReference.GetComponent<Transform>().Find("LineRanderers").GetComponent<Transform>());
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
            new GradientColorKey[] { new GradientColorKey(Color.Lerp(c, Color.white, 0.5f), 0.0f), new GradientColorKey(Color.Lerp(c, Color.black, 0.5f), 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
          
        lRend.colorGradient = gradient;
          
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
                GameObject newTracer = Instantiate(tracerRef, trajectoryReference.GetComponent<Transform>().Find("Tracers").GetComponent<Transform>());
                newTracer.GetComponent<MeshRenderer>().material.color = colorSet[j];
                Transform tracerTrans = newTracer.GetComponent<Transform>();
                tracerTrans.localPosition = gesture.poses[i].joints[j].ToVector();
                newTracer.GetComponent<TrajectoryTracer>().SetAttributes(i, this);

                //Debug.Log(points.ToArray());
            }

            trajectoryRenderers.Add(DrawTrajectoryByJointType(points.ToArray(), gesture.poses[0].joints[j].jointType, colorSet[j]));

            trajectoryData.Add(points.ToArray());
        }
    }

    public void UpdateCurrPoseIndex(int x)
    {
        currPoseIndex = x;
    }
    public void UpdateSkeletonPos()
    {
        Transform[] transforms = skeletonRef.GetComponentsInChildren<Transform>();
        for (int i = 1; i < 21; i++)
        {
            //transforms[i].localPosition = trajectoryRenderers[i - 1].GetPosition(currPoseIndex);
            transforms[i].localPosition = gesture.poses[currPoseIndex].joints[i-1].ToVector();
        }
    }

    public void UpdateTrajectoryPos()
    {
        Transform t = trajectoryReference.GetComponent<Transform>();
        if (currPos != t.position)
        {
            Vector3 pos = currPos - t.position;
            currPos = t.position;
            for (int i = 0; i < trajectoryRenderers.Count; i++)
            {
                Vector3[] newpos = trajectoryData[i];
                for (int j = 0; j < newpos.Length; j++)
                {
                    newpos[j] -= pos;
                }
                trajectoryRenderers[i].SetPositions(newpos);
            }
        }
    }

}
