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
    [SerializeField]
    private AudioSource audSrc;
    [SerializeField]
    private AudioClip treasureCollect;

    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("collides with " + other.gameObject.tag);
    //    //when the player hits the obstacle, tell the game manager to deduct points
    //    if (other.gameObject.CompareTag("PlayerHand"))
    //    {
    //        Destroy(gameObject);
    //        Debug.Log("Touched Treasure");
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("PlayerHand"))
    //    {
    //        Debug.Log("Gathered Treasure");
    //        Destroy(gameObject);
    //        Debug.Log("Deleted");
    //    }
    //}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerHand"))
        {
            Debug.Log("Toucing Treasure");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerHand"))
        {
            audSrc.PlayOneShot(treasureCollect);
            Debug.Log("Gathered Treasure");
            Destroy(gameObject);
            Debug.Log("Deleted");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerHand"))
        {
            Debug.Log("Toucing Treasure");
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerHand"))
        {
            audSrc.PlayOneShot(treasureCollect);
            Debug.Log("Gathered Treasure");
            Destroy(gameObject);
            Debug.Log("Deleted");
        }
    }
}
