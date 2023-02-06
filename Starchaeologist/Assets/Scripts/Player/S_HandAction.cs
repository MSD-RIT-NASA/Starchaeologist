using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class S_HandAction : MonoBehaviour
{
    /*Description
     
     This script is used to create custom actions for the XR rig,
    This will be attached to each hand of the XR rig
     
     */

    ActionBasedController controller;
    XRRayInteractor teleportRay;
    XRInteractorLineVisual teleportLine;
    GameObject teleportReticle;
    float activationThreshold = 0.2f;
    bool teleportActive = false;
    //enable this if this script is attached to the left hand of the rig
    //This is enabled automatically in the prefab
    public bool leftHand = false;
    //leave alone in the editor
    public bool paused = false;

    /*PLAYER CONTROLS

    -teleport
        -trigger
    -whip
        -right hand a button (primary action)
        -hold (a), swing controller, release (a)
    -Pause/menu
        -left hand menu button
        -changes hand rays to UI interactables
    -menu interaction
        -primary action (a/x) each hand
    -grab
        -grip button
     
     */


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
            PauseMenu.singleton.Resume();
            rightHand.paused = false;
            rightRay.SetActive(false);
            leftRay.SetActive(false);
        }
        else//open the pause menu
        {
            Debug.Log("Pause");
            paused = true;
            PauseMenu.singleton.Pause();
            rightHand.paused = true;
            rightRay.SetActive(true);
            leftRay.SetActive(true);
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
