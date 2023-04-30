using Accord.MachineLearning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterColorSetting : MonoBehaviour
{
    [SerializeField]
    public List<Color> colors = new List<Color>();
    public static ClusterColorSetting Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
