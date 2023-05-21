using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    IO io = new IO();
    // Start is called before the first frame update
    void Start()
    {
        io.LoadHandGesture();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
