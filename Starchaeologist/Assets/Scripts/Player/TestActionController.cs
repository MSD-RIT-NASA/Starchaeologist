using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestActionController : MonoBehaviour
{
    private ActionBasedController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();

        bool isPressed = controller.selectAction.action.ReadValue<bool>();


        controller.selectAction.action.performed += Action_Select;

        controller.activateAction.action.performed += Action_Trigger;

        controller.activateActionValue.action.performed += Action_Value_Trigger;



    }

    private void Action_Trigger(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("trigger Button is Pressed");
    }
    private void Action_Value_Trigger(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("trigger Button is Pressing");
        Debug.Log(controller.activateActionValue.action.ReadValue<float>());
    }

    private void Action_Select(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Select Button is Pressed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
