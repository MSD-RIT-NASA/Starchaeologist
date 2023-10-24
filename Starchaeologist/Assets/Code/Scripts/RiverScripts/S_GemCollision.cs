using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_GemCollision : MonoBehaviour
{
    [SerializeField] Text scoreText;

    void OnTriggerEnter(Collider other)
    {
        print("Trigger");
        //when the player hits the obstacle, tell the game manager to deduct points
        if (other.gameObject.CompareTag("Treasure"))
        {

            GameObject manager = GameObject.Find("Game Manager");
            if(manager)
            {
                S_RiverGame riverManager = manager.GetComponent<S_RiverGame>();
                if(riverManager != null)
                {
                    manager.GetComponent<S_RiverGame>().ObstacleHit();
                }
            }
            scoreScript scoreScript = scoreScript.Instance;
            if(scoreScript)
            {
                scoreScript.treasureScore();

                Debug.Log(scoreScript.Score);
                Debug.Log("score updated");

                scoreText.text = "Score: " + scoreScript.Score;

            }
            else
            {
                Debug.LogWarning("Score script instance was not found");
            }

            other.gameObject.GetComponent<TreasureCollision>().ActivateFX();
            Destroy(other.gameObject.transform.parent.gameObject);
        }
    }
}
