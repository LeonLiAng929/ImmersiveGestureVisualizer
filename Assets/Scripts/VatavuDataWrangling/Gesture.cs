using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gesture
{
    public string gestureType;
    public List<Pose> poses = new List<Pose>();
    public int num_of_poses;
    public int id;
    public char trial;
    public int cluster;
    public Vector2 PCA_Coordinate;
    public Vector2 MDS_Coordinate;

    private float localSimilarity; // DTW similarity score between this gesture and the average gesture of the cluster it belongs to.
    private float globalSimilarity; // DTW similarity score between this gesture and the average gesture of the entire dataset.
    private Vector3 rescaleReference = new Vector3(1, 1, 1);
    private BoundingBox boundingBox;
    private Vector3 centroid;

    public class BoundingBox
    {
        public float maxX = float.NegativeInfinity;
        public float minX = float.PositiveInfinity;
        public float maxY = float.NegativeInfinity;
        public float minY = float.PositiveInfinity;
        public float maxZ = float.NegativeInfinity;
        public float minZ = float.PositiveInfinity;
    }

    /// <summary>
    /// Calculates the bounding box based on data in .csv file.
    /// </summary>
    public void SetBoundingBox()
    {
        boundingBox = new BoundingBox();
        foreach(Pose p in poses) {
            foreach (Joint d in p.joints)
            {
                Vector3 coordinate = d.ToVector();
               
                if (coordinate.x > boundingBox.maxX)
                {
                    boundingBox.maxX = coordinate.x;
                }
                if (coordinate.x < boundingBox.minX)
                {
                    boundingBox.minX = coordinate.x;
                }
                if (coordinate.y > boundingBox.maxY)
                {
                    boundingBox.maxY = coordinate.y;
                }
                if (coordinate.y < boundingBox.minY)
                {
                    boundingBox.minY = coordinate.y;
                }
                if (coordinate.z > boundingBox.maxZ)
                {
                    boundingBox.maxZ = coordinate.z;
                }
                if (coordinate.z < boundingBox.minZ)
                {
                    boundingBox.minZ = coordinate.z;
                }
              
            }
        }
    }

    /// <summary>
    /// Returns the calculated bounding box 
    /// </summary>
    public BoundingBox GetBoundingBox()
    {
        return boundingBox;
    }

    /// <summary>
    /// Calculates and returns the size of the bounding box.
    /// </summary>
    public Vector3 GetBoundingBoxSize()
    {
        return new Vector3(boundingBox.maxX - boundingBox.minX, boundingBox.maxY - boundingBox.minY, boundingBox.maxZ - boundingBox.minZ);
    }

    /// <summary>
    /// Returns centroid of this recorded gesture
    /// </summary>
    public Vector3 GetCentroid()
    {
        return centroid;
    }

    /// <summary>
    /// Calculates the centroid for the recorded gesture based on its bounding box
    /// </summary>
    public void SetCentroid()
    {
        Vector3 size = GetBoundingBoxSize();
        centroid = new Vector3(size.x / 2 + boundingBox.minX, size.y / 2 + boundingBox.minY, size.z / 2 + boundingBox.minZ);
    }

    /// <summary>
    /// Returns the time duration of this gesture.
    /// </summary>
    public double GetProductionTimeInMilliseconds()
    {
        return (poses.Count == 0) ? 0 : poses[poses.Count - 1].timestamp - poses[0].timestamp;
    }

    /// <summary>
    /// Resamples a whole-body gesture into a fixed number of n body poses uniformly spaced in time.
    /// </summary>
    public Gesture Resample(int n)
    {
        if (this.poses.Count == 0)
            return null;

        List<Pose> set = new List<Pose>();
        double I = this.GetProductionTimeInMilliseconds() / (n - 1);

        set.Add(poses[0]);
        for (int i = 1; i < poses.Count; i++)
        {
            double timeDiff = poses[i].timestamp - set[set.Count - 1].timestamp;
            while (timeDiff >= I)
            {
                // interpolate two body postures
                double t = I / timeDiff;
                Pose posture = new Pose();
                posture.num_of_joints = poses[i].joints.Count;
                for (int j = 0; j < poses[i].joints.Count; j++)
                    posture.joints.Add(new Joint(float.Parse(((1 - t) * set[set.Count - 1].joints[j].x + t * poses[i].joints[j].x).ToString()),
                        float.Parse(((1 - t) * set[set.Count - 1].joints[j].y + t * poses[i].joints[j].y).ToString()),
                        float.Parse(((1 - t) * set[set.Count - 1].joints[j].z + t * poses[i].joints[j].z).ToString()),
                        set[set.Count - 1].joints[j].jointType));

                posture.timestamp = (1 - t) * set[set.Count - 1].timestamp + t * poses[i].timestamp;
                set.Add(posture);
                timeDiff -= I;
            }
        }
        if (set.Count == n - 1)
            set.Add(poses[poses.Count - 1]);

        Gesture g = new Gesture();
        g.MakeEqualTo(this);
        g.num_of_poses = n;
        g.poses = set;
        return g;
    }


    /// <summary>
    /// Translates the gesture so that its centroid becomes (0, 0, 0).
    /// </summary>
    public void TranslateToOrigin()
    {
        foreach (Pose pose in poses)
            foreach (Joint joint in pose.joints)
            {
                joint.x -= centroid.x;
                joint.y -= centroid.y;
                joint.z -= centroid.z;
            }
        SetBoundingBox();
        SetCentroid();
    }

    /// <summary>
    /// Rescales the recorded gesture based on rescaleReference.
    /// </summary>
    public void NormalizeHeight()
    {
        Vector3 size = GetBoundingBoxSize();
        //float xScale = rescaleReference.x / size.x;
        float yScale = rescaleReference.y / size.y;
        //float zScale = size.z / rescaleReference.z;

        foreach (Pose pose in poses)
        {
            foreach (Joint joint in pose.joints)
            {
               // joint.x = joint.x * xScale;
                joint.y = joint.y * yScale;
                //joint.z = joint.z * zScale;
            }
        }
        SetBoundingBox();
        SetCentroid();
    }

    public void NormalizeTimestamp()
    {
       
        try
        {
            var benchm = poses[0].timestamp;
        }
        catch (System.ArgumentOutOfRangeException) { Debug.Log(gestureType + id.ToString() +" " +trial.ToString()); }
        var benchmark = poses[0].timestamp;
        foreach (Pose pose in poses)
        {
            pose.timestamp = pose.timestamp - benchmark;
        }
    }

    public float[][] ToTimeSeriesData()
    {
        float[][] series = new float[num_of_poses][];
        for (int i = 0; i < num_of_poses; i++)
        {
            float[] serie = new float[poses[i].num_of_joints];
            foreach(Joint joint in poses[i].joints)
            {
                serie = serie.Concat(joint.ToArray()).ToArray();
            }
            series[i] = serie;
           
        }
        return series;
    }

    public void MakeEqualTo(Gesture g)
    {
        gestureType = g.gestureType;
        poses = g.poses;
        num_of_poses = g.num_of_poses;
        id = g.id;
        cluster = g.cluster;
        boundingBox = g.GetBoundingBox();
        centroid = g.GetCentroid();

    }

    /// <summary>
    /// Wrangle gestural dataset in poses for python
    /// </summary>
    /// <returns></returns>
   public List<List<float>> DataWranglingForPython()
    {
        List<List<float>> series = new List<List<float>>();
        for (int i = 0; i < num_of_poses; i++)
        {
            List<float> serie = new List<float>();
            foreach(Joint j in poses[i].joints)
            {
                serie.Add(j.x);
                serie.Add(j.y);
                serie.Add(j.z);
            }
            series.Add(serie);
        }
        return series;
    }

    public List<float> DataFlatteningForPCA()
    {
        List<float> li = new List<float>();
        for (int i = 0; i < num_of_poses; i++)
        {
            foreach (Joint j in poses[i].joints)
            {
                li.Add(j.x);
                li.Add(j.y);
                li.Add(j.z);
            }
        }
        return li;
    }

    public void SetLocalSimilarity(float s)
    {
        localSimilarity = s;
    }

    public float GetLocalSimilarity()
    {
        return localSimilarity;
    }
    public void SetGlobalSimilarity(float s)
    {
        globalSimilarity = s;
    }

    public float GetGlobalSimilarity()
    {
        return globalSimilarity;
    }

    public Gesture TwoDimensionalGestureData()
    {
        Gesture g = new Gesture();
        g.MakeEqualTo(this);
        foreach (Pose p in g.poses)
        {
            foreach (Joint d in p.joints)
            {
                d.z = 0;
            }
        }
        return g;
    }
}
