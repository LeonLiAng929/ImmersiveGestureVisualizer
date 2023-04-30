using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class DatasetPreview : MonoBehaviour
{
    public string referentName;
    public string displayName;
    public bool userStudyData; 
    public bool trainingData;
    public bool curr = false;
    [SerializeField]
    public List<string> userStudyReferents = new List<string>();
    protected XRSimpleInteractable xRSimpleInteractable;
    public Gesture averageGesture;
    public GameObject prefab;
    private bool animate = false;
    [HideInInspector]
    public GameObject previewRepr;
    private int init = 1;
    private float timer = 0.0f;
    private float currTime = 0;
    private int counter = 0;
    private float prevTimestamp;
    private int numOfGestures;
    private bool needReassign = false;

    // Start is called before the first frame update
    void Awake()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.firstHoverEntered.AddListener(PreviewDataset);
        xRSimpleInteractable.lastHoverExited.AddListener(QuitPreview);
        xRSimpleInteractable.selectExited.AddListener(LoadDatasetInScene);

        GestureAnalyser gestureAnalyser = GestureAnalyser.instance;
        if (userStudyData) {
            try
            {
                gestureAnalyser.LoadUserStudyData(userStudyReferents);
                numOfGestures = gestureAnalyser.GetGestureCount();
                gestureAnalyser.CalculateAverageGestureForDataset();
                averageGesture = gestureAnalyser.GetGlobalAverageGesture();

                InstantiatePreview();

                if (!curr)
                {
                    gameObject.SetActive(false);
                }
            }
            catch (System.NullReferenceException)
            {
                needReassign = true;
            }
        }
        else
        {
            try
            {
                gestureAnalyser.LoadData(referentName);
                numOfGestures = gestureAnalyser.GetGestureCount();
                gestureAnalyser.CalculateAverageGestureForDataset();
                averageGesture = gestureAnalyser.GetGlobalAverageGesture();

                InstantiatePreview();

                if (!curr)
                {
                    gameObject.SetActive(false);
                }
            }
            catch (System.NullReferenceException)
            {
                needReassign = true;
            }
        }
    }

    private void Start()
    {
        if (needReassign)
        {
            GestureAnalyser gestureAnalyser = GestureAnalyser.instance;
            if (userStudyData)
            {
                gestureAnalyser.LoadUserStudyData(userStudyReferents);
            }
            else
            {
                gestureAnalyser.LoadData(referentName);
            }
            numOfGestures = gestureAnalyser.GetGestureCount();
            gestureAnalyser.CalculateAverageGestureForDataset();
            averageGesture = gestureAnalyser.GetGlobalAverageGesture();

            InstantiatePreview();

            if (!curr)
            {
                gameObject.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            AnimationConditionUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (animate)
        {
            Animate();
        }
    }

    public void ActivateAnimate()
    {
        if (animate)
        {
            animate = false;
        }
        else
        {
            animate = true;
        }

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

        Transform[] transforms = previewRepr.transform.Find("Skeleton").GetComponentsInChildren<Transform>();

        for (int i = 1; i < averageGesture.poses[0].num_of_joints + 1; i++)
        {
            Vector3 start_pos = transforms[i].localPosition;
            Vector3 end_pos = averageGesture.poses[counter].joints[i - 1].ToVector();

            if (currTime - prevTimestamp > 0)
            {
                float ratio = (Extension.SecondsToMs(timer) - prevTimestamp) / (currTime - prevTimestamp);

                Vector3 interpolation = Vector3.Lerp(start_pos, end_pos, ratio);

                transforms[i].localPosition = interpolation - new Vector3(0, 0, interpolation.z);
            }
        }
    }

    public void AnimationConditionUpdate()
    {
        float lastTimeStamp = (float)averageGesture.poses[averageGesture.poses.Count - 1].timestamp;
        if (Extension.SecondsToMs(timer) > lastTimeStamp)
        {
            AnimationModeReset();
        }
        if (Extension.SecondsToMs(timer) > currTime)
        {
            prevTimestamp = currTime;
            UpdateCounter();
            currTime = (float)averageGesture.poses[counter].timestamp;
        }
    }

    public void UpdateCounter()
    {
        counter += 1;
    }

    public void AnimationModeReset()
    {

        Transform[] transforms = previewRepr.transform.Find("Skeleton").GetComponentsInChildren<Transform>();
        for (int i = 1; i < averageGesture.poses[0].num_of_joints + 1; i++)
        {
            transforms[i].localPosition = averageGesture.poses[0].joints[i - 1].ToVector();
        }
        timer = 0f;
        currTime = 0;
        counter = 0;
        init = 1;
        //UpdateCounter();
    }
    public void InstantiatePreview()
    {
        prefab.SetActive(true);
        previewRepr = Instantiate(prefab, this.transform);
        GameObject skeleton = previewRepr.transform.Find("Skeleton").gameObject;
        Gesture TwoDimensionalGesture = averageGesture;
        MeshRenderer[] transforms = skeleton.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < transforms.Length; i++)
        {
            Vector3 pos = TwoDimensionalGesture.poses[0].joints[i].ToVector();

            transforms[i].transform.localPosition = pos - new Vector3(0, 0, pos.z);
        }

        previewRepr.transform.Find("GestureInfo").gameObject.GetComponent<TextMesh>().text = "Referent: " + displayName + "\n Number of gestures: "
            + numOfGestures.ToString();
        prefab.SetActive(false);
    }


    public void QuitAnimationMode()
    {
        Transform[] transforms = previewRepr.transform.Find("Skeleton").GetComponentsInChildren<Transform>();

        for (int i = 1; i < averageGesture.poses[0].num_of_joints + 1; i++)
        {
            Vector3 pos = averageGesture.poses[0].joints[i - 1].ToVector();
            transforms[i].localPosition = pos;
        }
        timer = 0f;
        currTime = 0;
        counter = 0;
        init = 1;
    }
    public void LoadDatasetInScene(SelectExitEventArgs args)
    {

        FadeScreen.instance.FadeOut();
    

        GestureAnalyser gestureAnalyser = GestureAnalyser.instance;
        if (userStudyData) { gestureAnalyser.LoadUserStudyData(userStudyReferents); }
        else
        {
            gestureAnalyser.LoadData(referentName);
        }
        GestureVisualizer gestureVisualizer = GestureVisualizer.instance;
        ClusteringMethods prevMethod = gestureVisualizer.clusteringMethod;
        ClusteringRationales prevRationale = gestureVisualizer.clusteringRationale;
        if (trainingData)
        {
            gestureVisualizer.k = 5;
            gestureVisualizer.clusteringRationale = ClusteringRationales.DBA;
            gestureVisualizer.clusteringMethod = ClusteringMethods.K_Means;
            gestureVisualizer.DestroyAllClusters();
            gestureVisualizer.startup = true;


            StartCoroutine(gestureVisualizer.InitializeVisualization(true));
        }
        else
        {
            gestureVisualizer.k = 1;
            gestureVisualizer.clusteringRationale = ClusteringRationales.DBA;
            gestureVisualizer.clusteringMethod = ClusteringMethods.K_Means;
            gestureVisualizer.DestroyAllClusters();
            gestureVisualizer.startup = true;


            StartCoroutine(gestureVisualizer.InitializeVisualization());
        }
        gestureVisualizer.clusteringMethod = prevMethod;
        gestureVisualizer.clusteringRationale = prevRationale;


    }

    public void PreviewDataset(HoverEnterEventArgs args)
    {
        ActivateAnimate();
    }

    public void QuitPreview(HoverExitEventArgs args)
    {
        ActivateAnimate();
        QuitAnimationMode();
    }
}
