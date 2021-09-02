using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Revolving : MonoBehaviour
{
    private List<Transform> featureTrans = new List<Transform>();
    public bool hovered = false;
    // Start is called before the first frame update
    void Start()
    {
        featureTrans = new List<Transform>(gameObject.GetComponentsInChildren<Transform>(true));
        if (featureTrans.Contains(gameObject.transform))
            featureTrans.Remove(gameObject.transform);
        InstantiateInCircle(featureTrans, gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(!hovered)
        gameObject.transform.Rotate(new Vector3(0,1,0), 0.1f);
    }


    public void InstantiateInCircle(List<Transform> featureObjs, Vector3 location)
    {
        int howMany = featureObjs.Count;
        float angleSection = Mathf.PI * 2f / howMany;

        for (int i = 0; i < howMany; i++)
        {
            float angle = i * angleSection;
            float radius = 2;
            Vector3 newPos = location + new Vector3(Mathf.Cos(angle), 1, Mathf.Sin(angle)) * radius;
            //newPos.y = yPosition;
            featureObjs[i].localPosition = newPos;
        }
    }
}
