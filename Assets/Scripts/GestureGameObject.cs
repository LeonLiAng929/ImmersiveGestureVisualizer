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
    private bool swing = false;
    private bool stacked = false;
    public bool selected = false;
    private float currTime = 0;
    private int counter = 0;
    private float prevTimestamp;
    public Vector3 allocatedPos;
    public Vector3 sizeB4Stack;
    private Quaternion lastQuat;
    int rotate = 0;
    int currPoseIndex = 0;
    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            AnimationConditionUpdate();
        }
        if (swing)
        {
            SwingConditionUpdate();
        }

    }

   void FixedUpdate()
    {
        if (animate)
        {
            Animate();
        }

        if (swing)
        {
            Swing();
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
        //else if(curr == Actions.Slidimation) { ShowTracer();  }
        else if (curr == Actions.Slidimation) { ActivateSwinging(); 
            GestureVisualizer.instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out lastQuat); }
        else if(curr == Actions.StackGestures) { StackGesture();}
    }


    public void AnimationConditionUpdate()
    {
        float lastTimeStamp = (float)gesture.poses[gesture.poses.Count-1].timestamp;
        if (Extension.SecondsToMs(timer) > lastTimeStamp)
        {
            AnimationModeReset();
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
    public void AnimationModeReset()
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
        {
            animate = false;
            foreach (MeshRenderer mr in gameObject.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
            {
                Color temp = mr.material.color;
                temp.a = 1;
                mr.material.color = temp;
            }
        }
        else
        {
            animate = true;
            foreach (MeshRenderer mr in gameObject.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
            {
                Color temp = mr.material.color;
                temp.a = 0.5f;
                mr.material.color = temp;
            }
        }

    }

    public void Swing()
    {
        GameObject skeleton = gameObject.GetComponentInChildren<TrajectoryTR>().skeletonRef;
        Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();
        for (int i = 1; i < 21; i++)
        {
            transforms[i].localPosition = gesture.poses[currPoseIndex].joints[i - 1].ToVector();
        }
    }

    void GetRotateDirection(Quaternion from, Quaternion to)
    {
        float fromY = from.eulerAngles.y;
        float toY = to.eulerAngles.y;
        float clockWise = 0f;
        float counterClockWise = 0f;

        if (fromY <= toY)
        {
            clockWise = toY - fromY;
            counterClockWise = fromY + (360 - toY);
        }
        else
        {
            clockWise = (360 - fromY) + toY;
            counterClockWise = fromY - toY;
        }
        if (clockWise <= counterClockWise)
            rotate = -1;
        else
            rotate = 1;
        if (Math.Abs(fromY-toY) < 60/gesture.num_of_poses)
        {
            rotate = 0;
        }
    }

    public void SwingConditionUpdate() {
        Quaternion currQuat;
        GestureVisualizer.instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out currQuat);
        GetRotateDirection(lastQuat, currQuat);
        if (rotate != 0)
        {
            if(rotate == 1)
            {
                if(currPoseIndex < gesture.num_of_poses - 1)
                currPoseIndex += 1;
            }
            else
            {
                if (currPoseIndex > 0)
                {
                    currPoseIndex -= 1;
                }
            }
        }
        lastQuat = currQuat;
    }
    public void ActivateSwinging()
    {
        if (swing)
            swing = false;
        else
            swing = true;
    }
    public void ChangeCluster()
    {
        List<GameObject> selectionList = GestureVisualizer.instance.selectedGestures;
        if (selected)
        {
            selected = false;
            GestureVisualizer.instance.UpdateGlowingFieldColour(gameObject) ;
            if (gameObject.name == "AverageGesture")
            {
                GestureGameObject[] children = gameObject.transform.parent.GetComponentsInChildren<GestureGameObject>(true);
                foreach (GestureGameObject gGO in children)
                {
                    if (gGO.gameObject.name != "AverageGesture")
                    {
                        if (selectionList.Contains(gGO.gameObject))
                        {
                            GestureVisualizer.instance.UpdateGlowingFieldColour(gGO.gameObject);
                            selectionList.Remove(gGO.gameObject);
                            gGO.selected = false;
                        }
                    }
                }
            }
            else
            {
                if (selectionList.Contains(gameObject))
                    selectionList.Remove(gameObject);
            }

        }
        else
        {
            selected = true;
            gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.white;    

            if (gameObject.name == "AverageGesture")
            {
                GestureGameObject[] children = gameObject.transform.parent.GetComponentsInChildren<GestureGameObject>(true);
                foreach(GestureGameObject gGO in children)
                {
                    if (gGO.gameObject.name != "AverageGesture")
                    {
                        if (!selectionList.Contains(gGO.gameObject))
                        {
                            gGO.gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.white;
                            gGO.selected = true;
                            selectionList.Add(gGO.gameObject);
                        }
                    }
                }
            }
            else
            {
                if (!selectionList.Contains(gameObject))
                    selectionList.Add(gameObject);
            }
        }
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
    public void StackGesture()
    {
        stacked = true;
        List<GameObject> stackedList = GestureVisualizer.instance.stackedObjects;
        if (!stackedList.Contains(gameObject))
        {
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
            stackedList.Add(gameObject);
            if (gameObject.name == "AverageGesture")
            {
                sizeB4Stack = gameObject.transform.localScale;
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        GestureVisualizer.instance.PrepareStack();
    }

    public bool IsStacked()
    {
        return stacked;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void RevertStacking()
    {
        stacked = false;
        gameObject.transform.localPosition = allocatedPos;
        if (gameObject.name == "AverageGesture")
        {
            gameObject.transform.localScale = sizeB4Stack;
        }
    }
}
  