using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class S_ObstacleCollider : MonoBehaviour
{
    [SerializeField] Text scoreText;

    void OnTriggerEnter(Collider other)
    {
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("Obstacle"))
        {

            
            GameObject.Find("Game Manager").GetComponent<S_RiverGame>().ObstacleHit();
            scoreScript.Instance.hitScore();
            Debug.Log(scoreScript.Score);
            Debug.Log("score updated");
            scoreText.text = "Score: " + scoreScript.Score;
        }
    }
}
