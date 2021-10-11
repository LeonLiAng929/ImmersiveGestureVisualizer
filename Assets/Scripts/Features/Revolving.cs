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
        List<LookAtMe> uniqueFeatures = new List<LookAtMe>(gameObject.GetComponentsInChildren<LookAtMe>(true));
        if (uniqueFeatures.Contains(gameObject.GetComponent<LookAtMe>()))
            uniqueFeatures.Remove(gameObject.GetComponent<LookAtMe>());
        foreach(LookAtMe tag in uniqueFeatures)
        {
            featureTrans.Add(tag.gameObject.transform);
        }
        InstantiateInCircle(featureTrans, gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, Camera.main.transform.localPosition.y, gameObject.transform.localPosition.z);
        //gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, gameObject.transform.position.z);
        /*if (!hovered)
        {
            gameObject.transform.Rotate(new Vector3(0,0,0.1f));
        }*/
    }


    public void InstantiateInCircle(List<Transform> featureObjs, Vector3 location)
    {
        int howMany = featureObjs.Count;
        float angleSection = Mathf.PI * 2f / howMany;
        for (int i = 0; i < howMany; i++)
        {
            float angle = i * angleSection;
            float radius = 0.8f;
            //Vector3 newPos = location + new Vector3(Mathf.Cos(angle), 0.55f, Mathf.Sin(angle)) * radius;
            Vector3 newPos = location + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 1.5f) * radius;
            //newPos.y = yPosition;
            featureObjs[i].localPosition = newPos;
        }
    }
}
