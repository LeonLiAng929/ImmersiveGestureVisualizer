using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.MachineLearning;
using Accord.Statistics.Filters;
using Accord.Statistics.Kernels;
using System.Linq;
//using ADN.TimeSeries;

public class K_Mean : MonoBehaviour
{
    // Start is called before the first frame update
   /* void Start()
    {
        List<double[]> series = new List<double[]>();
        series.Add(new double[] { 2, 2, 4, 5 });
        series.Add(new double[] { 1, 2, 3 });
        series.Add(new double[] { 5, 5, 5 });
        double [] a = ADN.TimeSeries.DBA.Average(series);
        foreach (double x in a)
        {
            //Debug.Log(x);
        }
   
        
    }

    
    private void Start()
    {
        //PerformClustering();
        var a = new List<List<int>> { new List<int>() { 1,2,3} };
        var b = new List<int>() { 4 };
        a.Add(b);
        foreach (List<int> i in a)
        {
            
            Debug.Log(i[0]);
        }
    }
    void PerformClustering()
    {
        IO xmlLoader = IO.instance;
        List<Gesture> gestures = xmlLoader.LoadXML("angry like a bear-1");
        foreach (Gesture g in gestures)
        {
            g.SetBoundingBox();
            g.SetCentroid();
            g.TranslateToOrigin();
            g.NormalizeHeight();
        }
            Accord.Math.Random.Generator.Seed = 0;

        // Declare some mixed discrete and continuous observations
        double[][] observations =
        {
            //             (categorical) (discrete) (continuous)
            new double[] {       1,          -1,        -2.2,   1   },
            new double[] {       1,          -6,        -5.5,   1      },
            new double[] {       2,           1,         1.1 ,2     },
            new double[] {       2,           2,         1.2   ,3   },
            new double[] {       2,           2,         2.6     ,4 },
            new double[] {       3,           2,         1.4     ,2 },
            new double[] {       3,           4,         5.2      ,1},
            new double[] {       1,           6,         5.1      ,1},
            new double[] {       1,           6,         5.9      ,4},
        };

        // Create a new codification algorithm to convert 
        // the mixed variables above into all continuous:
          var codification = new Codification<double>()
          {
              CodificationVariable.Categorical,
              CodificationVariable.Discrete,
              CodificationVariable.Continuous
          };

          // Learn the codification from observations
          var model = codification.Learn(observations);

          // Transform the mixed observations into only continuous:
          double[][] newObservations = model.ToDouble().Transform(observations);

          // (newObservations will be equivalent to)
          double[][] expected =
          {
              //               (one hot)    (discrete)    (continuous)
              new double[] {    1, 0, 0,        -1,          -2.2      },
              new double[] {    1, 0, 0,        -6,          -5.5      },
              new double[] {    0, 1, 0,         1,           1.1      },
              new double[] {    0, 1, 0,         2,           1.2      },
              new double[] {    0, 1, 0,         2,           2.6      },
              new double[] {    0, 0, 1,         2,           1.4      },
              new double[] {    0, 0, 1,         4,           5.2      },
              new double[] {    1, 0, 0,         6,           5.1      },
              new double[] {    1, 0, 0,         6,           5.9      },
           };
          Debug.Log(newObservations[0].Length);
          Debug.Log(newObservations[0][5]);
          Debug.Log(newObservations);
        // Create a new K-Means algorithm
        //DynamicTimeWarping temp = new DynamicTimeWarping();
        KMeans kmeans = new KMeans(k: 3) {
            Distance = new DynamicTimeWarping()
        };
        


        // Compute and retrieve the data centroids
        var clusters = kmeans.Learn(observations);

        // Use the centroids to parition all the data
        int[] labels = clusters.Decide(observations);
    }*/
}
