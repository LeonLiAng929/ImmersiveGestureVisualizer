using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SummonMenu : MonoBehaviour
{
    public InputActionReference buttonPressedRef;
    // Start is called before the first frame update
    private void Awake()
    {
        buttonPressedRef.action.started += Summon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Summon(InputAction.CallbackContext ctx)
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);
    }
}
