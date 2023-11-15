//NASA x RIT authors: Deen Grey and Noah Flanders

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class MineGame : MonoBehaviour
{
    //singleton
    public static MineGame singleton;

    [SerializeField] GameObject playerReference;
    [SerializeField] GameObject rightHandRay;
    [SerializeField] GameObject leftHandRay;
    [SerializeField] GameObject shadowReference;
    [SerializeField] Minecart raftReference;
    [SerializeField] Timer timer;
    [SerializeField] UdpSocket server;
    S_MineCart raftScript;

    public List<GameObject> trackReferences = new List<GameObject>(); //populated with positions while the mine is being built from the S_MineBuilder script
    Vector3 nextDestination = new Vector3(0, 0, 0);
    Vector3 currentDirection = new Vector3(0, 0, 1);
    public float cartAcceleration = 0.1f;
    public float cartSpeed = 1.0f;
    float currentSpeed = 2f;
    public bool timeToMove = true; //turned true the first time the player teleports to the raft from the S_RaftCollision script
    bool slowDown = false;
    bool playerAttached = false;
    int checkpointIndex = 0;

    [SerializeField]
    private GameObject countdown;
    [SerializeField]
    private GameObject readyToStart;
    [SerializeField]
    private TMP_Text countdownText;

    public AudioSource soundfxSource;
    public AudioClip railGrinding_SFX;
    public AudioClip railRiding_SFX;
    public AudioClip explosion1_SFX;
    public AudioClip explosion2_SFX;
    public AudioClip explosion3_SFX;
    public AudioClip bats_SFX;
    public AudioClip treasure_SFX;
    public AudioClip obstacleHit_SFX;

    private Vector3 playerFallPos;
    private Vector3 playerFallVel;
    private Vector3 playerFallAccel;

    //[SerializeField]
    public List<Transform> routes;

    private int routeToGo;

    private float tParam;

    private Vector3 objectPosition;

    private float speedModifier;

    private bool coroutineAllowed;

    private float deadTimeStart;

    [SerializeField]
    private float deadTime;

    public float DeadTime
    {
        get { return deadTime; }
    }

    [SerializeField]
    [Tooltip("Called whenever the minecart begins to move, so that other events can easily hook into it")]
    private UnityEvent onGameStart;

    [SerializeField]
    [Tooltip("Called whenever the minecart stops at the end, so that other events can easily hook into it")]
    private UnityEvent onGameEnd;

    // Start is called before the first frame update
    void Start()
    {
        playerFallPos = playerReference.transform.position;
        playerFallVel = new Vector3(0f, -50f, 0f);
        playerFallAccel = new Vector3(0f, -10f, 0f);

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
        speedModifier = 0.3f;
        coroutineAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        raftReference.IsMoving = timeToMove;
        //start the game by moving the raft
        if (timeToMove)
        {
            readyToStart.SetActive(false);
            if (!soundfxSource.isPlaying)
            {
                soundfxSource.PlayOneShot(railRiding_SFX);
            }
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
                onGameEnd.Invoke();
                //Tell python the game is not running
                //server.GameStart = false;
                //server.GameOver = true;
                readyToStart.SetActive(false);
            }
        }
        else
        {
            if (timer.TimeRemaining > 0)
            {
                countdownText.text = "" + ((int)timer.TimeRemaining + 1);
                playerFallVel += playerFallAccel * timer.GetTime;
                playerFallPos += playerFallVel * timer.GetTime;
                playerReference.transform.position = new Vector3(playerFallPos.x, playerFallPos.y, playerFallPos.z);
            }
            else
            {
                if (deadTimeStart == 0)
                {
                    deadTimeStart = timer.TimePassed;
                }
                leftHandRay.SetActive(true);
                rightHandRay.SetActive(true);
                countdown.SetActive(false);
                if (timeToMove)
                {
                    readyToStart.SetActive(true);
                }
                readyToStart.SetActive(true);
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
            objectPosition.y -= 1;
            raftReference.transform.eulerAngles = new Vector3(curAngles.x, curAngles.y, raftReference.TiltAngle);
            Vector3 upVec = raftReference.transform.up;
            raftReference.transform.LookAt(objectPosition, upVec);
            raftReference.transform.position = objectPosition;
            objectPosition.y += 2;
            playerReference.transform.LookAt(objectPosition);
            playerReference.transform.position = objectPosition;
            this.transform.position = objectPosition;

            if (routeToGo < trackReferences.Count - 1)
            {
                //Put the shadow in front of the player
                objectPosition = Mathf.Pow(1 - tParam, 3) * p02 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p12 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p22 + Mathf.Pow(tParam, 3) * p32;
                shadowReference.transform.LookAt(objectPosition);
                shadowReference.transform.position = objectPosition;
            }
            else
            {
                shadowReference.SetActive(false);
            }
            yield return 0;
        }

        tParam = 0;
        //speedModifier = speedModifier * 0.90f;
        routeToGo += 1;


        //optimization
        if (routeToGo - 2 >= 0)//disable track segments that are far behind the player
        {
            if (trackReferences[routeToGo - 2].gameObject.name != "RepeatedTurns_Track(Clone)" &&
                trackReferences[routeToGo - 2].gameObject.name != "RepeatedTurns_Track_Right" && trackReferences[routeToGo - 2].gameObject.name != "RepeatedTurns_Track_Left" &&
                trackReferences[routeToGo - 2].gameObject.name != "SensoryDeprevation(Clone)")
            {
                Debug.Log(trackReferences[routeToGo - 2].gameObject.name);
                trackReferences[routeToGo - 2].SetActive(false);
            }
        }
        if (routeToGo + 2 < trackReferences.Count)//enable segments that are getting close to the player
        {
            trackReferences[routeToGo + 2].SetActive(true);
        }
        coroutineAllowed = true;

    }

    public void TimeToMove()
    {
        //Tells the python server the game has started
        //server.GameStart = true;

        rightHandRay.SetActive(false);
        leftHandRay.SetActive(false);
        timeToMove = true;
        deadTime = timer.TimePassed - deadTimeStart;

        onGameStart.Invoke();
    }
}