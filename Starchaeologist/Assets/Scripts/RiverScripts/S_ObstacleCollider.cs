using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_ObstacleCollider : MonoBehaviour
{
    private Text scoreText;

    void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponentInChildren<Text>();
        scoreText.text = "Score: " + 0;
    }
    void OnTriggerEnter(Collider other)
    {
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("PlayerHead"))
        {

            
            GameObject.Find("Game Manager").GetComponent<S_RiverGame>().ObstacleHit();
            scoreScript.Instance.hitScore();
            Debug.Log(scoreScript.Score);
            Debug.Log("score updated");
            scoreText.text = "Score: " + scoreScript.Score;
            

            Destroy(this);
        }
    }
}
