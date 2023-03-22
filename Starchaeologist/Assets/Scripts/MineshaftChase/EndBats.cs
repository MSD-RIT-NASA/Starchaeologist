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
