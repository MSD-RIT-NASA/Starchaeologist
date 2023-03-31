using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

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

    private Text txt;
    private Text txtBlip;

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

    void Start()
    {
        txt = GameObject.Find("Score").transform.GetChild(0).gameObject.GetComponent<Text>();
        txtBlip = GameObject.Find("Score").transform.GetChild(1).GetComponent<Text>();
        txtBlip.enabled = false;
    }


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
            int currentScore = int.Parse(txt.text.Split(' ')[1], CultureInfo.InvariantCulture.NumberFormat);
            txt.text = "Score: " + (currentScore + 1);

            audSrc.PlayOneShot(treasureCollect);
            Debug.Log("Gathered Treasure");
            Destroy(gameObject);
            Debug.Log("Deleted");
        }
    }
}
