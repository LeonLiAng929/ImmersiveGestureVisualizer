using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSkeleton : MonoBehaviour
{
    [SerializeField]
    protected GameObject lr;
    [SerializeField]
    //protected Transform skeletonRendererContainer;

    protected Transform trans;
    protected List<LineRenderer> lineRenderers = new List<LineRenderer>();
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
     
        lr.GetComponent<LineRenderer>().SetWidth(0.01f, 0.01f);
      
        DrawHumanSkeleton();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSkeleton();
    }

    LineRenderer Draw(Vector3[] points)
    {
        GameObject newLineGen = Instantiate(lr, trans.Find("LineRenderers"));
        // Get reference to newLineGen's LineRenderer.
        LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();

        // Set amount of LineRenderer positions = amount of line point positions.
        lRend.positionCount = points.Length;
        // Set positions of LineRenderer using linePoints array.
        lRend.SetPositions(points);
        return lRend;
    }

    void UpdateSkeleton()
    {
        //update neck
        // Debug.Log(trans.Find("Head"));
        Vector3[] neck = { trans.Find("Head").position, trans.Find("ShoulderCenter").position };
        lineRenderers[0].SetPositions(neck);
        //update arms
        Vector3[] leftarm = { trans.Find("ShoulderCenter").position,
            trans.Find("ShoulderLeft").position,
            trans.Find("ElbowLeft").position,
            trans.Find("WristLeft").position,
            trans.Find("HandLeft").position};

        Vector3[] rightarm = { trans.Find("ShoulderCenter").position,
            trans.Find("ShoulderRight").position,
            trans.Find("ElbowRight").position,
            trans.Find("WristRight").position,
            trans.Find("HandRight").position};

        lineRenderers[1].SetPositions(leftarm);
        lineRenderers[2].SetPositions(rightarm);

        // update spine
        Vector3[] spine = { trans.Find("ShoulderCenter").position,
            trans.Find("Spine").position,
            trans.Find("HipCenter").position};
        lineRenderers[3].SetPositions(spine);

        //update legs
        Vector3[] leftleg = { trans.Find("HipCenter").position,
            trans.Find("HipLeft").position,
            trans.Find("KneeLeft").position,
            trans.Find("AnkleLeft").position,
            trans.Find("FootLeft").position};

        Vector3[] rightleg = { trans.Find("HipCenter").position,
            trans.Find("HipRight").position,
            trans.Find("KneeRight").position,
            trans.Find("AnkleRight").position,
            trans.Find("FootRight").position};
        lineRenderers[4].SetPositions(leftleg);
        lineRenderers[5].SetPositions(rightleg);
    }

    void DrawHumanSkeleton()
    {
        //draw neck
       // Debug.Log(trans.Find("Head"));
        Vector3[] a = { trans.Find("Head").position, trans.Find("ShoulderCenter").position};
        lineRenderers.Add(Draw(a));
        //draw arms
        Vector3[] leftarm = { trans.Find("ShoulderCenter").position,
            trans.Find("ShoulderLeft").position,
            trans.Find("ElbowLeft").position,
            trans.Find("WristLeft").position,
            trans.Find("HandLeft").position};

        Vector3[] rightarm = { trans.Find("ShoulderCenter").position,
            trans.Find("ShoulderRight").position,
            trans.Find("ElbowRight").position,
            trans.Find("WristRight").position,
            trans.Find("HandRight").position};

        lineRenderers.Add(Draw(leftarm));
        lineRenderers.Add(Draw(rightarm));

        // draw spine
        Vector3[] spine = { trans.Find("ShoulderCenter").position,
            trans.Find("Spine").position,
            trans.Find("HipCenter").position};
        lineRenderers.Add(Draw(spine));

        //draw legs
        Vector3[] leftleg = { trans.Find("HipCenter").position,
            trans.Find("HipLeft").position,
            trans.Find("KneeLeft").position,
            trans.Find("AnkleLeft").position,
            trans.Find("FootLeft").position};

        Vector3[] rightleg = { trans.Find("HipCenter").position,
            trans.Find("HipRight").position,
            trans.Find("KneeRight").position,
            trans.Find("AnkleRight").position,
            trans.Find("FootRight").position};
        lineRenderers.Add(Draw(leftleg));
        lineRenderers.Add(Draw(rightleg));
    }
}
