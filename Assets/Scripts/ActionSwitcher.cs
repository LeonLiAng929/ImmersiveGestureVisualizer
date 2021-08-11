using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionSwitcher : MonoBehaviour
{
    #region Singleton
    public static ActionSwitcher instance;
    #endregion
    //public Dictionary<string, bool> actions = new Dictionary<string, bool>();

    private static Actions currentlyActive;
    public int actionID;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    { 
        if(EventSystem.current.currentSelectedGameObject == gameObject)
        {
            if (currentlyActive != (Actions)actionID)
            {
                currentlyActive = (Actions)actionID;
                Debug.Log(currentlyActive);
            }
        }
    }

    public void SwitchAction()
    {
        Debug.Log("clicked!");
        if(Enum.IsDefined(typeof(Actions), actionID))
        {
            currentlyActive = (Actions)actionID;
            Debug.Log(currentlyActive);
        }
    }

    public Actions GetCurrentAction()
    {
        return currentlyActive;
    }
}
