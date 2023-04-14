using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Unity.XR.CoreUtils;
using TMPro;
public class S_RiverGame : S_RiverBuilder
{



    //singleton
    public static S_RiverGame singleton;
    [SerializeField] GameObject playerReference;
    [SerializeField] GameObject raftReference;
    [SerializeField] GameObject riverWater;
    S_Raft raftScript;

    [SerializeField] Camera vrCamera;
    [SerializeField] GameObject raftObject;
    [SerializeField] GameObject rightHandRay;
    [SerializeField] GameObject leftHandRay;
    [SerializeField] GameObject countdownCanvas;
    [SerializeField] GameObject readyCanvas;

    private Quaternion vrCameraRotation;

    [SerializeField] GameObject raftEndPoint;
    public List<GameObject> riverReferences; //populated with positions while the river is being built from the S_RiverBuilder script
    Vector3 nextDestination;
    Vector3 currentDirection = new Vector3(0, 0, 1);
    Vector3 desiredDirection = new Vector3(0, 0, 1);
    [SerializeField] float raftAcceleration = 0.1f;
    [SerializeField] float raftSpeed = 3.0f;
    float currentSpeed = 0f;
    public bool timeToMove = false; //turned true the first time the player teleports to the raft from the S_RaftCollision script
    bool slowDown = false;
    bool playerAttached = false;
    int checkpointIndex = 0;

