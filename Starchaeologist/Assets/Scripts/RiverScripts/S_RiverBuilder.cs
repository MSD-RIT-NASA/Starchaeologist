using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RiverBuilder : MonoBehaviour
{

    public int segmentCount = 10;
    public int artifactPieces = 4;
    Vector3 spawnPosition = new Vector3(0,0,0);
    List<GameObject> spawnedSegments = new List<GameObject>();
    //GameObject newSpawn;

    public List<GameObject> segmentPrefabs = new List<GameObject>();
    public List<GameObject> obstaclePrefabs = new List<GameObject>();
    public List<GameObject> treasurePrefabs = new List<GameObject>();
    public List<GameObject> artifactPrefabs = new List<GameObject>();
    
    
    List<Vector3>[] obstacleSpawns;

    // Start is called before the first frame update
    void Start()
    {
        //spawn the amount of river segemnts requested
        //GetComponent<S_RiverGame>().riverReferences.Add(GameObject.Find("RiverStart"));
        spawnedSegments.Add(GameObject.Find("RiverStart"));
        obstacleSpawns = new List<Vector3>[segmentCount];

        int i = 0;
        while (i < segmentCount)
        {
            //choose one of the available segment prefabs and place it at the end of the last placed piece
            GameObject newSpawn = Instantiate(segmentPrefabs[Random.Range(0, segmentPrefabs.Count)]);
            newSpawn.transform.position = spawnPosition;
            if(i >= 5)
            {
                newSpawn.SetActive(false);
            }
            spawnPosition = newSpawn.transform.GetChild(1).transform.position;

            spawnedSegments.Add(newSpawn);

            
            //record the positions available for spawning obstacles and artifacts
            obstacleSpawns[i] = new List<Vector3>();
            int j = 2;
            while(j < newSpawn.transform.childCount)
            {
                obstacleSpawns[i].Add(newSpawn.transform.GetChild(j).transform.position);
                j++;
            }

            i++;
        }

        //place the end of the river at the end of the river
        GameObject endReference = GameObject.Find("RiverEnd");
        endReference.transform.position = spawnPosition;
        endReference.SetActive(false);
        spawnedSegments.Add(endReference);

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

        //give the game script the list of river pieces
        GetComponent<S_RiverGame>().riverReferences = spawnedSegments;

        //remove this script
        Destroy(this);
    }

    //choose a location from the list to place the item
    private void PlaceThings(GameObject spawnThis)
    {
        //see if there is a location to spawn at
        Vector3 givePosition = Vector3.zero;
        int objectAttempts = 0;
        int i = -1;
        while (givePosition == Vector3.zero)
        {
            //choose a river segment to spawn on
            i = Random.Range(0, obstacleSpawns.Length);

            if (obstacleSpawns[i].Count != 0)
            {
                //choose a checkpoint on said river to spawn at
                int j = Random.Range(0, obstacleSpawns[i].Count);
                givePosition = obstacleSpawns[i][j];
                obstacleSpawns[i].RemoveAt(j);
            }
            else if(objectAttempts >= 10)
            {
                Debug.Log("No more room");
                return;
            }

            objectAttempts++;
        }

        i++;
        GameObject newSpawn = Instantiate(spawnThis, spawnedSegments[i].transform);
        newSpawn.transform.position = givePosition;
        newSpawn.transform.rotation = Quaternion.Euler(0, (Random.Range(0, 2) * 180), 0);
    }
}
