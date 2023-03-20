using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VehicleController : MonoBehaviour
{
    //Controls
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private Camera PlayerCam;

    public Rigidbody carBody;

    public bool inControl = true;

    //Car Acceleration and Braking stats
    public float xTiltValue;
    public float zTiltValue;
    public float forwardTiltValue;
    public float acccel = 150f;
    public float breaking = 300f;
    public float turnAng = 10f;

    public float currentAccel = 0f;
    public float currentBreaking = 0f;
    public float turnPower = 5f;

    // Start is called before the first frame update
    void Start()
    {
        carBody.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(inControl)
        {
            //Headset tilt values from main camera 
            xTiltValue = PlayerCam.transform.localRotation.x;
            zTiltValue = -PlayerCam.transform.localRotation.z;

            currentAccel = acccel * xTiltValue + 175f;


            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Time.deltaTime * zTiltValue * turnPower * 10f, 0f));
            transform.position = carBody.transform.position;
        }
        else
        {
            //Slow to a stop
            currentAccel *= .05f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Time.deltaTime * zTiltValue * turnPower * 10f, 0f));
            transform.position = carBody.transform.position;
        }
    }

    private void FixedUpdate()
    {
        carBody.AddForce(transform.forward * currentAccel * 50f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("I Collided");
            carBody.velocity *= .05f;
        }
        if (collision.gameObject.name == "RaceFinish")
        {
            inControl = false;
        }
    }
}
