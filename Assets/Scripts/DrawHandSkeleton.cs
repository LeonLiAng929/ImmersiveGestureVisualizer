using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHandSkeleton : MonoBehaviour
{
    [SerializeField]
    public GameObject lr;
    //protected Transform skeletonRendererContainer;

    protected Transform trans;
    protected List<LineRenderer> lineRenderers = new List<LineRenderer>();
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
     
        lr.GetComponent<LineRenderer>().SetWidth(0.035f, 0.035f);
      
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
        //update palm

        Vector3[] palm = { trans.Find("17").position,
            trans.Find("13").position,
            trans.Find("9").position,
            trans.Find("5").position,
            trans.Find("2").position};
        lineRenderers[0].SetPositions(palm);
        //update Thumb
        Vector3[] thumb = { trans.Find("4").position,
            trans.Find("3").position,
            trans.Find("2").position,
            trans.Find("1").position,
            trans.Find("0").position};

        lineRenderers[1].SetPositions(thumb);


        // update index
        Vector3[] index = { trans.Find("5").position,
            trans.Find("6").position,
            trans.Find("7").position,
            trans.Find("8").position
        };
        lineRenderers[2].SetPositions(index); 

        //update middle
        Vector3[] middle = { trans.Find("9").position,
            trans.Find("10").position,
            trans.Find("11").position,
            trans.Find("12").position};
        lineRenderers[3].SetPositions(middle);
        //update ring
        Vector3[] ring = { trans.Find("13").position,
            trans.Find("14").position,
            trans.Find("15").position,
            trans.Find("16").position};


        lineRenderers[4].SetPositions(ring);

        //update little
        Vector3[] little = { trans.Find("0").position,
            trans.Find("17").position,
            trans.Find("18").position,
            trans.Find("19").position,
            trans.Find("20").position};


        lineRenderers[5].SetPositions(little);
    }

    void DrawHumanSkeleton()
    {
        //draw palm
       
        Vector3[] palm = { trans.Find("17").position,
            trans.Find("13").position,
            trans.Find("9").position,
            trans.Find("5").position,
            trans.Find("2").position};
        lineRenderers.Add(Draw(palm));
        //draw Thumb
        Vector3[] thumb = { trans.Find("4").position,
            trans.Find("3").position,
            trans.Find("2").position,
            trans.Find("1").position,
            trans.Find("0").position}; 

        lineRenderers.Add(Draw(thumb));
    

        // draw index
        Vector3[] index = { trans.Find("5").position,
            trans.Find("6").position,
            trans.Find("7").position,
            trans.Find("8").position
        };
        lineRenderers.Add(Draw(index));

        //draw middle
        Vector3[] middle = { trans.Find("9").position,
            trans.Find("10").position,
            trans.Find("11").position,
            trans.Find("12").position};
        lineRenderers.Add(Draw(middle));
        //draw ring
        Vector3[] ring = { trans.Find("13").position,
            trans.Find("14").position,
            trans.Find("15").position,
            trans.Find("16").position};

        
        lineRenderers.Add(Draw(ring));

        //draw little
        Vector3[] little = { trans.Find("0").position,
            trans.Find("17").position,
            trans.Find("18").position,
            trans.Find("19").position,
            trans.Find("20").position};


        lineRenderers.Add(Draw(little));
    }
}
