using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateScript : MonoBehaviour
{

    /*
     TO DO:
        -keep record of what trap will get used for this plate (game manager's job)
        -include use one of the wobble scripts once the player steps on this
        -Tell the Game Manager the rotation for the platform, then get the python's rotation
        -give the game manager this platform's indexes so it know which one to listen to 
     */

    //public int xIndex;
    //public int zIndex;

    public Vector2 myPosition;
    public List<Vector2> adjacentPlates;
    public bool reactivate = false;
    bool triggered = false;
    PuzzlingGame managerReference;

    //trap variables
    public bool trapped = false;
    bool trapping = false;
    float trapTimer = 0f;

    /*wobble variables*/
    //general
    bool wobbling = false;
    float wobbleTimer = 0f;
    int wobbleType = 0;
    public float tiltRange = 10f;
    public float tiltSpeed = 1.0f;

    S_2_Wobble twoWayScript;
    S_4_Wobble fourWayScript;
    public Quaternion desiredRotation;


    // Start is called before the first frame update
    void Start()
    {
        managerReference = GameObject.Find("GameManager").GetComponent<PuzzlingGame>();
        twoWayScript = GetComponent<S_2_Wobble>();
        fourWayScript = GetComponent<S_4_Wobble>();
        desiredRotation = Quaternion.Euler(0, 0, 0);
        twoWayScript = GetComponent<S_2_Wobble>();
    }

    // Update is called once per frame
    void Update()
    {
        if(trapped)//make sure the platform is trapped
        {
            if (wobbling)//when the player enters the tile it will first wobble/lower
            {
                /* TO DO
                 figure out the wobbling
                 */
                //Debug.Log("I'm wobbling");
                if(wobbleTimer > 5f)
                {
                    if(wobbleType == 0)
                    {
                        twoWayScript.back2Zero = true;
                    }
                    else
                    {
                        fourWayScript.back2Zero = true;
                    }
                    wobbling = false;
                    trapping = true;
                    managerReference.activateTrap = true;
                    Debug.Log("Back to zero");
                }
                wobbleTimer = wobbleTimer + Time.deltaTime;
            }
            else if (trapping && !managerReference.activateTrap)//then the trap will go off
            {
                /* TO DO
                 figure out the trapping
                 */
                Debug.Log("Trap done");
                trapping = false;
                //trapTimer = trapTimer + Time.deltaTime;
                //
                //if(trapTimer > 3f)
                //{
                //    //reactivate = true;
                //    trapping = false;
                //}
            }
        }
        if(reactivate)
        {
            Debug.Log("you can move now");
            reactivate = false;
            managerReference.ActivatePlates(adjacentPlates);
            triggered = false;
        }
    }

    //trigger detection
    void OnTriggerEnter(Collider other)
    {
        //check of the collider is the player
        if (other.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;

            //deactivate the platforms that are currently active
            managerReference.DeactivatePlatforms(myPosition);

            //if the platform is trapped starting wobbling, if not go straight to activating the adjacent tiles
            if (trapped)
            {
                //randomly  choose a wobble patern
                wobbling = true;
                wobbleType = Random.Range(0, 3);

                if(wobbleType == 0)//0 = 2-way wobble
                {
                    twoWayScript.enabled = true;
                    twoWayScript.DataSetup();//get a new rotation every time
                }
                else//1 = 4-way circular, 2 = 4-way figure-8
                {
                    fourWayScript.enabled = true;
                    fourWayScript.DataSetup(wobbleType);//get a new rotation every time
                }
            }
            else
            {
                reactivate = true;
            }
            /*
                -disable teleportation for the duration of the trap
                -Wobble the platform
                -set off the trap
                -re-enable teleportation
             */
        }
    }
}
