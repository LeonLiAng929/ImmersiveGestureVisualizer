using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRTest : MonoBehaviour
{
    int i = 0;
    float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - t > 1)
        {
            Debug.Log(i);
            Vector3[] old = gameObject.GetComponent<TubeRenderer>().points;
            Vector3[] plus = { new Vector3(i, i, i) };
            i += 1;
            Vector3[] newArray = new Vector3[old.Length + plus.Length];
            old.CopyTo(newArray, 0);
            plus.CopyTo(newArray, old.Length);
            gameObject.GetComponent<TubeRenderer>().points = newArray;
            t += Time.deltaTime;
        }
        
    }
}
