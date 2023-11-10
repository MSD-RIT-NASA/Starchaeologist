using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*DESCRIPTION
 * 
 * This script is attached to any player damaging object in PuzzlingTimes.
 * 
 * tells PuzzlingGame that the player has been hit and should record score 
 * changes.
 * 
 */
public class Trap_Object : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("PlayerHead"))
        {
            //GameObject.Find("Game Manager").GetComponent<PuzzlingGame>().TrapHit();
            PuzzlingGame.singleton.TrapHit();
        }
    }
}
