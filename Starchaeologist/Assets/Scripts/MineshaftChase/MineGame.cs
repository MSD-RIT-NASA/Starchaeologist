using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineGame : MonoBehaviour
{
    //singleton
    public static MineGame singleton;

    [SerializeField] GameObject playerReference;
    [SerializeField] GameObject shadowReference;
    [SerializeField] Minecart raftReference;
    S_MineCart raftScript;

    public List<GameObject> trackReferences = new List<GameObject>(); //populated with positions while the mine is being built from the S_MineBuilder script
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
        speedModifier = 0.4f;
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
                playerAttached = true;
            }
            //go along the route on the track 
            if (coroutineAllowed && routeToGo < trackReferences.Count)
            {
                StartCoroutine(GoByTheRoute(routeToGo));
            }
            //if it's the end of the track then stop moving 
            else if (routeToGo == trackReferences.Count)
            {
                timeToMove = false;
            }
        }
    }
    private IEnumerator GoByTheRoute(int routeNum)
    {
        coroutineAllowed = false;

        //get the control points on the current track
        Vector3 p0 = routes[routeNum].GetChild(0).position;
        Vector3 p1 = routes[routeNum].GetChild(1).position;
        Vector3 p2 = routes[routeNum].GetChild(2).position;
        Vector3 p3 = routes[routeNum].GetChild(3).position;

        //for the shadow
        Vector3 p02 = routes[routeNum].GetChild(0).position;
        Vector3 p12 = routes[routeNum].GetChild(1).position;
        Vector3 p22 = routes[routeNum].GetChild(2).position;
        Vector3 p32 = routes[routeNum].GetChild(3).position;

        if (routeToGo < trackReferences.Count - 1)
        {
            //make sure the shadow has a place to be put
            p02 = routes[routeNum + 1].GetChild(0).position;
            p12 = routes[routeNum + 1].GetChild(1).position;
            p22 = routes[routeNum + 1].GetChild(2).position;
            p32 = routes[routeNum + 1].GetChild(3).position;
        }
        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;
            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;


            Vector3 curAngles = raftReference.transform.eulerAngles;
            objectPosition.y += 1;
            raftReference.transform.eulerAngles = new Vector3(curAngles.x, curAngles.y, raftReference.TiltAngle);
            Vector3 upVec = raftReference.transform.up;
            raftReference.transform.LookAt(objectPosition, upVec);
            raftReference.transform.position = objectPosition;
            objectPosition.y += 1;
            playerReference.transform.LookAt(objectPosition);
            playerReference.transform.position = objectPosition;

            if (routeToGo < trackReferences.Count - 1)
            {
                //Put the shadow in front of the player
                objectPosition = Mathf.Pow(1 - tParam, 3) * p02 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p12 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p22 + Mathf.Pow(tParam, 3) * p32;
                shadowReference.transform.LookAt(objectPosition);
                shadowReference.transform.position = objectPosition;
            }
            yield return 0;
        }

        tParam = 0;
        speedModifier = speedModifier * 0.90f;
        routeToGo += 1;


        //optimization
        if (routeToGo - 2 >= 0)//disable track segments that are far behind the player
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