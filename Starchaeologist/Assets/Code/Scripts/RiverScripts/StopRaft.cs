using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRaft : MonoBehaviour
{
    [SerializeField]
    private S_RiverGame game;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHead"))
        {
            game.StopMove();
        }
    }
}
