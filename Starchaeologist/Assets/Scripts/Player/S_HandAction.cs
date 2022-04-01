using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class S_HandAction : MonoBehaviour
{
    ActionBasedController controller;
    XRRayInteractor teleportRay;
    XRInteractorLineVisual teleportLine;
    GameObject teleportReticle;
    float activationThreshold = 0.2f;
    bool teleportActive = false;
    public bool leftHand = false;
    public bool paused = false;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();
        teleportRay = GetComponent<XRRayInteractor>();
        teleportLine = GetComponent<XRInteractorLineVisual>();
        teleportReticle = gameObject.transform.GetChild(0).gameObject;
        teleportReticle.SetActive(false);

        controller.selectActionValue.action.performed += Action_Selec_Value;
        if(leftHand)
        {
            controller.activateAction.action.performed += Action_Pause;
            transform.parent.GetChild(5).gameObject.SetActive(false);
            transform.parent.GetChild(6).gameObject.SetActive(false);
        }
    }

    //pause the game when the menu button is pressed (left hand only)
    private void Action_Pause(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //grab references
        S_HandAction rightHand =  transform.parent.GetChild(2).GetComponent<S_HandAction>();
        GameObject rightRay =  transform.parent.GetChild(5).gameObject;
        GameObject leftRay =  transform.parent.GetChild(6).gameObject;

        //close the pause menu
        if (paused)
        {
            Debug.Log("Unpause");
            paused = false;
            rightHand.paused = false;
            rightRay.SetActive(false);
            leftRay.SetActive(false);
            //TO DO: deactivate the actual pause menu
        }
        else//open the pause menu
        {
            Debug.Log("Pause");
            paused = true;
            rightHand.paused = true;
            rightRay.SetActive(true);
            leftRay.SetActive(true);
            //TO DO: activate the actual pause menu
        }

    }

    //when the player presses the trigger, read the value
    private void Action_Selec_Value(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(!paused)
        {
            //if the value is close to pressed, activate the teleportation ray
            if (controller.selectActionValue.action.ReadValue<float>() > activationThreshold)
            {
                teleportActive = true;
            }
            else
            {
                teleportActive = false;
            }
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        if(teleportRay && teleportLine)
        {
            teleportLine.enabled = teleportActive;
            teleportRay.enabled = teleportActive;
            if(!teleportActive)
            {
                teleportReticle.SetActive(teleportActive);
            }
            //teleportReticle.SetActive(isActive);
        }
    }
}
