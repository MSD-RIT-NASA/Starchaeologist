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
    [Space]
    [SerializeField] GameObject collFX;

    private Text txt;
    private Text txtBlip;

    void Start()
    {
        txt = GameObject.Find("Score").transform.GetChild(0).gameObject.GetComponent<Text>();
        txtBlip = GameObject.Find("Score").transform.GetChild(1).GetComponent<Text>();
        txtBlip.enabled = false;

        // If the gem would spawn in the floor, move it up
        if (transform.position.y < 0.75f)
        {
            transform.position = new Vector3(transform.position.x, 0.75f, transform.position.z);
        }
    }

    public void ActivateFX()
    {
        // Yes this is a little wierd to spawn on parent but t
        // was just how the treasures are organized and when
        // trying to fix it the game yells at me. There is too
        // much dependent on transform.getchild in the logic 
        Instantiate(collFX, this.transform.parent.position, Quaternion.identity);
        collFX.GetComponent<ParticleSystem>().Play();
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("PlayerHand"))
    //    {
    //        Debug.Log("Toucing Treasure");
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("PlayerHand"))
    //    {
    //        audSrc.PlayOneShot(treasureCollect);
    //        Debug.Log("Gathered Treasure");
    //        Destroy(gameObject);
    //        Debug.Log("Deleted");
    //    }
    //}


    // COLLISION IS HANDLED BY HANDS -Narai 

   /* private void OnTriggerEnter(Collider collision)
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
    }*/
}
