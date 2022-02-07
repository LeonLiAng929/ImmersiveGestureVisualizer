using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

public class SnapByDirection : MonoBehaviour
{
    public GameObject SnapOptions;
    Color initColor;
    public int direction;
    Quaternion temp = new Quaternion();

    // Start is called before the first frame update
    void Start()
    {
        initColor = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Grab")
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.name == "Grab")
        {
            bool leftTriggered;
            bool rightTriggered;
            GestureVisualizer.instance.leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out leftTriggered);
            GestureVisualizer.instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rightTriggered);
            if (!leftTriggered && !rightTriggered)
            {
                DirectionSnap(transform.parent.parent.parent, other.transform.parent, direction);
                gameObject.GetComponent<MeshRenderer>().material.color = initColor;
                transform.parent.parent.GetComponent<BoardSnap>().HideSnapOptions();
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Grab")
        {
            gameObject.GetComponent<MeshRenderer>().material.color = initColor;
        }
    }

    /// <summary>
    /// snap board2 to board 1 along direct
    /// </summary>
    /// <param name="board1"></param>
    /// <param name="board2"></param>
    /// <param name="direct"></param>
    public void DirectionSnap(Transform board1, Transform board2, int direct) // direction = 0,1,2,3 |up,down,left,right
    {
        Vector2 size1 = board1.Find("Grab").GetComponent<BoardSnap>().lengthByWidth;
        board2.rotation = board1.rotation;
        board2.position = board1.position;
        if (direct == 0)
        {
            board2.localPosition += board2.up.normalized * (size1.y + 0.2f);
            
        }
        else if (direct == 1)
        {
            board2.localPosition -= board2.up.normalized * (size1.y + 0.2f);
        }
        else if (direct == 2)
        {
            board2.localPosition -= board2.right.normalized * (size1.x + 0.2f);
        }
        else if (direct == 3)
        {
            board2.localPosition += board2.right.normalized * (size1.x + 0.2f);
        }
        board2.LookAt(Camera.main.transform, -1*board2.up);
        board2.Rotate(new Vector3(180, 0, 0), Space.Self);
    }

    
}
