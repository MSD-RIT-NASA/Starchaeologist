//NASA x RIT author: Noah Flanders

//Cleans up the unnecessary bat models in the scene to improve performace
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBats : MonoBehaviour
{
    [SerializeField]
    private SpawnBats batSpawner;

    [SerializeField]
    private AudioSource audSrc;

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            audSrc.Stop();
            batSpawner.DeleteBats();
        }
    }
}
