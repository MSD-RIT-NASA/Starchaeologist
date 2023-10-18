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
        if (other.gameObject.CompareTag("Tresure"))
        {


            GameObject.Find("Game Manager").GetComponent<S_RiverGame>().ObstacleHit();
            scoreScript.Instance.treasureScore();
            Debug.Log(scoreScript.Score);
            Debug.Log("score updated");
            scoreText.text = "Score: " + scoreScript.Score;

            Destroy(other.gameObject);
        }
    }
}
