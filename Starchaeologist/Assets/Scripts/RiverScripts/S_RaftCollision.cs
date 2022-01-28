using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class S_RaftCollision : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        //when the player first comes into contact with the raft, tell the game to start playing then remove this script to save space
        if (other.gameObject.CompareTag("Player"))
        {
            //tell the platform to start moving
            

            //stop the player from teleporting back to land once the ride starts
            GameObject startReference = GameObject.Find("RiverStart");
            for (int i = 0; i < startReference.transform.childCount; i++)
            {
                if(startReference.transform.GetChild(i).GetComponent<TeleportationArea>())
                {
                    Destroy(startReference.transform.GetChild(i).GetComponent<TeleportationArea>());
                    //startReference.transform.GetChild(i).GetComponent<TeleportationArea>().enabled = false;
                }
            }
            
            GameObject.Find("Game Manager").GetComponent<S_RiverGame>().timeToMove = true;

            //stop the player from teleporting back onto the raft
            GetComponent<TeleportationAnchor>().enabled = false;

            //remove this script
            Destroy(this);
        }
    }

    void OnTriggerExit(Collider other)
    {

    }
}
