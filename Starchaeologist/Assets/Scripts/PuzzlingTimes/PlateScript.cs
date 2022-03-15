using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateScript : MonoBehaviour
{
    public Vector2 myPosition;
    public List<Vector2> adjacentPlates;
    public bool reactivate = false;
    public bool triggered = false;
    PuzzlingGame managerReference;

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


    // Start is called before the first frame update
    public void DataSetup(Vector2 getPosition)
    {
        myPosition = getPosition;
        managerReference = GameObject.Find("GameManager").GetComponent<PuzzlingGame>();
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
    }

    // Update is called once per frame
    void Update()
    {
        if(trapped)//make sure the platform is trapped
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
                    managerReference.TrapTime();
                    Debug.Log("Back to zero");
                }
                wobbleTimer = wobbleTimer + Time.deltaTime;
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
        }
    }
}
