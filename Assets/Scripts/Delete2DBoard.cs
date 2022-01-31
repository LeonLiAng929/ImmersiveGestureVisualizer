using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Delete2DBoard : MonoBehaviour
{
    protected XRSimpleInteractable xRSimpleInteractable;
    public GameObject board;
    // Start is called before the first frame update
    void Start()
    {
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectExited.AddListener(DeleteBoard);
    }


    public void DeleteBoard(SelectExitEventArgs args)
    {
        GestureVisualizer.instance.DestroyBoardRecord(board);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
