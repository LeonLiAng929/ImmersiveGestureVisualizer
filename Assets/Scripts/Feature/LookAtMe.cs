using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMe : MonoBehaviour
{
    Transform user;
    // Start is called before the first frame update
    void Start()
    {
        user = Camera.main.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(user.position);
    }
}
