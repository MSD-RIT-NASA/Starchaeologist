using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBats : MonoBehaviour
{
    [SerializeField]
    private SpawnBats batSpawner;


    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            batSpawner.DeleteBats();
        }
    }
}
