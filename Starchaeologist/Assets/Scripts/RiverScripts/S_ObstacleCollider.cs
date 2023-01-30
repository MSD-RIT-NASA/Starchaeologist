using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ObstacleCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("PlayerHead"))
        {

            
            GameObject.Find("Game Manager").GetComponent<S_RiverGame>().ObstacleHit();
            scoreScript.singleton.hitScore();
            Debug.Log(scoreScript.Score);
            Debug.Log("score updated");

            Destroy(this);
        }
    }
}
