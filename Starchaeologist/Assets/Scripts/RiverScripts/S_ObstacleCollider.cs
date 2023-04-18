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


            S_RiverGame riverScript = GameObject.Find("Game Manager").GetComponent<S_RiverGame>();
            riverScript.ObstacleHit();
            Debug.Log("score updated");
            scoreText.text = "Score: " + riverScript.score;
            Destroy(other.transform.parent.gameObject);
        }
    }
}
