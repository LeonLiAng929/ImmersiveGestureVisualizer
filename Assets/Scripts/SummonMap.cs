using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SummonMap : MonoBehaviour
{
    public InputActionReference secondaryButtonPressedRef;
    public InputActionReference zoomOutRef;
    public InputActionReference zoomInRef;
    public Camera mapCam;
    // Start is called before the first frame update
    private void Awake()
    {
        secondaryButtonPressedRef.action.started += Summon;
        zoomOutRef.action.started += ZoomOut;
        zoomInRef.action.started += ZoomIn;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ZoomIn(InputAction.CallbackContext ctx)
    {
        if (gameObject.activeSelf)
            mapCam.orthographicSize -= 1;
    }

    private void ZoomOut(InputAction.CallbackContext ctx)
    {
        if (gameObject.activeSelf)
            mapCam.orthographicSize += 1;
    }
    private void Summon(InputAction.CallbackContext ctx)
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);
    }
}
