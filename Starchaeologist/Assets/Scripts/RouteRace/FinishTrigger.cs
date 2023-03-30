using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] RaceGame raceScr;

    void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Player")
        {
            raceScr.PlayerCrossed();
        }
    }
}
