using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateScript : MonoBehaviour
{

    /*
     TO DO:
        -add a trigger to check player collision
        -keep record of what trap will get used for this plate
        -include use one of the wobble scripts once the player steps on this
        -Tell the Game Manager the rotation for the platform, then get the python's rotation
        -give the game manager this platform's indexes so it know which one to listen to 
     */

    //public int xIndex;
    //public int zIndex;

    public Vector2 myPosition;

    public bool trapped = false;

    public List<Vector2> adjacentPlates;

    bool wobbling = false;
    float wobbleTimer = 0f;

    bool trapping = false;
    float trapTimer = 0f;

    bool reactivate = false;

    PuzzlingGame managerReference;

    // Start is called before the first frame update
    void Start()
    {
        managerReference = GameObject.Find("GameManager").GetComponent<PuzzlingGame>();
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
                Debug.Log("I'm wobbling");

                wobbling = false;
                trapping = true;
            }
            else if (trapping)//then the trap will go off
            {
                /* TO DO
                 figure out the trapping
                 */
                Debug.Log("I'm trapping");
                trapTimer = trapTimer + Time.deltaTime;

                if(trapTimer > 3f)
                {
                    reactivate = true;
                    trapping = false;
                }
            }
        }
        if(reactivate)
        {
            reactivate = false;
            managerReference.ActivatePlates(adjacentPlates);
        }
    }

    //trigger detection
    void OnTriggerEnter(Collider other)
    {
        //check of the collider is the player
        if (other.gameObject.CompareTag("Player"))
        {
            //deactivate the platforms that are currently active
            managerReference.DeactivatePlatforms(myPosition);

            //if the platform is trapped starting wobbling, if not go straight to activating the adjacent tiles
            if (trapped)
            {
                wobbling = true;
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
