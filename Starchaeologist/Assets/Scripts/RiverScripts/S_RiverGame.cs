using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class S_RiverGame : MonoBehaviour
{
    GameObject playerReference;
    GameObject raftReference;

    public List<Vector3> riverCheckpoints = new List<Vector3>(); //populated with positions while the river is being built from the S_RiverBuilder script
    Vector3 currentDirection = new Vector3(0, 0, 1);
    public float raftAcceleration = 0.1f;
    public float raftSpeed = 3.0f;
    float currentSpeed = 0f; 
    public bool timeToMove = false; //turned true the first time the player teleports to the raft from the S_RaftCollision script
    bool slowDown = false;
    bool playerAttached = false;
    int checkpointIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerReference = GameObject.Find("Player_Rig");
        raftReference = GameObject.Find("Raft_Fake");
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
    }


    void MoveRaft()
    {
        //accelerate or decelerate the raft
        if(currentSpeed != raftSpeed && !slowDown)
        {
            currentSpeed = currentSpeed + (raftAcceleration * Time.deltaTime * raftSpeed * 5f);
        }
        else if(slowDown)
        {
            currentSpeed = currentSpeed - (raftAcceleration * Time.deltaTime * raftSpeed);
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0.25f, raftSpeed);

        //turn the raft on the direction of the next checkpoint
        Vector3 desiredDirection = Vector3.Normalize(riverCheckpoints[checkpointIndex] - raftReference.transform.position);
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
        if (Vector3.Distance(raftReference.transform.position, riverCheckpoints[checkpointIndex]) < 1f)
        {
            //riverCheckpoints.RemoveAt(0);
            checkpointIndex++;
            if (checkpointIndex == riverCheckpoints.Count - 1)
            {
                slowDown = true;
            }
            if (checkpointIndex == riverCheckpoints.Count)
            {
                slowDown = false;
                timeToMove = false;
                playerAttached = false;
                playerReference.transform.parent = null;
            }
        }
    }
}
