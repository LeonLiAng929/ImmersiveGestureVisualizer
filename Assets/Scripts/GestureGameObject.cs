using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class GestureGameObject : MonoBehaviour
{
    public Gesture gesture;

    [SerializeField]
    protected XRSimpleInteractable xRSimpleInteractable;

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.activated.AddListener(PerformAction);
    }

    private void OnDestroy()
    {
        xRSimpleInteractable.activated.RemoveAllListeners();
    }

    public void PerformAction(ActivateEventArgs arg)
    {
        Actions curr = ActionSwitcher.instance.GetCurrentAction();
        if (curr == Actions.Animate){ Animate(); }
        else if(curr == Actions.ChangeCluster){ ChangeCluster(); }
        else if(curr == Actions.ShowSmallMultiples) { ShowSmallMultiples();}
        else if(curr == Actions.ShowTracer) { ShowTracer();}
        else if(curr == Actions.StackGestures) { StackGestures();}
    }

    public void Animate()
    {

    }
    public void ChangeCluster()
    {

    }
    public void ShowSmallMultiples()
    {
        GameObject multiples = GetComponent<Transform>().Find("SmallMultiples").gameObject;
        bool isActive = !multiples.activeSelf;
        multiples.SetActive(isActive);
    }
    public void ShowTracer()
    {
        GameObject tracers = GetComponent<Transform>().Find("Tracers").gameObject;
        bool isActive = !tracers.activeSelf;
        tracers.SetActive(isActive);
    }
    public void StackGestures()
    {

    }

}
