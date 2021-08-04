using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class pressed : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            Debug.Log("clicked!");
        }
    }
   
    public void Pressed()
    {
        Debug.Log("!!!!");
    }
}
