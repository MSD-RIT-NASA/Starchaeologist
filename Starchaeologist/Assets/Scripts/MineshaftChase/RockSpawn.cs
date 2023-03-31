//NASA x RIT author: Noah Flanders

//To be attached to the spawned rock prefabs 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawn : MonoBehaviour
{
    //To keep track of the locations the rocks spawned to prevent multiple spawning
    //in the same location
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
