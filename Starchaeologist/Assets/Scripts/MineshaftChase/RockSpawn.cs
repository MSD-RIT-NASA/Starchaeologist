using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawn : MonoBehaviour
{
    private bool hasSpawnedHere;


    public bool Spawned
    {
        get { return hasSpawnedHere; }
        set { hasSpawnedHere = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        hasSpawnedHere = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
