using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*DESCRIPTION
 * 
 * This script tells when the player has been hit by a river obstacle
 * 
 */


public class S_ObstacleCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("PlayerHead"))
        {
            //GameObject.Find("Game Manager").GetComponent<S_RiverGame>().ObstacleHit();
            scoreScript.singleton.hitScore();

            Destroy(this);
        }
    }
}
