//NASA x RIT author: Noah Flanders

//Handles the explosion animations and the falling rock spawns
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartExplosions : MonoBehaviour
{
    [SerializeField]
    private List<ParticleSystem> explosions;

    [SerializeField]
    private List<RockSpawn> rockSpawns;

    [SerializeField]
    private GameObject rockPrefab;

    [SerializeField]
    private AudioSource audSrc;
    [SerializeField]
    private AudioClip explosion3;

    private List<GameObject> rocks;
    private int rocksSpawned;

    // Start is called before the first frame update
    void Start()
    {
        rocks = new List<GameObject>();
        rocksSpawned = 0;
        for (int i = 0; i < explosions.Count; i++)
        {
            explosions[i].Stop();
        }
    }

    void Update()
    {
        for(int i = 0; i < rocks.Count; i++)
        {
            //Moves rocks if they exist
            if (rocks[i] != null)
            {
                rocks[i].transform.position = new Vector3(
                    rocks[i].transform.position.x,
                    rocks[i].transform.position.y - 0.2f,
                    rocks[i].transform.position.z
                    );
                float rotRateX = Random.Range(0f, 2f);
                float rotRateY = Random.Range(0f, 2f);
                float rotRateZ = Random.Range(0f, 2f);
                rocks[i].transform.eulerAngles = new Vector3(
                    rocks[i].transform.eulerAngles.x - rotRateX,
                    rocks[i].transform.eulerAngles.y - rotRateY,
                    rocks[i].transform.eulerAngles.z - rotRateZ
                    );
            }
        }
    }

    /// <summary>
    /// Starts explosion animations and falling rocks once the player enters the TNTTrack
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        audSrc.PlayOneShot(explosion3);
        if (other.gameObject.tag == "PlayerHead")
        {
            for(int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Play();
            }

            InvokeRepeating("DropRock", 0.0f, 0.3f);
        }
    }

    //Deletes the rocks when the player exits the TNTTrack
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            for(int i = 0; i < rocks.Count; i++)
            {
                Destroy(rocks[i]);
                rocks[i] = null;
            }
        }
    }

    //Drops new rocks in each possible spawn location (not twice in one spot)
    private void DropRock()
    {
        int randIndex = (int)Random.Range(0, 10);

        //If a rock has spawned in one location, pick a different one
        while (rockSpawns[randIndex].Spawned)
        {
            randIndex = (int)Random.Range(0, 10);

            //Spawns 10 rocks
            if(rocksSpawned >= 10)
            {
                CancelInvoke("DropRock");
                break;
            }
        }

        //New rock instance with random size
        rocks.Add(Instantiate(rockPrefab, rockSpawns[randIndex].transform));
        float randScale = Random.Range(0f, 10f);
        rocks[rocksSpawned].transform.localScale = new Vector3(
            rocks[rocksSpawned].transform.localScale.x * randScale,
            rocks[rocksSpawned].transform.localScale.y * randScale,
            rocks[rocksSpawned].transform.localScale.z * randScale
            );
        rocksSpawned++;
        rockSpawns[randIndex].Spawned = true;
    }
}
