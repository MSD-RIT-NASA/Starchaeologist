using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private TempleGameManager gm;
    private bool isDangerous;

    private void Start()
    {
        gm = GameObject.FindObjectOfType<TempleGameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isDangerous)
            return;

        if(other.tag == "PlayerHand")
        {
            gm.TakeCollision();
        }
    }
}
