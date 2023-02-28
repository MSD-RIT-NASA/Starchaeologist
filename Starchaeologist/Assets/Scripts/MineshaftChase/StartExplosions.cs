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

    private List<GameObject> rocks;

    // Start is called before the first frame update
    void Start()
    {
        rocks = new List<GameObject>();
        for (int i = 0; i < explosions.Count; i++)
        {
            explosions[i].Stop();
        }
    }

    void Update()
    {
        for(int i = 0; i < rocks.Count; i++)
        {
            rocks[i].transform.position = new Vector3(
                rocks[i].transform.position.x,
                rocks[i].transform.position.y - 0.2f,
                rocks[i].transform.position.z
                );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            for(int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Play();
            }

            for(int i = 0; i < rockSpawns.Count; i++)
            {
                int randIndex = (int)Random.Range(0, 10);
                while (rockSpawns[randIndex].Spawned)
                {
                    randIndex = (int)Random.Range(0, 10);
                }

                rocks.Add(Instantiate(rockPrefab, rockSpawns[randIndex].transform));
            }
        }
    }
}
