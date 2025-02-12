using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Unity.XR.CoreUtils;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class S_RiverGame : S_RiverBuilder
{



    //singleton
    public static S_RiverGame singleton;
    [SerializeField] GameObject playerReference;
    [SerializeField] GameObject raftReference;
    private S_Raft raftScript;

    [SerializeField] Camera vrCamera;
    [SerializeField] GameObject raftObject;
    [SerializeField] GameObject rightHandRay;
    [SerializeField] GameObject leftHandRay;
    [SerializeField] GameObject countdownCanvas;
    [SerializeField] GameObject readyCanvas;
    [SerializeField] GameObject raftEndPoint;

    Vector3 nextDestination;
    Vector3 desiredDirection = new Vector3(0, 0, 1);
    private float raftAcceleration = 0.1f;
    private float raftSpeed = 4.0f;
    private float turningSpeed = 0.15f;
    private float currentSpeed = 0f;
    public bool timeToMove = false; //turned true the first time the player teleports to the raft from the S_RaftCollision script
    bool slowDown = false;
    bool playerAttached = false;
    int checkpointIndex = 0;
    private Quaternion previousRotation = Quaternion.identity;
    private float timeOfLastCheckpoint;

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

    [SerializeField]
    [Tooltip("Called whenever the game begins, so that other events can easily hook into it")]
    private UnityEvent onGameStart;

    [SerializeField]
    [Tooltip("Called whenever the game ends, so that other events can easily hook into it")]
    private UnityEvent onGameEnd;



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

        GameObject startReference = GameObject.Find("RiverPlayerSpawn");
        for (int i = 0; i < startReference.transform.childCount; i++)
        {
            TeleportationArea test = startReference.transform.GetChild(i).GetComponent<TeleportationArea>();
            if (startReference.transform.GetChild(i).GetComponent<TeleportationArea>())
            {
                Destroy(startReference.transform.GetChild(i).GetComponent<TeleportationArea>());
                //startReference.transform.GetChild(i).GetComponent<TeleportationArea>().enabled = false;
            }
        }

        previousRotation = raftReference.transform.rotation;
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
            Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection);
            float progress = (Time.time - timeOfLastCheckpoint) * (currentSpeed * turningSpeed);
            progress = Mathf.Clamp01(progress);
            raftReference.transform.rotation = Quaternion.Slerp(previousRotation, desiredRotation, EasingFunctions.EaseInOutQuad(0, 1, progress));
            raftReference.transform.position += raftReference.transform.forward * currentSpeed * Time.deltaTime;
        }

        if (Mathf.Abs((raftReference.transform.position.z - nextDestination.z)) < 1f)
        {
            checkpointIndex++;
            timeOfLastCheckpoint = Time.time;

            if (checkpointIndex == checkpoints.Count-1)//if this is the last checkpoint to go to, start slowing down the raft
            {
                slowDown = true;
                nextDestination = checkpoints[checkpointIndex];
                previousRotation = raftReference.transform.rotation;
            }
            else if (checkpointIndex == checkpoints.Count)//if the raft has reached the last checkpoint, stop
            {
                StopMove();
            }
            else//otherwise set the new destination to the next checkpoint
            {
                nextDestination = checkpoints[checkpointIndex];
                previousRotation = raftReference.transform.rotation;
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
        timeToMove = true;
        server.GameStart = true;
        rightHandRay.SetActive(false);
        leftHandRay.SetActive(false);
        timerCanvas.SetActive(false);
        //GetComponent<UdpSocket>().GameStart = true;
        //Debug.Log("START BUTTON PRESSED");

        onGameStart.Invoke();
    }

    public void StopMove()
    {
        // End the level
        slowDown = false;
        timeToMove = false;
        playerAttached = false;
        playerReference.transform.parent = null;

        // Show the canvas at the end of the level
        FindObjectOfType<ScoreData>().SetScoreCanvasActive(true);

        onGameEnd.Invoke();
    }
}

