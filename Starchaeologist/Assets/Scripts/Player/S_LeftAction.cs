using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class S_LeftAction : MonoBehaviour
{
    ActionBasedController controller;
    XRRayInteractor teleportRay;
    XRInteractorLineVisual teleportLine;
    GameObject teleportReticle;
    float activationThreshold = 0.1f;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();
        teleportRay = GetComponent<XRRayInteractor>();
        teleportLine = GetComponent<XRInteractorLineVisual>();
        teleportReticle = gameObject.transform.GetChild(0).gameObject;


        controller.selectActionValue.action.performed += Action_Selec_Value;
    }

    private void Action_Selec_Value(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(controller.selectActionValue.action.ReadValue<float>() > activationThreshold)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(teleportRay && teleportLine)
        {
            teleportLine.enabled = isActive;
            teleportRay.enabled = isActive;
            teleportReticle.SetActive(isActive);
        }
    }
}
