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

    private float timer = 0.0f; // the time since start() in seconds.
    private int init = 1;
    private bool animate = false;
    private float currTime = 0;
    private int counter = 0;
    private float prevTimestamp;
    private GameObject stackedObj;
    public Vector3 allocatedPos;
    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            AnimationConditionUpdate();
        }
    }

   void FixedUpdate()
    {
        if (animate)
        {
            Animate();
        }
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
        if (curr == Actions.Animate){ ActivateAnimate();  }
        else if(curr == Actions.ChangeCluster){ ChangeCluster();  }
        else if(curr == Actions.ShowSmallMultiples) { ShowSmallMultiples();  }
        else if(curr == Actions.ShowTracer) { ShowTracer();  }
        else if(curr == Actions.StackGestures) { StackGestures();}
    }
    
    public void AnimationConditionUpdate()
    {
        float lastTimeStamp = (float)gesture.poses[gesture.poses.Count-1].timestamp;
        if (Extension.SecondsToMs(timer) > lastTimeStamp)
        {
            Reset();
        }
        if (Extension.SecondsToMs(timer) > currTime)
        {
            prevTimestamp = currTime;
            UpdateCounter();
            currTime = (float)gesture.poses[counter].timestamp;
        }
    }

    public void UpdateCounter()
    {
        counter += 1;
    }
    public void Reset()
    {
        Transform[] transforms = GetComponent<Transform>().Find("Trajectory").Find("Skeleton").GetComponentsInChildren<Transform>();

        for (int i = 1; i < gesture.poses[0].num_of_joints+1; i++)
        {   
            transforms[i].localPosition = gesture.poses[0].joints[i - 1].ToVector();
        }
        timer = 0f;
        currTime = 0;
        counter = 1;
        init = 1;
        UpdateCounter();
    }
    public void Animate()
    {
        if (init == 1)
        {
            init = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }

        Transform[] transforms= GetComponent<Transform>().Find("Trajectory").Find("Skeleton").GetComponentsInChildren<Transform>();
        for (int i = 1; i < gesture.poses[0].num_of_joints + 1; i++)
        {
            Vector3 start_pos = transforms[i].localPosition;
            Vector3 end_pos = gesture.poses[counter].joints[i-1].ToVector();

            if (currTime - prevTimestamp > 0)
            {
                float ratio = (Extension.SecondsToMs(timer) - prevTimestamp) / (currTime - prevTimestamp);
                Vector3 interpolation = Vector3.Lerp(start_pos, end_pos, ratio);
           
                transforms[i].localPosition = Vector3.Lerp(start_pos, end_pos, ratio);
            }
        }
    }

    public void ActivateAnimate()
    {
        if (animate)
            animate = false;
        else
            animate = true;
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
        GameObject tracers = GetComponent<Transform>().Find("Trajectory").Find("Tracers").gameObject;
        bool isActive = !tracers.activeSelf;
        tracers.SetActive(isActive);
    }
    public void StackGestures()
    {
        /*if (stackedObj != null)
            Destroy(stackedObj);
        
        GameObject clone = Instantiate(GetComponent<Transform>().gameObject);
        clone.GetComponent<Transform>().Find("Trajectory").localPosition = new Vector3(0, 0, 0);
        stackedObj = clone;
        stackedObj.name = "StackedObject" + gesture.id.ToString();
        List<GameObject> stackedList = GestureVisualizer.instance.stackedObjects;
        if (!stackedList.Contains(stackedObj))
            stackedList.Add(stackedObj);
        GestureVisualizer.instance.PrepareStack();*/
        List<GameObject> stackedList = GestureVisualizer.instance.stackedObjects;
        if (!stackedList.Contains(gameObject))
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
            stackedList.Add(gameObject);
        GestureVisualizer.instance.PrepareStack();
    }

    public void ResumePosition()
    {
        gameObject.transform.localPosition = allocatedPos;
    }
}
  