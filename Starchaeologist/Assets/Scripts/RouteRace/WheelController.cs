using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;


public class WheelController : MonoBehaviour
{
    //Controls
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private Camera PlayerCam;

    //Wheel references
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider backLeft;
    [SerializeField] WheelCollider backRight;


    //Car Acceleration and Braking stats
    public float acccel = 150f;
    public float breaking = 300f;
    public float turnAng = 10f;

    public float currentAccel = 0f;
    public float currentBreaking = 0f;
    public float currentTurnAng = 0f;

    private void OnEnable()
    {
        controller.selectAction.action.performed += OnControllerPerformed;
    }
    private void OnDisable()
    {
        controller.selectAction.action.performed -= OnControllerPerformed;
    }

    private void OnControllerPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.action);
    }

    private void Start()
    {
        PlayerCam = Camera.main;
    }

    private void FixedUpdate()
    {
        currentAccel = acccel * .2f + 100f;
        

        if(controller.selectAction.action.inProgress)
        {
            Debug.Log("true");
            currentBreaking = breaking;
        }
        else
        {
            currentBreaking = 0f;
        }

        //Steering
        currentTurnAng = turnAng * -PlayerCam.transform.localRotation.y;
        frontLeft.steerAngle = currentTurnAng;
        frontRight.steerAngle = currentTurnAng;

        frontLeft.motorTorque = currentAccel;
        frontRight.motorTorque = currentAccel;

        frontRight.brakeTorque = currentBreaking;
        frontLeft.brakeTorque = currentBreaking;
        backRight.brakeTorque = currentBreaking;
        backLeft.brakeTorque = currentBreaking;

    }
}
