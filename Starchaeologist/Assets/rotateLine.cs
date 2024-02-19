using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class rotateLine : MonoBehaviour
{
    private ActionBasedController controller;
    private XRBaseInteractor interactor;
    // Start is called before the first frame update
    void Start()
    {
        //Vector2 joystickInput = interactor.GetComponent<XRController>().controllerNode == UnityEngine.XR.XRNode.LeftHand;

        controller = GetComponent<ActionBasedController>();
        float z = Random.Range(0, 360);
        Debug.Log(z);
        this.transform.rotation = Quaternion.Euler(0, 0, z);
        //Debug.Log(transform.rotation.z);
    }

    // Update is called once per frame
    void Update()
    {
        /* get the controller
         * on input adjust the rotation of the 
         * line
         */
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, -0.1f);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, 0.1f);
        }
    }
}
