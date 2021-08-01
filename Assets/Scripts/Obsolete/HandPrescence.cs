using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPrescence : MonoBehaviour
{
    // Start is called before the first frame update
    private InputDevice target;
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        //InputDevices.GetDevices(devices);
        InputDeviceCharacteristics rightHand = InputDeviceCharacteristics.Right;
        InputDevices.GetDevicesWithCharacteristics(rightHand, devices);
        target = devices[0];
    }

    // Update is called once per frame
    void Update()
    {
        target.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        if (primaryButtonValue)
        {
            Debug.Log("pressed!");
        }
    }
}