    [SerializeField] Timer timer;
    [SerializeField] UdpSocket server;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] GameObject timerCanvas;
    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject leftHand;

    //python variables
    /*
    PYTHON COMMUNICATION FORMAT
    Rotation
    -Send: 'rotation rotation1(float) rotation2(float)'
    -Receive: 'rotation rotation1(float) rotation2(float)'
    Score Calibration 
    -Send: 'calibrate gameScore(float)'
    -Receive: 'calibrateStop balanceScore(float)'
    Killswitch 
    -Receive 'kill'
    -Receive 'live'
    Quit Game
    -Send 'quit'
    -Receive 'quit'
    */
    //private HelloRequester pythonCommunicator;
    public float rotationX = 0f;
    public float rotationZ = 0f;
    public bool rotationChanged = false;

    //PythonCommunicator communicateReference;




    // Start is called before the first frame update
    void Start()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }

        //pythonCommunicator = new HelloRequester();
        raftScript = raftReference.transform.GetChild(1).GetComponent<S_Raft>();
        //communicateReference = GetComponent<PythonCommunicator>();
        //pythonCommunicator.Start();
        rightHand.SetActive(false);
        leftHand.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.TimeRemaining > 0)
        {
            countdownText.text = "" + ((int)timer.TimeRemaining + 1);
        }
        else
        {
            countdownCanvas.SetActive(false);
            readyCanvas.SetActive(true);
            rightHand.SetActive(true);
            leftHand.SetActive(true);
            rightHandRay.SetActive(true);
            leftHandRay.SetActive(true);
        }

        //start the game by moving the raft
        if (timeToMove)
        {
            Debug.Log(vrCamera.transform.position);
            //stick the player under the raft gameobject to help with movement
            if (!playerAttached)
            {
                raftScript.tilting = true;
                playerAttached = true;
                playerReference.transform.parent = raftReference.transform;
                playerReference.transform.position = raftReference.transform.GetChild(0).position;
                nextDestination = checkpoints[0];
            }
            MoveRaft();
            //make the raft rotate
            raftScript.Raft();
        }
        //python communication
        Communication();

    }


    void MoveRaft()
    {
        float distance = checkpoints[checkpointIndex].z - raftReference.transform.position.z;
        //accelerate or decelerate the raft
        if (currentSpeed != raftSpeed && !slowDown)
        {
            
            currentSpeed = currentSpeed + (raftAcceleration * Time.deltaTime * raftSpeed * 5f);

            //tilting affects for the python comunicator
            //raftScript.tiltRange = raftScript.tiltRange + (raftAcceleration * Time.deltaTime * raftScript.maxRange * 5f);
        }
        else if (slowDown)
        {
            if (distance <= 6.0f)
            {
                //using the distance and the current speed of the raft we can calcuate how great our acceleration vector needs to be to come to a stop
                raftAcceleration = Mathf.Abs((0 - currentSpeed) / (Mathf.Abs(2 * (raftObject.transform.position.z - raftEndPoint.transform.position.z))));
                Debug.Log("Current Acceleration:" + raftAcceleration);

                currentSpeed = currentSpeed - (raftAcceleration * Time.deltaTime * raftSpeed);
                Debug.Log("Current Speed:" + currentSpeed);
                raftScript.tiltRange = raftScript.tiltRange - (raftAcceleration * Time.deltaTime * raftScript.maxRange);
            }
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0.25f, raftSpeed);
        raftScript.tiltRange = Mathf.Clamp(raftScript.tiltRange, 0.25f, raftScript.maxRange);

        //Vector3.RotateTowards(raftReference.transform.position,checkpoints[checkpointIndex],2*Mathf.PI,Mathf.PI);

        desiredDirection = Vector3.Normalize(checkpoints[checkpointIndex] - raftReference.transform.position);
        

        //raftReference.transform.LookAt(checkpoints[checkpointIndex]);
        if (distance <= 7)
        {
            raftReference.transform.position += desiredDirection * currentSpeed * Time.deltaTime;

            /*Quaternion lookRotation = Quaternion.LookRotation(desiredDirection);
            raftReference.transform.rotation = Quaternion.Slerp(raftReference.transform.rotation, lookRotation, Time.deltaTime * (currentSpeed/6));
            raftReference.transform.position += raftReference.transform.forward * currentSpeed * Time.deltaTime;*/
        }
        else
        {
            //Quaternion toRotation = Quaternion.FromToRotation(raftReference.transform.forward, desiredDirection); // instead of LookRotation( )
            //raftReference.transform.rotation = Quaternion.Lerp(raftReference.transform.rotation, toRotation, (currentSpeed * Time.deltaTime));
            Quaternion lookRotation = Quaternion.LookRotation(desiredDirection);
            raftReference.transform.rotation = Quaternion.Slerp(raftReference.transform.rotation,lookRotation,Time.deltaTime* (currentSpeed/3));
            raftReference.transform.position += raftReference.transform.forward * currentSpeed * Time.deltaTime;
        }
        
        /*//calculating and normalizing the direction vector for each checkpoint
        Vector3 direction = checkpoints[checkpointIndex] - raftReference.transform.position;
        direction = direction.normalized;


        //getting the current direction we want to shift toward and the current direction
        desiredDirection = Vector3.Normalize(checkpoints[checkpointIndex] - raftReference.transform.position);

        currentDirection = Vector3.Normalize(currentDirection + (desiredDirection * Time.deltaTime * currentSpeed));

        //Vector3.Lerp(currentDirection, desiredDirection,currentSpeed*Time.deltaTime);

        Debug.Log(currentDirection);
        //move the raft
        //raftReference.transform.position = Vector3.MoveTowards(raftReference.transform.position,nextDestination,currentSpeed * Time.deltaTime);
        //raftReference.transform.position = Vector3.Lerp(raftReference.transform.position, nextDestination, currentSpeed * Time.deltaTime);
        raftReference.transform.position += currentDirection * Time.deltaTime * currentSpeed;
        riverWater.transform.position = new Vector3(0, 0, raftReference.transform.position.z);*/

        //check if the raft has reach the checkpoint then go to the next one

        if (Mathf.Abs((raftReference.transform.position.z - nextDestination.z)) < 1f)
        {
            checkpointIndex++;

            if (checkpointIndex == checkpoints.Count-1)//if this is the last checkpoint to go to, start slowing down the raft
            {
                slowDown = true;
                nextDestination = checkpoints[checkpointIndex];
            }
            else if (checkpointIndex == checkpoints.Count)//if the raft has reached the last checkpoint, stop
            {
                slowDown = false;
                timeToMove = false;
                playerAttached = false;
                playerReference.transform.parent = null;
            }
            else//otherwise set the new destination tot he next checkpoint
            {
                nextDestination = checkpoints[checkpointIndex];
            }
        }
    }

    //a method called by obstacles when the player hits them which will increment points
    public void ObstacleHit()
    {
        Debug.Log("The player hit me!");
    }

    //python communication function
    void Communication()
    {
        //if (!communicateReference)
        //    return;

        /*To DO
        -This script is the link between the python script and the plate scripts
        -get the desired rotation from the plate script
        -send it to the python script
        -grab the rotation of the real platform
        -set the platform's rotation as such
        -for now just send the desired rotation right into the platform's localRotation
         */
        if (timeToMove)
        {
            //currentScript.transform.localRotation = currentScript.desiredRotation;
            Vector2 giveRotation = new Vector2(raftScript.plannedRotation.x, raftScript.plannedRotation.z);
            //communicateReference.desiredRotation = giveRotation;
            //raftScript.transform.localRotation = Quaternion.Euler(communicateReference.realRotation.x, -45, communicateReference.realRotation.y);
            //raftScript.transform.localRotation = Quaternion.Euler(raftScript.plannedRotation);
        }
        else
        {
            //communicateReference.desiredRotation = new Vector2(0, 0);
        }
    }

    public void TimeToMove()
    {
        //Tells the python server the game has started
        //server.GameStart = true;

        rightHandRay.SetActive(false);
        leftHandRay.SetActive(false);
        timerCanvas.SetActive(false);
    }
}

