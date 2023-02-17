using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public List<ParticleSystem> explosions;

    [SerializeField]
    private CameraShake mainCam;

    void Start()
    {
        explosions = new List<ParticleSystem>();
    }

    public void OnTriggerEnter(Collider trigger)
    {
        //If the player passes through the TNT Zone
        if(trigger.gameObject.tag == "TntZone")
        {
            //Activate the explosion chain for a set distance
        }
    }
}
