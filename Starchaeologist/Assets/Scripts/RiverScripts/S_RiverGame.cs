using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public class S_RiverGame : MonoBehaviour
{
    public static S_RiverGame singleton;

    GameObject playerReference;
    GameObject raftReference;
    S_Raft raftScript;
    GameObject riverWater;

    public List<GameObject> riverReferences = new List<GameObject>(); //populated with positions while the river is being built from the S_RiverBuilder script
    Vector3 nextDestination = new Vector3(0, 0, 0);
    Vector3 currentDirection = new Vector3(0, 0, 1);
    public float raftAcceleration = 0.1f;
    public float raftSpeed = 3.0f;
    float currentSpeed = 0f;
    public bool timeToMove = false; //turned true the first time the player teleports to the raft from the S_RaftCollision script
    bool slowDown = false;
    bool playerAttached = false;
    int checkpointIndex = 0;

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

    PythonCommunicator communicateReference;




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

        communicateReference = GetComponent<PythonCommunicator>();

        playerReference = GameObject.Find("Player_Rig");
        raftReference = GameObject.Find("Raft_Fake");
        riverWater = GameObject.Find("RiverWater");

        //pythonCommunicator = new HelloRequester();
        raftScript = raftReference.transform.GetChild(1).GetComponent<S_Raft>();
        //pythonCommunicator.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //start the game by moving the raft
        if (timeToMove)
        {
            //stick the player under the raft gameobject to help with movement
            if (!playerAttached)
            {
                raftScript.tilting = true;
                playerAttached = true;
                playerReference.transform.parent = raftReference.transform;
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

        //accelerate or decelerate the raft
        if (currentSpeed != raftSpeed && !slowDown)
        {
            currentSpeed = currentSpeed + (raftAcceleration * Time.deltaTime * raftSpeed * 5f);
            
            raftScript.tiltRange = raftScript.tiltRange + (raftAcceleration * Time.deltaTime * raftScript.maxRange * 5f);
        }
        else if (slowDown)
        {
            currentSpeed = currentSpeed - (raftAcceleration * Time.deltaTime * raftSpeed);
            
            raftScript.tiltRange = raftScript.tiltRange - (raftAcceleration * Time.deltaTime * raftScript.maxRange);
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0.25f, raftSpeed);
        raftScript.tiltRange = Mathf.Clamp(raftScript.tiltRange, 0.25f, raftScript.maxRange);

        //Direct the raft to the next checkpoint
        Vector3 desiredDirection = Vector3.Normalize(nextDestination - raftReference.transform.position);
        if (Mathf.Abs(Vector3.Angle(desiredDirection, currentDirection)) < 1f)
        {
            currentDirection = desiredDirection;
        }
        else
        {
            currentDirection = Vector3.Normalize(currentDirection + (desiredDirection * Time.deltaTime * currentSpeed));
        }

        //move the raft
        raftReference.transform.position += currentDirection * Time.deltaTime * currentSpeed;
        riverWater.transform.position = new Vector3(0, 0, raftReference.transform.position.z);

        //check if the raft has reach the checkpoint then go to the next one
        if (Vector3.Distance(raftReference.transform.position, nextDestination) < 1f)
        {
            checkpointIndex++;

            if (checkpointIndex == riverReferences.Count - 1)//if this is the last checkpoint to go to, start slowing down the raft
            {
                slowDown = true;
                nextDestination = riverReferences[checkpointIndex].transform.GetChild(1).transform.position;
            }
            else if (checkpointIndex == riverReferences.Count)//if the raft has reached the last checkpoint, stop
            {
                slowDown = false;
                timeToMove = false;
                playerAttached = false;
                playerReference.transform.parent = null;
            }
            else//otherwise set the new destination tot he next checkpoint
            {
                nextDestination = riverReferences[checkpointIndex].transform.GetChild(1).transform.position;
            }

            //optimization
            if (checkpointIndex - 5 >= 0)//disable river segments that are far behind the player
            {
                riverReferences[checkpointIndex - 5].SetActive(false);
            }
            if (checkpointIndex + 5 < riverReferences.Count)//enable segments that are getting close to the player
            {
                riverReferences[checkpointIndex + 5].SetActive(true);
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
            communicateReference.desiredRotation = giveRotation;
            raftScript.transform.localRotation = Quaternion.Euler(communicateReference.realRotation.x, 0, communicateReference.realRotation.y);
        }
        else
        {
            communicateReference.desiredRotation = new Vector2(0, 0);
        }
    }
}

