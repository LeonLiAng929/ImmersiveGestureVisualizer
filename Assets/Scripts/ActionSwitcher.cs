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
    private static Actions previouslyActive;
    public int actionID;
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.activated.AddListener(SwitchAction);
        xRSimpleInteractable.firstHoverEntered.AddListener(OnHovered);
        init = gameObject.GetComponent<MeshRenderer>().material.color;
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
    public void SwitchAction(ActivateEventArgs args)
    {
        if (currentlyActive != (Actions)actionID)
        {
            previouslyActive = currentlyActive;
            currentlyActive = (Actions)actionID;
            Debug.Log(currentlyActive);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        if (previouslyActive == (Actions)actionID)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = init;
        }
    }

    public void OnHovered(HoverEnterEventArgs arg)
    {
        gameObject.transform.parent.GetComponent<Revolving>().hovered = true;
    }
    public Actions GetCurrentAction()
    {
        return currentlyActive;
    }
}
