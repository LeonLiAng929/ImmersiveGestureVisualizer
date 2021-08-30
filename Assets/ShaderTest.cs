using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class ShaderTest : MonoBehaviour
{
    public Shader a;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<MeshRenderer>().material.shader = a;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
