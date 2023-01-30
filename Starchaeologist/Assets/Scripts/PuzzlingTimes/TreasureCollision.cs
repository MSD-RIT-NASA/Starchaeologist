using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*DESCRIPTION
 * 
 * This script tells when the player has collected a piece of treasure
 * 
 */

public class TreasureCollision : MonoBehaviour
{
    //void OnTriggerEnter(Collider other)
    //{
    //    //when the player hits the obstacle, tell the game manager to deduct points
    //    if (other.gameObject.CompareTag("PlayerHand"))
    //    {
    //        //Destroy(gameObject);
    //    }
    //}

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHand"))
        {
            Debug.Log("Gathered Treasure");
            Destroy(gameObject);
        }
    }
}
