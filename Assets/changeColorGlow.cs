using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeColorGlow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
