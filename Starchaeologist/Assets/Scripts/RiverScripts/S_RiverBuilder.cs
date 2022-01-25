using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RiverBuilder : MonoBehaviour
{

    public int segmentCount = 10;
    public int artifactPieces = 4;
    Vector3 spawnPosition = new Vector3(0,0,0);
    GameObject newSpawn;

    public List<GameObject> segmentPrefabs = new List<GameObject>();
    public List<GameObject> obstaclePrefabs = new List<GameObject>();
    public List<GameObject> treasurePrefabs = new List<GameObject>();
    public List<GameObject> artifactPrefabs = new List<GameObject>();
    

    List<Vector3> obstacleSpawns = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        //spawn the amount of river segemnts requested
        int i = 0;
        while (i < segmentCount)
        {
            //choose one of the available segment prefabs and place it at the end of the last placed piece
            newSpawn = Instantiate(segmentPrefabs[Random.Range(0, segmentPrefabs.Count)]);
            newSpawn.transform.position = spawnPosition;
            spawnPosition = newSpawn.transform.GetChild(1).transform.position;

            //record the positions available for spawning obstacles and artifacts
            int j = 2;
            while(j < newSpawn.transform.childCount)
            {
                obstacleSpawns.Add(newSpawn.transform.GetChild(j).transform.position);
                j++;
            }

            i++;
        }

        //spawn artifact pieces, treasure, and obstacles along the river
        i = 0;
        while(i < segmentCount)
        {
            //artifacts spawner
            if(i < artifactPieces)
            {
                //do this once there are actual pieces. depending on the artifact, this will change based on the amount of pieces
                //newSpawn = Instantiate(artifactPrefabs[i]);
                PlaceThings(artifactPrefabs[0]);
            }

            //treasure spawner
            PlaceThings(treasurePrefabs[Random.Range(0, treasurePrefabs.Count)]);

            //obstacle spawner
            PlaceThings(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)]);


            i++;
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //choose a location from the list to place the item
    private Vector3 PlaceThings()
    {
        //if there are no locations to spawn along the river place it elsewhere
        Vector3 givePosition = new Vector3(0, -100, 0);

        if (obstacleSpawns.Count != 0)
        {
            int spawnIndex = Random.Range(0, obstacleSpawns.Count);
            givePosition = obstacleSpawns[spawnIndex];
            obstacleSpawns.RemoveAt(spawnIndex);
        }
        
        return givePosition;
    }

    //choose a location from the list to place the item
    private void PlaceThings(GameObject spawnThis)
    {
        //see if there is a location to spawn at
        Vector3 givePosition;
        if (obstacleSpawns.Count != 0)
        {
            int spawnIndex = Random.Range(0, obstacleSpawns.Count);
            givePosition = obstacleSpawns[spawnIndex];
            obstacleSpawns.RemoveAt(spawnIndex);
        }
        else
        {
            Debug.Log("No more room");
            return;
        }

        newSpawn = Instantiate(spawnThis);
        newSpawn.transform.position = givePosition;
        newSpawn.transform.rotation = Quaternion.Euler(0, (Random.Range(0, 2) * 180), 0);
    }
}
