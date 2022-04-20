using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("PlayerBody"))
        {
            Debug.Log("Gathered Treasure");
            Destroy(gameObject);
        }
    }
}
