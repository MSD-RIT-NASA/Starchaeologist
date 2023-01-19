using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBuilder : MonoBehaviour
{
    public int segmentCount = 5;

    List<GameObject> spawnedSegments = new List<GameObject>();

    public List<GameObject> segmentPrefabs_2M = new List<GameObject>();
    public List<GameObject> obstaclePrefabs = new List<GameObject>();

    List<GameObject>[] segmentArray;

    List<List<Vector3>> obstacleSpawns;

    // Start is called before the first frame update
    void Start()
    {
        DataSetup();
        SegmentSetup();

        GetComponent<MineGame>().trackReferences = spawnedSegments;
        //remove the script 
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DataSetup()
    {
        obstacleSpawns = new List<List<Vector3>>(segmentCount);
        segmentArray = new List<GameObject>[1];
        segmentArray[0] = segmentPrefabs_2M;
    }

    private void SegmentSetup()
    {
        //spawn the amount of track requested
        spawnedSegments.Add(GameObject.Find("Plane"));

        Vector3 spawnPosition = new Vector3(0, -1, 5);
        int i = 0;
        while (i < segmentCount)
        {
            //choose one of the available segment prefabs and place it at the end of the last placed piece
            GameObject newSpawn = Instantiate(segmentArray[0][Random.Range(0, segmentArray[0].Count)]);
            newSpawn.transform.position = spawnPosition;

            if (i >= 5)
            {
                newSpawn.SetActive(false);
            }
            spawnPosition = new Vector3(0, -1, newSpawn.transform.position.z+10);

            spawnedSegments.Add(newSpawn);

            //record the positions available for spawning obstacles and artifacts
            obstacleSpawns.Add(new List<Vector3>());
            obstacleSpawns[i].Add(new Vector3(0, 0, newSpawn.transform.position.z));
            int j = 2;
            while (j < newSpawn.transform.childCount)
            {
                obstacleSpawns[i].Add(new Vector3(0, 0, newSpawn.transform.position.z));
                j++;
            }

            i++;
        }

        //spawn artifact pieces, treasure, and obstacles along the river
        i = 0;
        while (i < segmentCount)
        {
            ////artifacts spawner
            //if (i < artifactPieces)
            //{
            //    //do this once there are actual pieces. depending on the artifact, this will change based on the amount of pieces
            //    //newSpawn = Instantiate(artifactPrefabs[i]);
            //    PlaceThings(artifactPrefabs[0]);
            //}

            //treasure spawner
            // PlaceThings(treasurePrefabs[Random.Range(0, treasurePrefabs.Count)]);

            //obstacle spawner
            PlaceThings(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)]);


            i++;
        }

    }

    //choose a location from the list to place the item
    private void PlaceThings(GameObject spawnThis)
    {
        //see if there is a location to spawn at
        Vector3 givePosition = Vector3.zero;
        int i = -1;
        while (givePosition == Vector3.zero)
        {
            //choose a river segment to spawn on
            i = Random.Range(0, obstacleSpawns.Count);
            if (obstacleSpawns[i].Count != 0)
            {
                //choose a checkpoint on said river to spawn at
                int j = 0; //Random.Range(0, obstacleSpawns[i].Count);
                givePosition = obstacleSpawns[i][j];
                obstacleSpawns[i].RemoveAt(j);
            }
            else
            {
                obstacleSpawns.RemoveAt(i);
            }
            
        }

        i++;
        GameObject newSpawn = Instantiate(spawnThis, spawnedSegments[i].transform);
        givePosition.x += Random.Range(-1, 2);
        newSpawn.transform.position = givePosition;
        newSpawn.transform.rotation = Quaternion.Euler(0, (Random.Range(0, 2) * 180), 0);
    }
}
