using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void OnTriggerEnter(Collider trigger)
    {
        //If the player passes through the TNT Zone
        if(trigger.gameObject.tag == "TntZone")
        {
            //Start shaking camera
        }
    }
}
