using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBats : MonoBehaviour
{
    [SerializeField]
    private GameObject batPrefab;

    [SerializeField]
    private List<GameObject> spawnPoints;

    private int numBats;
    private float batSpeed;

    private List<GameObject> bats;

    // Start is called before the first frame update
    void Start()
    {
        numBats = 0;
        batSpeed = 0.05f;
        bats = new List<GameObject>();
        InvokeRepeating("BatSpawn", 0.0f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < numBats; i++)
        {
            bats[i].transform.position += bats[i].transform.up * batSpeed;
        }
    }


    public void RepeatBat()
    {
        //Invoke("BatSpawn", 0.2f);
    }


    public void BatSpawn()
    {
        //Spawn bat at a random spawnpoint chosen from the list
        int randIndex = (int)Random.Range(0.0f, 5.0f);
        bats.Add(Instantiate(batPrefab, spawnPoints[randIndex].transform));

        //
        //bats[numBats].transform.Rotate((5f * numBats), 0.0f, 0.0f, Space.Self);
        numBats++;

        if(numBats > 10)
        {
            CancelInvoke("BatSpawn");
        }
    }
}
