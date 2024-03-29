using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class GestureGameObject : MonoBehaviour
{
    public Gesture gesture;
    public Gesture2DObject uiRef;
    public Transform rightController; // just in case if some controllers cant be dectected using XR.commam usages.

    [SerializeField]
    protected XRSimpleInteractable xRSimpleInteractable;

    private float timer = 0.0f; // the time since start() in seconds.
    private int init = 1;
    public bool animate = false;
    public bool swing = false;
    public bool stacked = false;
    public bool selected = false;
    public bool averageGesture = false;
    public bool previoulyInAnimationMode = false;
    private float currTime = 0;
    private int counter = 0;
    private float prevTimestamp;
    public Vector3 initPos;
    public Vector3 sizeB4Stack;
    public Quaternion lastQuat;
    public GameObject arrow;
    public bool inComparison = false;
    public int rotate = 0;
    public int currPoseIndex = 0;

    public Vector3 initSize;
    // Update is called once per frame

    // a skeleton to link the animation and the slidimation with the small-multiples
    private GameObject timeIndicator;

    private void Start()
    {
        arrow = gameObject.transform.Find("Arrow").gameObject;
        arrow.SetActive(false);
        if (gameObject.name != "Gesture")
        {
            int jointCount = gesture.poses[0].num_of_joints;
            GameObject multiples = GetComponent<Transform>().Find("SmallMultiples").gameObject;
            timeIndicator = Instantiate(GestureVisualizer.instance.skeletonModel, multiples.transform);
            timeIndicator.name = "TimeIndicator";
            Transform[] transforms = timeIndicator.GetComponentsInChildren<Transform>();
            transforms[0].localPosition = new Vector3(0, 0, 0.7f);

            for (int i = 1; i < jointCount +1; i++)
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
        if(ActionSwitcher.instance.GetCurrentAction() == Actions.Idle)
        {
            if (previoulyInAnimationMode)
            {
                previoulyInAnimationMode = false;
                QuitAnimationMode();
            }
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

    public void DisplayConnection()
    {
        if (!averageGesture)
        {
            GameObject connection = transform.Find("Connection").gameObject;
            connection.SetActive(!connection.activeSelf);
        }
    }
    public void PerformAction(SelectExitEventArgs arg)
    {
        Actions curr = ActionSwitcher.instance.GetCurrentAction();
        if (curr == Actions.Animate)
        {
            if (inComparison)
            {
                foreach(GameObject gO in GestureVisualizer.instance.selectedGestures)
                {
                    gO.GetComponent<GestureGameObject>().ActivateAnimate(false);
                }
            }
            else { ActivateAnimate(false); }
        }
        else if (curr == Actions.ChangeCluster) { ChangeCluster(); }
        else if (curr == Actions.SmallMultiples) {
            if (inComparison)
            {
                foreach (GameObject gO in GestureVisualizer.instance.selectedGestures)
                {
                    gO.GetComponent<GestureGameObject>().ShowSmallMultiples(false);
                }
            }
            else
            {
                ShowSmallMultiples(false);
            } }
        else if (curr == Actions.Swing)
        {
            if (inComparison)
            {
                foreach (GameObject gO in GestureVisualizer.instance.selectedGestures)
                { gO.GetComponent<GestureGameObject>().previoulyInAnimationMode = true;
                    gO.GetComponent<GestureGameObject>().ActivateSwinging(false);
                    GestureVisualizer.instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out gO.GetComponent<GestureGameObject>().lastQuat);
                }
            }
            else
            {
                previoulyInAnimationMode = true; ActivateSwinging(false);
                GestureVisualizer.instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out lastQuat);
            }
        }
        else if (curr == Actions.StackGestures) { StackGesture(false); }
        else if (curr == Actions.CloseComparison)
        {
            /*if (arg.interactor.gameObject.name == "LeftHand Controller")
                CloseComparison(true);
            else if (arg.interactor.gameObject.name == "RightHand Controller")
            {
                CloseComparison(false);
            }*/
            CloseComparison();
        }
        else if (curr == Actions.Mark)
        {
            IdleSelect();
        }
    }

    public void CloseComparison()
    {
        //if ((lefthand && !GestureVisualizer.instance.leftHandSelected) || (!lefthand && !GestureVisualizer.instance.rightHandSelected))
        List<GameObject> selectionList = GestureVisualizer.instance.selectedGestures;
        if (selected)
        {
            inComparison = false;
            if (!averageGesture)
            {
                try
                {
                    uiRef.ComparisonIndicator.SetActive(false);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                }
                catch (MissingReferenceException)
                { }
            }
            selected = false;
            if (selectionList.Contains(gameObject))
            {
                if (!averageGesture)
                {
                    try
                    {
                        uiRef.GetComponent<MeshRenderer>().material.color = GestureVisualizer.instance.UpdateGlowingFieldColour(gameObject);
                    }
                    catch (MissingReferenceException)
                    {

                    }
                }
                selectionList.Remove(gameObject);
                /*if (lefthand)
                    GestureVisualizer.instance.leftHandSelected = false;
                else
                    GestureVisualizer.instance.rightHandSelected = false;*/
            }
        }
        else
        {
            inComparison = true;
            if (!averageGesture)
            {
                try
                {
                    uiRef.ComparisonIndicator.SetActive(true);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                }
                catch (MissingReferenceException)
                {

                }
            }
            selected = true;
            if (!selectionList.Contains(gameObject))
            {
                selectionList.Add(gameObject);
                gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.white;
                /*if (lefthand)
                {
                    gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.white;
                    GestureVisualizer.instance.leftHandSelected = true;
                }
                else
                {
                    gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.black;
                    GestureVisualizer.instance.rightHandSelected = true;
                }*/
            }

        }
        initPos = gameObject.transform.localPosition;
        if (averageGesture)
        {
            initSize = gameObject.transform.localScale;
        }
        /*if (GestureVisualizer.instance.rightHandSelected && GestureVisualizer.instance.leftHandSelected)
        {
            GestureVisualizer.instance.CloseComparison();
        }*/
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

    public void QuitAnimationMode()
    {
        Transform[] transforms = GetComponent<Transform>().Find("Trajectory").Find("Skeleton").GetComponentsInChildren<Transform>();
        if (!averageGesture)
        {
            try
            {
                Transform[] twoDSkeletonTrans = uiRef.TwoDModel.GetComponentsInChildren<Transform>();
                for (int i = 1; i < gesture.poses[0].num_of_joints + 1; i++)
                {
                    Vector3 pos = gesture.poses[0].joints[i - 1].ToVector();

                    twoDSkeletonTrans[i].localPosition = pos - new Vector3(0, 0, pos.z);
                }
            }
            catch (MissingReferenceException) { }
        }
        ResetTrajectories();
        for (int i = 1; i < gesture.poses[0].num_of_joints + 1; i++)
        {
            Vector3 pos = gesture.poses[0].joints[i - 1].ToVector();
            transforms[i].localPosition = pos;
        }
        timer = 0f;
        currTime = 0;
        counter = 0;
        init = 1;
        //UpdateCounter();
        GameObject trajObj = transform.Find("Trajectory").gameObject;
        TrajectoryTR traj = trajObj.GetComponent<TrajectoryTR>();
        traj.DrawTrajectory(GestureVisualizer.instance.trajectoryColorSet);

    }
    public void Animate()
    {
        if (init == 1)
        {
            ResetTrajectories();
            init = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }

        Transform[] transforms= GetComponent<Transform>().Find("Trajectory").Find("Skeleton").GetComponentsInChildren<Transform>();
        //GameObject multiples = GetComponent<Transform>().Find("SmallMultiples").gameObject;
        Transform[] twoDSkeletonTrans = null;
        if (!averageGesture)
        {
            try
            {
                twoDSkeletonTrans = uiRef.TwoDModel.GetComponentsInChildren<Transform>();
            }
            catch (MissingReferenceException) { }
        }

        int jointCount = gesture.poses[0].num_of_joints;
        //Transform[] timeIndicatorTrans = new Transform[jointCount+1];
        //if (!averageGesture)
        //{
        //timeIndicatorTrans = timeIndicator.GetComponentsInChildren<Transform>();
        //}
        Transform[] timeIndicatorTrans = timeIndicator.GetComponentsInChildren<Transform>();
        for (int i = 1; i < gesture.poses[0].num_of_joints + 1; i++)
        {
            Vector3 start_pos = transforms[i].localPosition;
            Vector3 end_pos = gesture.poses[counter].joints[i-1].ToVector();
            string jointType = gesture.poses[counter].joints[i - 1].jointType;
            if (currTime - prevTimestamp > 0)
            {
                float ratio = (Extension.SecondsToMs(timer) - prevTimestamp) / (currTime - prevTimestamp);
                
                Vector3 interpolation = Vector3.Lerp(start_pos, end_pos, ratio);

                transforms[i].localPosition = interpolation;
                UpdateTrajectoryByType(jointType, interpolation);
                if (twoDSkeletonTrans != null)
                {
                    twoDSkeletonTrans[i].localPosition = interpolation - new Vector3(0, 0, interpolation.z);
                }
                double distance = currTime / gesture.poses[gesture.poses.Count - 1].timestamp * 2.0f;
                //if (!averageGesture)
                //{
                //  timeIndicatorTrans[i].localPosition = interpolation;
                //timeIndicator.transform.localPosition = new Vector3(0, 0, 0.7f) + new Vector3(0, 0, (float)distance);
                //}
                timeIndicatorTrans[i].localPosition = interpolation;
                timeIndicator.transform.localPosition = new Vector3(0, 0, 0.7f) + new Vector3(0, 0, (float)distance);
            }
        }
    }

    public void UpdateTrajectoryByType(string type, Vector3 joint)
    {
        TubeRenderer tr = gameObject.transform.Find("Trajectory").Find("LineRanderers").Find(type).GetComponent<TubeRenderer>();
        if (tr.gameObject.activeSelf)
        {
            Vector3[] old = tr.points;
            Vector3[] update = new Vector3[] { joint };
            Vector3[] newPoints = new Vector3[old.Length + update.Length];
            old.CopyTo(newPoints, 0);
            update.CopyTo(newPoints, old.Length);
            tr.points = newPoints;
        }
    }

    public void ResetTrajectories()
    {
        TubeRenderer[] trs = gameObject.transform.Find("Trajectory").GetComponentsInChildren<TubeRenderer>();
        foreach (TubeRenderer tr in trs)
        {
            tr.points = new Vector3[] { };
        }
    }
    public void ActivateAnimate(bool calledByParent)
    {
        previoulyInAnimationMode = true;
        if (animate)
        {
            if (!averageGesture && gameObject.name != "Preview")
            {
                try
                {
                    uiRef.AnimationIndicator.SetActive(false);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                }
                catch (MissingReferenceException) { }
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
            if (!averageGesture && gameObject.name != "Preview")
            {
                try
                {
                    uiRef.AnimationIndicator.SetActive(true);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                }
                catch (MissingReferenceException) { }
            }
            animate = true;
            if (!calledByParent && gameObject.name != "Preview") 
                UserStudy.instance.IncrementCount(Actions.Animate);
            if (gameObject.name != "Preview")
            {
                timeIndicator.SetActive(true);
            }
        
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
        int jointCount = gesture.poses[0].num_of_joints;
        Transform[] timeIndicatorTrans = timeIndicator.GetComponentsInChildren<Transform>();
        Transform[] twoDSkeletonTrans = null;
        if (!averageGesture)
        {
            try
            {
                twoDSkeletonTrans = uiRef.TwoDModel.GetComponentsInChildren<Transform>();
            }
            catch (MissingReferenceException) { }

        }
        for (int i = 1; i < jointCount+1; i++)
        {
            Vector3 pos = gesture.poses[currPoseIndex].joints[i - 1].ToVector();
            transforms[i].localPosition = pos;
            if (twoDSkeletonTrans != null)
                twoDSkeletonTrans[i].localPosition = pos - new Vector3(0, 0, pos.z);
            double distance = gesture.poses[currPoseIndex].timestamp / gesture.poses[gesture.poses.Count - 1].timestamp * 2.0f;
            //if (!averageGesture)
            //{
                timeIndicatorTrans[i].localPosition = gesture.poses[currPoseIndex].joints[i - 1].ToVector();
                timeIndicator.transform.localPosition = new Vector3(0, 0, 0.7f) + new Vector3(0, 0, (float)distance);
            //}
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
    public void ActivateSwinging(bool calledByParent) //if this action is invoked by its cluster, then do not count the action twice for user study.
    {
        if (swing)
        {
            //if (!averageGesture)
            //{
                timeIndicator.SetActive(false);
            //}
         
            foreach (MeshRenderer mr in gameObject.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
            {
                Color temp = mr.material.color;
                temp.a = 1f;
                mr.material.color = temp;
            }
            swing = false;
            if (!averageGesture)
            {
                try
                {
                    uiRef.SlidimationIndicator.SetActive(false);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                }
                catch (MissingReferenceException) { }
            }
        }
        else
        {
            //if (!averageGesture)
            //{
                timeIndicator.SetActive(true);
            //}
            
            foreach (MeshRenderer mr in gameObject.transform.Find("Trajectory").Find("LineRanderers").GetComponentsInChildren<MeshRenderer>())
            {
                Color temp = mr.material.color;
                temp.a = 0.25f;
                mr.material.color = temp;
            }
            swing = true;
            if(!calledByParent)
                UserStudy.instance.IncrementCount(Actions.Swing);
            if (!averageGesture)
            {
                try
                {
                    uiRef.SlidimationIndicator.SetActive(true);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                }
                catch (MissingReferenceException) { }
            }
        }
    }
    public void ChangeCluster()
    {
        List<GameObject> selectionList = GestureVisualizer.instance.selectedGestures;
        if (selected)
        {
            selected = false;
            if (averageGesture)
            {
                GestureGameObject[] children = gameObject.transform.parent.GetComponentsInChildren<GestureGameObject>(true);
                foreach (GestureGameObject gGO in children)
                {
                    if (!gGO.averageGesture)
                    {
                        if (selectionList.Contains(gGO.gameObject))
                        {
                            try
                            {
                                uiRef.GetComponent<MeshRenderer>().material.color = GestureVisualizer.instance.UpdateGlowingFieldColour(gGO.gameObject);
                            }
                            catch (MissingReferenceException) { }
                            selectionList.Remove(gGO.gameObject);
                            gGO.selected = false;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    uiRef.ChangingClusterIndicator.SetActive(false);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                    uiRef.GetComponent<MeshRenderer>().material.color = GestureVisualizer.instance.UpdateGlowingFieldColour(gameObject);
                }
                catch (MissingReferenceException) { }
                if (selectionList.Contains(gameObject))
                    selectionList.Remove(gameObject);
            }

        }
        else
        { 
            selected = true;
            gameObject.transform.Find("GlowingField").GetComponent<MeshRenderer>().material.color = Color.white; 

            if (averageGesture)
            {
                GestureGameObject[] children = gameObject.transform.parent.GetComponentsInChildren<GestureGameObject>(true);
                foreach(GestureGameObject gGO in children)
                {
                    if (!gGO.averageGesture)
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
                try
                {
                    uiRef.ChangingClusterIndicator.SetActive(true);
                    //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
                    uiRef.GetComponent<MeshRenderer>().material.color = Color.white;
                }
                catch (MissingReferenceException) { }
                if (!selectionList.Contains(gameObject))
                    selectionList.Add(gameObject);
            }
        }
    }
    public void ShowSmallMultiples(bool calledByParent)
    {
        GameObject multiples = GetComponent<Transform>().Find("SmallMultiples").gameObject;
        bool isActive = !multiples.activeSelf;
        multiples.SetActive(isActive);
        if (isActive && !calledByParent)
            UserStudy.instance.IncrementCount(Actions.SmallMultiples);

        if (!averageGesture)
        {
            try
            {
                uiRef.SmallmultiplesIndicator.SetActive(isActive);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            catch (MissingReferenceException) { }
        }
    }
    public void ShowTracer()
    {
        GameObject tracers = GetComponent<Transform>().Find("Trajectory").Find("Tracers").gameObject;
        bool isActive = !tracers.activeSelf;
        tracers.SetActive(isActive);
    }
    public void StackGesture(bool calledByParent)
    {
        stacked = true;
        if (!averageGesture)
        {
            try
            {
                uiRef.StackingIndicator.SetActive(true);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            catch (MissingReferenceException) { }
        }
        List<GameObject> stackedList = GestureVisualizer.instance.stackedObjects;
        if (!stackedList.Contains(gameObject))
        {
            initPos = gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
            stackedList.Add(gameObject);
            if (averageGesture)
            {
                sizeB4Stack = gameObject.transform.localScale;
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        GestureVisualizer.instance.PrepareStack();
        if (!calledByParent)
            UserStudy.instance.IncrementCount(Actions.StackGestures);
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
        if (!averageGesture)
        {
            try
            {
                uiRef.StackingIndicator.SetActive(false);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            catch (MissingReferenceException) { }
        }
        else
        {
            gameObject.transform.localScale = sizeB4Stack;
        }
    }
    public void RevertComparing()
    {
        selected = false;
        if (!averageGesture)
        {
            try
            {
                uiRef.ComparisonIndicator.SetActive(false);
                //GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            catch (MissingReferenceException) { }
        }
        else
        {
            gameObject.transform.localScale = initSize;
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
        if (!averageGesture)
        {
            try
            {
                uiRef.arrow.SetActive(arrow.activeSelf);
                GestureVisualizer.instance.oldBoardIndicatorUpdate(uiRef);
            }
            catch (MissingReferenceException) { }
        }
    }
}
  