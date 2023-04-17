using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*DESCRIPTION
 * 
 *  This script pertains to all moving platforms within Puzzling Times. This will
 *  be attached to each pressure plate (see prefab) and controls the movement of
 *  the individual plates. 
 *  
 *  The script is called when the player teleports (collides) with the platform.
 *  This activates depending on its trap status (set in PuzzlingBuilder). If the
 *  platform is 'trapped', it will choose one of two rotation scripts to call and 
 *  tell PuzzlingGame to set off a trap relative to its position. if not trapped 
 *  then the game simply continues
 *  
 *  This script keeps track of the indeces of adjacent tiles and applicable traps.
 *  When a plate is teleported onto it, it tells PuzzlingGame to deactivate all
 *  active tiles to the player must endure the trap. Once Reactivate() is called
 *  all recorded adjacent plates will be active for teleportation.
 *  
 *  This script is also attached to the start and end platforms simply for teleportation
 *  locking reasons. Never make them 'trapped'.
 * 
 */

public class PlateScript : MonoBehaviour
{
    public Vector2 myPosition;
    public List<Vector2> adjacentPlates;
    //public bool reactivate = false;
    public bool triggered = false;
    //PuzzlingGame managerReference;
    public bool ready = false;

    //trap variables
    public bool trapped = false;

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

    //the manager scrpt will use this to determine which trap to set off
    public List<int> trapList;

    public Transform leftHand;
    public Transform rightHand;
    void Start()
    {
        //Transform hands = Instantiate(leftHand) as Transform;
        Transform hands = GameObject.Find("XR Rig 2").transform.GetChild(0).transform.Find("LeftHand Controller").transform.Find("Left_Hand").transform;
        Physics.IgnoreCollision(hands.GetComponent<Collider>(), GetComponent<Collider>());
       // hands = Instantiate(rightHand) as Transform;
        hands = GameObject.Find("XR Rig 2").transform.GetChild(0).transform.Find("RightHand Controller").transform.Find("Right_Hand").transform;
        Physics.IgnoreCollision(hands.GetComponent<Collider>(), GetComponent<Collider>());
    }

    // Start is called before the first frame update
    public void DataSetup(Vector2 getPosition)
    {
        myPosition = getPosition;
        //managerReference = GameObject.Find("Game Manager").GetComponent<PuzzlingGame>();
        twoWayScript = GetComponent<S_2_Wobble>();
        fourWayScript = GetComponent<S_4_Wobble>();
        desiredRotation = Quaternion.Euler(0, 0, 0);
        twoWayScript = GetComponent<S_2_Wobble>();

        //set up the trap list
        //0 = ceiling spikes
        //1 = arrows
        //2 = log swing          
        //3 = pillar swipe
        trapList = new List<int>();
        trapList.Add(0);//every platform can be ceiling spiked           
        
        if ((int)myPosition.y % 3 != 1)//pillars obstruct arrows
        {
            trapList.Add(1);
        }
        
        if((int)myPosition.x == 2 || (int)myPosition.x == 3)//swings and pillars cannot affect the same plate
        {
            trapList.Add(2);
        }
        else
        {
            trapList.Add(3);
        }

        ready = true;

        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (wobbling)//when the player enters the tile it will first wobble/lower
        {
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
                wobbleTimer = 0f;
                wobbling = false;
                PuzzlingGame.singleton.TrapTime();
                //managerReference.TrapTime();
                Debug.Log("Back to zero");
            }
            wobbleTimer = wobbleTimer + Time.deltaTime;
        }
    }

    //trigger detection
    void OnTriggerEnter(Collider other)
    {
        //check of the collider is the player
        if (other.gameObject.CompareTag("PlayerFoot") && !triggered && ready)
        {
            triggered = true;

            //deactivate the platforms that are currently active
            //managerReference.DeactivatePlatforms(myPosition);
            PuzzlingGame.singleton.DeactivatePlatforms(myPosition);

            //if the platform is trapped starting wobbling, if not go straight to activating the adjacent tiles
            if (trapped)
            {
                enabled = true;
                //randomly  choose a wobble patern
                wobbling = true;
                wobbleType = Random.Range(0, 3);
                PuzzlingGame.singleton.vignetteOn();
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
                Reactivate();
            }
        }
    }

    public void Reactivate()
    {
        Debug.Log("you can move now");
        //managerReference.ActivatePlates(adjacentPlates);
        PuzzlingGame.singleton.ActivatePlates(adjacentPlates);
        triggered = false;
        enabled = false;
    }
}
