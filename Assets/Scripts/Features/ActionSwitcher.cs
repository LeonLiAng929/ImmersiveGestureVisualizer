using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
// using UnityEngine.EventSystems;  //for the old menu system
using UnityEngine.XR.Interaction.Toolkit;

public class ActionSwitcher : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    Color init;
    #region Singleton
    public static ActionSwitcher instance;
    #endregion
    //public Dictionary<string, bool> actions = new Dictionary<string, bool>();

    private static Actions currentlyActive;
    public int actionID;
    private GameObject selectionIndicator;
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        selectionIndicator = transform.Find("SelectionIndicator").gameObject;
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(SwitchAction);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
        transform.Find("SelectionIndicator").gameObject.SetActive(false);
    }

    // for the old Menu system

    /*  // Update is called once per frame
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
    */

    // for the new menu system
    public void SwitchAction(SelectExitEventArgs args)
    {
        if (currentlyActive != (Actions)actionID)
        {
            foreach (ActionSwitcher aswr in gameObject.transform.parent.GetComponentsInChildren<ActionSwitcher>())
            {
                aswr.gameObject.GetComponent<MeshRenderer>().material.color = aswr.init;
                aswr.selectionIndicator.SetActive(false);

            }
            
            currentlyActive = (Actions)actionID;
            Debug.Log(currentlyActive);
      
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            selectionIndicator.SetActive(true);
        }
        else
        {
            foreach (ActionSwitcher aswr in gameObject.transform.parent.GetComponentsInChildren<ActionSwitcher>())
            {
                aswr.gameObject.GetComponent<MeshRenderer>().material.color = aswr.init;
                aswr.selectionIndicator.SetActive(false);
                currentlyActive = Actions.Idle;
                
            }
            Debug.Log(currentlyActive);
        }
    }

    public Actions GetCurrentAction()
    {
        return currentlyActive;
    }
}
