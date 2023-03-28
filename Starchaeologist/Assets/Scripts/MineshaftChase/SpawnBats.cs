//NASA x RIT author: Noah Flanders

//Spawns and moves bat objects when a player is on the BatTrack
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBats : MonoBehaviour
{
    [SerializeField]
    private GameObject batPrefab;

    [SerializeField]
    private List<GameObject> spawnPoints;

    [SerializeField]
    private AudioSource audSrc;
    [SerializeField]
    private AudioClip batChirp;

    private int numBats;
    private float batSpeed;

    private List<GameObject> bats;

    // Start is called before the first frame update
    void Start()
    {
        numBats = 0;
        batSpeed = 0.075f;
        bats = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < numBats; i++)
        {
            if (bats[i] != null)
            {
                bats[i].transform.position += bats[i].transform.up * batSpeed;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "PlayerHead")
        {
            audSrc.PlayOneShot(batChirp);
            InvokeRepeating("BatSpawn", 0.0f, 0.3f);
        }
    }


    public void BatSpawn()
    {
        //Spawn bat at a random spawnpoint chosen from the list
        int randIndex = (int)Random.Range(0.0f, 5.0f);
        bats.Add(Instantiate(batPrefab, spawnPoints[randIndex].transform));
        numBats++;

        //Spawns 10 bats
        if(numBats > 10)
        {
            CancelInvoke("BatSpawn");
        }
    }

    //Deletes bats once the player has left the BatTrack
    public void DeleteBats()
    {
        for (int i = 0; i < bats.Count; i++)
        {
            Destroy(bats[i]);
            bats[i] = null;
        }
    }
}
