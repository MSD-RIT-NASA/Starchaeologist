using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public class S_RiverGame : MonoBehaviour
{
    GameObject playerReference;
    GameObject raftReference;
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
    private HelloRequester pythonCommunicator;
    public float rotationX = 0f;
    public float rotationZ = 0f;
    public bool rotationChanged = false;
    int pythonBuffer = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerReference = GameObject.Find("Player_Rig");
        raftReference = GameObject.Find("Raft_Fake");
        riverWater = GameObject.Find("RiverWater");

        pythonCommunicator = new HelloRequester();
        //pythonCommunicator.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //start the game by moving the raft
        if(timeToMove)
        {
            //stick the player under the raft gameobject to help with movement
            if(!playerAttached)
            {
                playerAttached = true;
                playerReference.transform.parent = raftReference.transform;
            }
            MoveRaft();
        }

        //python communication
        if(pythonBuffer < 10)
        {
            string getMessage = pythonCommunicator.pythonMessage;
            if(!pythonCommunicator.Running)
            {
                float giveRotationX = Random.Range(-10f, 10f);
                float giveRotationZ = Random.Range(-10f, 10f);
                string giveMessage = giveRotationX + " " + giveRotationZ;
                pythonCommunicator.pythonMessage = giveMessage;
                pythonCommunicator.StartThread();
            }
            else if(pythonCommunicator.receiveMessage && getMessage != null)
            {
                pythonBuffer++;

                string[] getValues = getMessage.Split(' ');

                float xRotation = float.Parse(getValues[0], CultureInfo.InvariantCulture.NumberFormat);
                float zRotation = float.Parse(getValues[1], CultureInfo.InvariantCulture.NumberFormat);

                Debug.Log("Received: " + "xRotation(" + xRotation + "), zRotation(" + zRotation +")");
                pythonCommunicator.FinishedRunning();
            }
        }
    }


    void MoveRaft()
    {
        //accelerate or decelerate the raft
        if(currentSpeed != raftSpeed && !slowDown)
        {
            currentSpeed = currentSpeed + (raftAcceleration * Time.deltaTime * raftSpeed * 5f);
            /* 
            TO DO: Make this also affect the wobbling of the raft so it increases as the speed increases
            */
        }
        else if(slowDown)
        {
            currentSpeed = currentSpeed - (raftAcceleration * Time.deltaTime * raftSpeed);
            /* 
            TO DO: Make this also affect the wobbling of the raft so it decreases as the speed decreases
            */
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0.25f, raftSpeed);

        //turn the raft in the direction of the next checkpoint
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
        riverWater.transform.position = new Vector3(0,0, raftReference.transform.position.z);

        //rotate the raft
        //if(currentDirection.x < 0)
        //{
        //    raftReference.transform.rotation = Quaternion.Euler(0, -Vector3.Angle(Vector3.forward, currentDirection), 0);
        //}
        //else
        //{
        //    raftReference.transform.rotation = Quaternion.Euler(0, Vector3.Angle(Vector3.forward, currentDirection), 0);
        //}

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
            if(checkpointIndex - 5 >= 0)//disable river segments that are far behind the player
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


    private void OnDestroy()
    {
        pythonCommunicator.Stop();
    }
}
