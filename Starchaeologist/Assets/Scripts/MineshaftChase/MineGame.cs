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
    float currentSpeed = 2f;
    public bool timeToMove = true; //turned true the first time the player teleports to the raft from the S_RaftCollision script
    bool slowDown = false;
    bool playerAttached = false;
    int checkpointIndex = 0;


    //[SerializeField]
    public List<Transform> routes;

    private int routeToGo;

    private float tParam;

    private Vector3 objectPosition;

    private float speedModifier;

    private bool coroutineAllowed;


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

        routeToGo = 0;
        tParam = 0f;
        speedModifier = 0.5f;
        coroutineAllowed = true;
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
                //raftScript.tilting = true;
                playerAttached = true;
                playerReference.transform.parent = raftReference.transform;
               // playerReference.transform.position = raftReference.transform.GetChild(0).position;
            }

            if (coroutineAllowed)
            {
                StartCoroutine(GoByTheRoute(routeToGo));
            }

            //MoveRaft();
            //make the raft rotate
            //raftScript.Raft();
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

        //Direct the raft to the next checkpoint
        Vector3 desiredDirection = Vector3.Normalize(nextDestination - raftReference.transform.position);
        if (Mathf.Abs(Vector3.Angle(desiredDirection, currentDirection)) < 1f)
        {
            currentDirection = desiredDirection;
        }
        else
        {
            currentDirection = Vector3.Normalize(currentDirection + (desiredDirection * Time.deltaTime * currentSpeed));
            //raftReference.transform.rotation = Quaternion.LookRotation(currentDirection);
        }

        ////move the raft
        raftReference.transform.position += currentDirection * Time.deltaTime * currentSpeed;

        //check if the raft has reach the checkpoint then go to the next one
        if (Vector3.Distance(raftReference.transform.position, nextDestination) < 5f)
        {
            //Debug.Log("check for new checkpoing");
            checkpointIndex++;

            if (checkpointIndex == trackReferences.Count - 1)//if this is the last checkpoint to go to, start slowing down the raft
            {
                slowDown = true;
                nextDestination = trackReferences[checkpointIndex].transform.GetChild(1).transform.position;
            }
            else if (checkpointIndex == trackReferences.Count)//if the raft has reached the last checkpoint, stop
            {
                slowDown = false;
                timeToMove = false;
                playerAttached = false;
                playerReference.transform.parent = null;
            }
            else//otherwise set the new destination tot he next checkpoint
            {
                Debug.Log("New checkpoint");
                nextDestination = trackReferences[checkpointIndex].transform.GetChild(1).transform.position;
            }

            //optimization
            if (checkpointIndex - 2 >= 0)//disable river segments that are far behind the player
            {
                trackReferences[checkpointIndex - 2].SetActive(false);
            }
            if (checkpointIndex + 2 < trackReferences.Count)//enable segments that are getting close to the player
            {
                trackReferences[checkpointIndex + 2].SetActive(true);
            }
        }

    }
    private IEnumerator GoByTheRoute(int routeNum)
    {
        coroutineAllowed = false;

        Vector3 p0 = routes[routeNum].GetChild(0).position;
        Vector3 p1 = routes[routeNum].GetChild(1).position;
        Vector3 p2 = routes[routeNum].GetChild(2).position;
        Vector3 p3 = routes[routeNum].GetChild(3).position;
        Debug.Log(routes[routeNum]);
        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;

            raftReference.transform.LookAt(objectPosition);
            raftReference.transform.position = objectPosition;

            yield return new WaitForEndOfFrame();
        }

        tParam = 0;
        speedModifier = speedModifier * 0.90f;
        routeToGo += 1;

        if (routeToGo > routes.Count - 1)
        {
            routeToGo = 0;
        }
        //optimization
        if (routeToGo - 2 >= 0)//disable river segments that are far behind the player
        {
            trackReferences[routeToGo - 2].SetActive(false);
        }
        if (routeToGo + 2 < trackReferences.Count)//enable segments that are getting close to the player
        {
            trackReferences[routeToGo + 2].SetActive(true);
        }
        coroutineAllowed = true;

    }
}
