using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ChangeArrangement : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    #region Singleton
    public static ChangeArrangement instance;
    #endregion

    public Material global;
    public Material local;
    public Material lineUp;
    public Material PCA;
    public Material MDS;

    private int mode = 3; //0 = local, 1=global, 2 = lineUp, 3 = PCA
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(Rearrange);
        gameObject.GetComponent<MeshRenderer>().material = PCA;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Rearrange(SelectExitEventArgs arg)
    {
        Dictionary<int, GameObject> clusterObjs = GestureVisualizer.instance.GetClusterObjs();
        if (mode == 0)
        {
            mode = 2;
            gameObject.GetComponent<MeshRenderer>().material = local;
            GestureVisualizer.instance.arrangementMode = 0;
            GestureVisualizer.instance.AdjustClusterPosition();

            UserStudy.instance.IncrementCount(Actions.LocalArrangement);
        }
        else if (mode == 1){
            mode = 0;
            gameObject.GetComponent<MeshRenderer>().material = global;
            GestureVisualizer.instance.arrangementMode = 1;
            GestureVisualizer.instance.AdjustClusterPosition();

            UserStudy.instance.IncrementCount(Actions.GlobalArrangement);
        }
        else if ( mode == 2)
        {
            mode = 3;
            gameObject.GetComponent<MeshRenderer>().material = lineUp;
            GestureVisualizer.instance.arrangementMode = 2;
            GestureVisualizer.instance.AdjustClusterPosition();

            UserStudy.instance.IncrementCount(Actions.Line_Up_Arrangement);
        }
        else if (mode == 3)
        {
            mode = 4;
            gameObject.GetComponent<MeshRenderer>().material = PCA;
            GestureVisualizer.instance.arrangementMode = 3;
            GestureVisualizer.instance.AdjustClusterPosition();

            UserStudy.instance.IncrementCount(Actions.PCA_Arrangement);
        }
        else if (mode == 4)
        {
            mode = 1;
            gameObject.GetComponent<MeshRenderer>().material = MDS;
            GestureVisualizer.instance.arrangementMode = 4;
            GestureVisualizer.instance.AdjustClusterPosition();

            UserStudy.instance.IncrementCount(Actions.MDS_Arrangement);
        }
    }
}
