using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineGame : MonoBehaviour
{
    //singleton
    public static MineGame singleton;

    [SerializeField] GameObject playerReference;
    [SerializeField] GameObject raftReference;
    S_MineCart raftScript;

    public List<GameObject> trackReferences = new List<GameObject>(); //populated with positions while the river is being built from the S_RiverBuilder script
    Vector3 nextDestination = new Vector3(0, 0, 0);
    Vector3 currentDirection = new Vector3(0, 0, 1);
    public float cartAcceleration = 0.1f;
    public float cartSpeed = 3.0f;
    float currentSpeed = 1f;
    public bool timeToMove = true; //turned true the first time the player teleports to the raft from the S_RaftCollision script
    bool slowDown = false;
    bool playerAttached = false;
    int checkpointIndex = 0;

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

        raftScript = raftReference.transform.GetChild(1).GetComponent<S_MineCart>();
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
               // playerReference.transform.position = raftReference.transform.GetChild(0).position;
            }
            MoveRaft();
            //make the raft rotate
            raftScript.Raft();
        }
    }
    void MoveRaft()
    {
        ////accelerate or decelerate the raft
        //if (currentSpeed != cartSpeed && !slowDown)
        //{
        //    currentSpeed = currentSpeed + (cartAcceleration * Time.deltaTime * cartSpeed * 5f);

        //    raftScript.tiltRange = raftScript.tiltRange + (cartAcceleration * Time.deltaTime * raftScript.maxRange * 5f);
        //}
        //else if (slowDown)
        //{
        //    currentSpeed = currentSpeed - (cartAcceleration * Time.deltaTime * cartSpeed);

        //    raftScript.tiltRange = raftScript.tiltRange - (cartAcceleration * Time.deltaTime * raftScript.maxRange);
        //}
        //currentSpeed = Mathf.Clamp(currentSpeed, 0.25f, cartSpeed);
        //raftScript.tiltRange = Mathf.Clamp(raftScript.tiltRange, 0.25f, raftScript.maxRange);

        ////Direct the raft to the next checkpoint
        //Vector3 desiredDirection = Vector3.Normalize(nextDestination - raftReference.transform.position);
        //if (Mathf.Abs(Vector3.Angle(desiredDirection, currentDirection)) < 1f)
        //{
        //    currentDirection = desiredDirection;
        //}
        //else
        //{
        //    currentDirection = Vector3.Normalize(currentDirection + (desiredDirection * Time.deltaTime * currentSpeed));
        //}

        ////move the raft
        raftReference.transform.position += currentDirection * Time.deltaTime * currentSpeed;

        ////check if the raft has reach the checkpoint then go to the next one
        //if (Vector3.Distance(raftReference.transform.position, nextDestination) < 1f)
        //{
        //    checkpointIndex++;

        //    if (checkpointIndex == trackReferences.Count - 1)//if this is the last checkpoint to go to, start slowing down the raft
        //    {
        //        slowDown = true;
        //        nextDestination = trackReferences[checkpointIndex].transform.GetChild(1).transform.position;
        //    }
        //    else if (checkpointIndex == trackReferences.Count)//if the raft has reached the last checkpoint, stop
        //    {
        //        slowDown = false;
        //        timeToMove = false;
        //        playerAttached = false;
        //        playerReference.transform.parent = null;
        //    }
        //    else//otherwise set the new destination tot he next checkpoint
        //    {
        //        nextDestination = trackReferences[checkpointIndex].transform.GetChild(1).transform.position;
        //    }

        //    //optimization
        //    if (checkpointIndex - 5 >= 0)//disable river segments that are far behind the player
        //    {
        //        trackReferences[checkpointIndex - 5].SetActive(false);
        //    }
        //    if (checkpointIndex + 5 < trackReferences.Count)//enable segments that are getting close to the player
        //    {
        //        trackReferences[checkpointIndex + 5].SetActive(true);
        //    }
        //}
    }
   
}
