using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class GestureGameObject : MonoBehaviour
{
    public Gesture gesture;
    //public Gesture2DObject uiRef;
    public Transform rightController; // just in case if some controllers cant be dectected using XR.commam usages.

    [SerializeField]
    protected XRSimpleInteractable xRSimpleInteractable;

    private float timer = 0.0f; // the time since start() in seconds.
    private int init = 1;
    private bool animate = false;
    private bool swing = false;
    private bool stacked = false;
    public bool selected = false;
    public bool averageGesture = false;
    private float currTime = 0;
    private int counter = 0;
    private float prevTimestamp;
    public Vector3 allocatedPos;
    public Vector3 initPos;
    public Vector3 sizeB4Stack;
    public Quaternion lastQuat;
    public GameObject arrow;
    int rotate = 0;
    int currPoseIndex = 0;
    private int jointCount;
    // Update is called once per frame

    // a skeleton to link the animation and the slidimation with the small-multiples
    private GameObject timeIndicator;

    private void Start()
    {
        arrow = gameObject.transform.Find("Arrow").gameObject;
        arrow.SetActive(false);
        if (gameObject.name != "AverageGesture" && gameObject.name != "Gesture")
        {
            GameObject multiples = GetComponent<Transform>().Find("SmallMultiples").gameObject;
            timeIndicator = Instantiate(GestureVisualizer.instance.skeletonModel, multiples.transform);
            timeIndicator.name = "TimeIndicator";
            Transform[] transforms = timeIndicator.GetComponentsInChildren<Transform>();
            transforms[0].localPosition = new Vector3(0, 0, 0.7f);
            jointCount = gesture.poses[0].num_of_joints;
            for (int i = 1; i < jointCount; i++)
            {
                transforms[i].localPosition = gesture.poses[0].joints[i - 1].ToVector();
            }
            MeshRenderer[] mrs = timeIndicator.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mr in mrs)
            {
                mr.material.color = Color.red;
            }
        }
    }
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
        xRSimpleInteractable.selectExited.AddListener(PerformAction);
        
    }

    private void OnDestroy()
    {
        xRSimpleInteractable.selectExited.RemoveAllListeners();
    }

    public void PerformAction(SelectExitEventArgs arg)
    {
        Actions curr = ActionSwitcher.instance.GetCurrentAction();
        if (curr == Actions.Animate){ ActivateAnimate();  }
        else if(curr == Actions.ChangeCluster){ ChangeCluster();  }
        else if(curr == Actions.ShowSmallMultiples) { ShowSmallMultiples();  }
        //else if(curr == Actions.Slidimation) { ShowTracer();  }
        else if (curr == Actions.Slidimation) { ActivateSwinging(); 
            GestureVisualizer.instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out lastQuat); }
        else if(curr == Actions.StackGestures) { StackGesture();}
        else if (curr == Actions.CloseComparison) { if (arg.interactor.gameObject.name == "LeftHand Controller")
            CloseComparison(true);
            else if (arg.interactor.gameObject.name == "RightHand Controller")
            {
                CloseComparison(false);
            }
        }
        else if(curr == Actions.Idle)
        {
            IdleSelect();
        }
    }

    public void CloseComparison(bool lefthand)
    {
        if ((lefthand && !GestureVisualizer.instance.leftHandSelected) || (!lefthand && !GestureVisualizer.instance.rightHandSelected))
        {
            List<GameObject> selectionList = GestureVisualizer.instance.selectedGestures;
            if (selected)
            {
                if (gameObject.name != "AverageGesture")
                {
                    //uiRef.ComparisonIndicator.SetActive(false);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                }
                selected = false;
                if (selectionList.Contains(gameObject))
                {
                    //uiRef.GetComponent<MeshRenderer>().material.color = GestureVisualizer.instance.UpdateGlowingFieldColour(gameObject);
              
                    selectionList.Remove(gameObject);
                    if (lefthand)
                        GestureVisualizer.instance.leftHandSelected = false;
                    else
                        GestureVisualizer.instance.rightHandSelected = false;
                }
            }
            else
            {
                if (gameObject.name != "AverageGesture")
                {
                    //uiRef.ComparisonIndicator.SetActive(true);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                }
                selected = true;
                if (!selectionList.Contains(gameObject))
                {
                    selectionList.Add(gameObject);
                    if (lefthand)
                    {
                        gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.white;
                        GestureVisualizer.instance.leftHandSelected = true;
                    }
                    else
                    {
                        gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.black;
                        GestureVisualizer.instance.rightHandSelected = true;
                    }
                }

            }
            initPos = gameObject.transform.localPosition;
            if (GestureVisualizer.instance.rightHandSelected && GestureVisualizer.instance.leftHandSelected)
            {
                GestureVisualizer.instance.CloseComparison();
            }
        }
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
        counter = 0;
        init = 1;
        //UpdateCounter();
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
        GameObject multiples = GetComponent<Transform>().Find("SmallMultiples").gameObject;
        Transform[] timeIndicatorTrans = new Transform[jointCount];
        if (gameObject.name != "AverageGesture")
        {
            timeIndicatorTrans = timeIndicator.GetComponentsInChildren<Transform>();
        }
        for (int i = 1; i < gesture.poses[0].num_of_joints + 1; i++)
        {
            Vector3 start_pos = transforms[i].localPosition;
            Vector3 end_pos = gesture.poses[counter].joints[i-1].ToVector();

            if (currTime - prevTimestamp > 0)
            {
                float ratio = (Extension.SecondsToMs(timer) - prevTimestamp) / (currTime - prevTimestamp);
                
                Vector3 interpolation = Vector3.Lerp(start_pos, end_pos, ratio);

                transforms[i].localPosition = interpolation;
               
                double distance = currTime / gesture.poses[gesture.poses.Count - 1].timestamp * 2.0f;
                if (gameObject.name != "AverageGesture")
                {
                    timeIndicatorTrans[i].localPosition = interpolation;
                    timeIndicator.transform.localPosition = new Vector3(0, 0, 0.7f) + new Vector3(0, 0, (float)distance);
                }
            }
        }
    }

    public void ActivateAnimate()
    {
        if (animate)
        {
            if (gameObject.name != "AverageGesture" && gameObject.name != "Preview")
            {
                //uiRef.AnimationIndicator.SetActive(false);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            animate = false;
            /*if (GetComponent<Transform>().Find("SmallMultiples").gameObject.activeSelf)
            {
                timeIndicator.SetActive(false);
            }*/
            

            foreach (MeshRenderer mr in gameObject.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
            {
                Color temp = mr.material.color;
                temp.a = 1;
                mr.material.color = temp;
            }
        }
        else
        {
            if (gameObject.name != "AverageGesture" && gameObject.name != "Preview")
            {
                //uiRef.AnimationIndicator.SetActive(true);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            animate = true;
            if (gameObject.name != "AverageGesture" && gameObject.name != "Preview")
            {
                timeIndicator.SetActive(true);
            }
        
            foreach (MeshRenderer mr in gameObject.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
            {
                Color temp = mr.material.color;
                temp.a = 0.25f;
                mr.material.color = temp;
            }
        }

    }

    public void Swing()
    {
        GameObject skeleton = gameObject.GetComponentInChildren<TrajectoryTR>().skeletonRef;
        Transform[] transforms = skeleton.GetComponentsInChildren<Transform>();

        Transform[] timeIndicatorTrans = new Transform[jointCount];
        if (gameObject.name != "AverageGesture")
        {
            timeIndicatorTrans = timeIndicator.GetComponentsInChildren<Transform>();
        }
        for (int i = 1; i < jointCount; i++)
        {
            transforms[i].localPosition = gesture.poses[currPoseIndex].joints[i - 1].ToVector();
            double distance = gesture.poses[currPoseIndex].timestamp / gesture.poses[gesture.poses.Count - 1].timestamp * 2.0f;
            if (gameObject.name != "AverageGesture")
            {
                timeIndicatorTrans[i].localPosition = gesture.poses[currPoseIndex].joints[i - 1].ToVector();
                timeIndicator.transform.localPosition = new Vector3(0, 0, 0.7f) + new Vector3(0, 0, (float)distance);
            }
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

        if (currQuat.eulerAngles == Vector3.zero)
            currQuat = rightController.rotation;

        GetRotateDirection(lastQuat, currQuat);
        if (rotate != 0)
        {
            if(rotate == -1)
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
        {
            if (gameObject.name != "AverageGesture")
            {
                timeIndicator.SetActive(false);
            }
         
            foreach (MeshRenderer mr in gameObject.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
            {
                Color temp = mr.material.color;
                temp.a = 1f;
                mr.material.color = temp;
            }
            swing = false;
            if (gameObject.name != "AverageGesture")
            {
                //uiRef.SlidimationIndicator.SetActive(false);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
        }
        else
        {
            if (gameObject.name != "AverageGesture")
            {
                timeIndicator.SetActive(true);
            }
            
            foreach (MeshRenderer mr in gameObject.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
            {
                Color temp = mr.material.color;
                temp.a = 0.25f;
                mr.material.color = temp;
            }
            swing = true;
            if (gameObject.name != "AverageGesture")
            {
                // uiRef.SlidimationIndicator.SetActive(true);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
        }
    }
    public void ChangeCluster()
    {
        List<GameObject> selectionList = GestureVisualizer.instance.selectedGestures;
        if (selected)
        {
            if (gameObject.name != "AverageGesture")
            {
                //uiRef.ChangingClusterIndicator.SetActive(false);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            selected = false;
            //uiRef.GetComponent<MeshRenderer>().material.color = GestureVisualizer.instance.UpdateGlowingFieldColour(gameObject) ;
            if (gameObject.name == "AverageGesture")
            {
                GestureGameObject[] children = gameObject.transform.parent.GetComponentsInChildren<GestureGameObject>(true);
                foreach (GestureGameObject gGO in children)
                {
                    if (gGO.gameObject.name != "AverageGesture")
                    {
                        if (selectionList.Contains(gGO.gameObject))
                        {
                            //uiRef.GetComponent<MeshRenderer>().material.color = GestureVisualizer.instance.UpdateGlowingFieldColour(gGO.gameObject);
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
            if (gameObject.name != "AverageGesture")
            {
                //uiRef.ChangingClusterIndicator.SetActive(true);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            selected = true;
            gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.white;
            //uiRef.GetComponent<MeshRenderer>().material.color = Color.white;

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
        if (gameObject.name != "AverageGesture")
        {
            //uiRef.SmallmultiplesIndicator.SetActive(isActive);
            //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
        }
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
        if (gameObject.name != "AverageGesture")
        {
            //uiRef.StackingIndicator.SetActive(true);
            //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
        }
        List<GameObject> stackedList = GestureVisualizer.instance.stackedObjects;
        if (!stackedList.Contains(gameObject))
        {
            initPos = gameObject.transform.localPosition;
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

        gameObject.transform.localPosition = initPos;
        if (gameObject.name != "AverageGesture")
        {
            //uiRef.StackingIndicator.SetActive(false);
            //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
        }
        else
        {
            gameObject.transform.localScale = sizeB4Stack;
        }
    }
    public void RevertComparing()
    {
        selected = false;
        if (gameObject.name != "AverageGesture")
        {
            //uiRef.ComparisonIndicator.SetActive(false);
            //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
        }
        gameObject.transform.localPosition = initPos;
        if (GestureVisualizer.instance.arrangementMode == 2)
        {
            gameObject.transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 1, 0));
        }
    }

    public void IdleSelect()
    {
        arrow.SetActive(!arrow.activeSelf);
        //uiRef.arrow.SetActive(arrow.activeSelf);
        //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
    }
}
  