using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLookAt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform, -1 * transform.up);
        transform.Rotate(new Vector3(180, 180, 0), Space.Self);
        
    }
}
