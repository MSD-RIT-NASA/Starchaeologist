using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Object : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("PlayerHead"))
        {
            GameObject.Find("Game Manager").GetComponent<PuzzlingGame>().TrapHit();

            Destroy(this);
        }
    }
}
