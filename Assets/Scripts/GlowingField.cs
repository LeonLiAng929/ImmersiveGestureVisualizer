using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingField : MonoBehaviour
{
    Transform capsule;
    // Start is called before the first frame update
    void Start()
    {
        capsule = gameObject.GetComponent<Transform>().parent.Find("Capsule");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(capsule.transform.position.x, 0, capsule.transform.position.z);
        gameObject.transform.localScale = new Vector3(1f+ capsule.transform.localScale.x, 0.05f, 1f+capsule.transform.localScale.z);
    }
}
