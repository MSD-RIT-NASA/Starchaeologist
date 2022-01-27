using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RiverGame : MonoBehaviour
{
    GameObject playerReference;
    GameObject raftReference;

    Vector3 goHere = new Vector3(0,0,0);
    public List<Vector3> riverCheckpoints = new List<Vector3>();
    Vector3 currentDirection = new Vector3(0, 0, 1);
    public float raftSpeed = 3.0f;
    bool timeToMove = true;
    int checkpointIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerReference = GameObject.Find("Player_Rig");
        raftReference = GameObject.Find("Raft_Fake");

        playerReference.transform.parent = raftReference.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeToMove)
        {
            MoveRaft();
        }
    }


    void MoveRaft()
    {
        Vector3 desiredDirection = Vector3.Normalize(riverCheckpoints[checkpointIndex] - raftReference.transform.position);
        if (Mathf.Abs(Vector3.Angle(desiredDirection, currentDirection)) < 1f)
        {
            currentDirection = desiredDirection;
        }
        else
        {
            currentDirection = Vector3.Normalize(currentDirection + (desiredDirection * Time.deltaTime * raftSpeed));
        }

        raftReference.transform.position += currentDirection * Time.deltaTime * raftSpeed;
        if(currentDirection.x < 0)
        {
            raftReference.transform.rotation = Quaternion.Euler(0, -Vector3.Angle(Vector3.forward, currentDirection), 0);
        }
        else
        {
            raftReference.transform.rotation = Quaternion.Euler(0, Vector3.Angle(Vector3.forward, currentDirection), 0);
        }
        

        if (Vector3.Distance(raftReference.transform.position, riverCheckpoints[checkpointIndex]) < 1f)
        {
            //riverCheckpoints.RemoveAt(0);
            checkpointIndex++;
        }
    }
}
