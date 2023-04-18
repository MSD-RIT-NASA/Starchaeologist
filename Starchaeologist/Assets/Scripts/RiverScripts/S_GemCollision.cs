using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_GemCollision : MonoBehaviour
{
    [SerializeField] Text scoreText;

    void OnTriggerEnter(Collider other)
    {
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("Treasure"))
        {
            S_RiverGame riverScript = GameObject.Find("Game Manager").GetComponent<S_RiverGame>();
            riverScript.TreaureHit();
            scoreText.text = "Score: " + riverScript.score;
            Destroy(other.gameObject);
        }
    }
}
