using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconTracing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, 4, Camera.main.gameObject.transform.position.z);
    }
}
