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

    public int xIndex;
    public int zIndex;

    public bool trapped = false;
    bool wobbling = false;
    bool trapping = false;

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


                wobbling = false;
                trapping = true;
            }
            else if (trapping)//then the trap will go off
            {


                trapping = false;
            }
        }
    }

    //trigger detection
    void OnTriggerEnter(Collider other)
    {
        //check of the collider is the player
        if (other.gameObject.CompareTag("Player"))
        {
            wobbling = true;
            /*
                -disable teleportation for the duration of the trap
                -Wobble the platform
                -set off the trap
                -re-enable teleportation
             */
        }
    }
}
