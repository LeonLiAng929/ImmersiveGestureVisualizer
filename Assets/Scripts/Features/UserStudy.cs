using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStudy : MonoBehaviour
{
    #region Singleton
    public static UserStudy instance;

    // Mark = -1, Animate = 0, ChangeCluster = 1,
    // Slidimation = 2, StackGestures = 3, UnfoldCluster = 4,
    // ShowSmallMultiples = 5, ResumeStackedGestures = 6,
    // StackAll = 7, HeatMap=8, CloseComparison=9,Search=10
    [HideInInspector]
    public int[] featureCount = new int[16]; 
    [HideInInspector]
    public Dictionary<Actions, float> featureDuration = new Dictionary<Actions, float>();
    #endregion
    private void Awake()
    {
        instance = this;
        /*for(int i =0;i<=10;i++)
        {
            featureCount.Add((Actions)i, 0);
            featureDuration.Add((Actions)i, 0);
        }*/
        for(int i = 0; i < featureCount.Length; i++)
        {
            featureCount[i] = 0;
        }

    }

    public void ClearCounts()
    {
        for (int i = 0; i < featureCount.Length; i++)
        {
            featureCount[i] = 0;
        }
        Debug.Log("Feature Counts Cleared!");
    }

    public void EndSession()
    {
        IO io = new IO();
        io.WriteFeatureCount();
        io.WriteUserResult();
        Debug.Log("Session Ended! Thanks for your participation!");
        ClearCounts();
    }

    public void IncrementCount(Actions a)
    {
        featureCount[(int)a] += 1;
    }

    public void AccumulateDuration(Actions a, float duration)
    {
        featureDuration[a] += duration;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
