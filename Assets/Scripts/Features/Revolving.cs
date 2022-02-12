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
        List<NewLookAt> uniqueFeatures = new List<NewLookAt>(gameObject.GetComponentsInChildren<NewLookAt>());
        if (uniqueFeatures.Contains(gameObject.GetComponent<NewLookAt>()))
            uniqueFeatures.Remove(gameObject.GetComponent<NewLookAt>());
        
        NewLookAt confirm = null;

        foreach(NewLookAt button in uniqueFeatures)
        {
            if (button.gameObject.name == "ClusteringInterface")
            {
                confirm = button;
            }
           
        }

        uniqueFeatures.Remove(confirm);
    

        foreach(NewLookAt tag in uniqueFeatures)
        {
            featureTrans.Add(tag.gameObject.transform);
        }
        InstantiateInSemiCircle(featureTrans, gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, Camera.main.transform.localPosition.y, gameObject.transform.localPosition.z);
        //gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, gameObject.transform.position.z);
        /*if (!hovered)
        {
            gameObject.transform.Rotate(new Vector3(0,0,0.1f));
        }*/
    }


    public void InstantiateInSemiCircle(List<Transform> featureObjs, Vector3 location)
    {
        int howMany = featureObjs.Count;
        float angleSection = Mathf.PI / howMany;
        for (int i = 0; i < howMany; i++)
        {
            float angle = i * angleSection;
            float radius = 1f;
            //Vector3 newPos = location + new Vector3(Mathf.Cos(angle), 0.55f, Mathf.Sin(angle)) * radius;
            Vector3 newPos = location + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 1.5f) * radius;
            //newPos.y = yPosition;
            featureObjs[i].localPosition = newPos;
        }
    }
}
