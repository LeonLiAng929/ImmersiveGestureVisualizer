using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class test2 : MonoBehaviour
{
    // Start is called before the first frame update
    InputDevice leftController;
    Quaternion last;
    void Start()
    {
        
        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);

        if (leftHandDevices.Count == 1)
        {
            leftController = leftHandDevices[0];
            leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out last);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion value;
        if (leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out value))
        {
            Debug.Log(value.eulerAngles);
            Debug.Log(GetRotateDirection(last, value));
        }
        last = value;
    }

    bool GetRotateDirection(Quaternion from, Quaternion to)
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
        return (clockWise <= counterClockWise);
    }
}
