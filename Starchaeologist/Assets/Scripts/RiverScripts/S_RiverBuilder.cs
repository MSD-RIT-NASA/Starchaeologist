using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RiverBuilder : MonoBehaviour
{

    public int segmentCount = 10;
    public int artifactPieces = 4;
    //Vector3 spawnPosition = new Vector3(0,0,0);
    
    List<GameObject> spawnedSegments = new List<GameObject>();
    //GameObject newSpawn;

    public List<GameObject> segmentPrefabs_2M = new List<GameObject>();
    public List<GameObject> segmentPrefabs_3M = new List<GameObject>();
    public List<GameObject> segmentPrefabs_4M = new List<GameObject>();
    public List<GameObject> segmentPrefabs_5M = new List<GameObject>();
    public List<GameObject> transitionPrefabs_to_2M = new List<GameObject>();
    public List<GameObject> transitionPrefabs_to_3M = new List<GameObject>();
    public List<GameObject> transitionPrefabs_to_4M = new List<GameObject>();
    public List<GameObject> transitionPrefabs_to_5M = new List<GameObject>();

    public List<GameObject> obstaclePrefabs = new List<GameObject>();
    public List<GameObject> treasurePrefabs = new List<GameObject>();
    public List<GameObject> artifactPrefabs = new List<GameObject>();

    List<GameObject>[] segmentArray;
    List<GameObject>[] transitionArray;

    List<Vector3>[] obstacleSpawns;

    // Start is called before the first frame update
    void Start()
    {
        DataSetup();

        //SegmentSetup();
        SegmentSetupTwo();

        //give the game script the list of river pieces
        GetComponent<S_RiverGame>().riverReferences = spawnedSegments;

        //remove this script
        Destroy(this);
    }

    private void DataSetup()
    {
        obstacleSpawns = new List<Vector3>[segmentCount];

        //set up the segments given in the scene into a managable array
        segmentArray = new List<GameObject>[4];
        segmentArray[0] = segmentPrefabs_2M;
        segmentArray[1] = segmentPrefabs_3M;
        segmentArray[2] = segmentPrefabs_4M;
        segmentArray[3] = segmentPrefabs_5M;
        //same with the transition pieces
        transitionArray = new List<GameObject>[4];
        transitionArray[0] = transitionPrefabs_to_2M;
        transitionArray[1] = transitionPrefabs_to_3M;
        transitionArray[2] = transitionPrefabs_to_4M;
        transitionArray[3] = transitionPrefabs_to_5M;
    }

    private void SegmentSetup()
    {
        //spawn the amount of river segemnts requested
        spawnedSegments.Add(GameObject.Find("RiverStart"));

        Vector3 spawnPosition = new Vector3(0, 0, 0);
        int i = 0;
        while (i < segmentCount)
        {
            //choose one of the available segment prefabs and place it at the end of the last placed piece
            GameObject newSpawn = Instantiate(segmentPrefabs_2M[Random.Range(0, segmentPrefabs_2M.Count)]);
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
    }

    private void SegmentSetupTwo()
    {
        //spawn the amount of river segemnts requested
        spawnedSegments.Add(GameObject.Find("RiverStart"));

        Vector3 spawnPosition = new Vector3(0, 0, 0);
        int bankHeight = 0;
        int i = 0;
        while (i < segmentCount)
        {
            //record the current height of the river banks and choose a height for the new segment
            int oldHeight = bankHeight;
            //the new height can only be within 2 height differences because there is no 2-5 incline given
            bankHeight = Mathf.Clamp(bankHeight + Random.Range(-2, 3), 0, segmentArray.Length - 1);

            //place the transition piece
            GameObject transitionPiece = Instantiate(transitionArray[bankHeight][oldHeight]);
            transitionPiece.transform.position = spawnPosition;
            spawnPosition = transitionPiece.transform.GetChild(1).transform.position;

            //choose one of the available segment prefabs and place it at the end of the last placed piece
            GameObject newSpawn = Instantiate(segmentArray[bankHeight][Random.Range(0, segmentArray[bankHeight].Count)]);
            newSpawn.transform.position = spawnPosition;
            if (i >= 5)
            {
                newSpawn.SetActive(false);
            }
            spawnPosition = newSpawn.transform.GetChild(1).transform.position;

            //set the transition piece as a child of the mesh of the new segment and add the segment to the list
            transitionPiece.transform.SetParent(newSpawn.transform.GetChild(0).transform);
            spawnedSegments.Add(newSpawn);


            //record the positions available for spawning obstacles and artifacts
            obstacleSpawns[i] = new List<Vector3>();
            obstacleSpawns[i].Add(newSpawn.transform.position);
            int j = 2;
            while (j < newSpawn.transform.childCount)
            {
                obstacleSpawns[i].Add(newSpawn.transform.GetChild(j).transform.position);
                j++;
            }
            obstacleSpawns[i].Add(newSpawn.transform.GetChild(1).transform.position);

            i++;
        }

        //place the end of the river at the end of the river
        GameObject endReference = GameObject.Find("RiverEnd");
        endReference.transform.position = spawnPosition;
        endReference.SetActive(false);
        spawnedSegments.Add(endReference);

        //spawn artifact pieces, treasure, and obstacles along the river
        i = 0;
        while (i < segmentCount*2)
        {
            ////artifacts spawner
            //if (i < artifactPieces)
            //{
            //    //do this once there are actual pieces. depending on the artifact, this will change based on the amount of pieces
            //    //newSpawn = Instantiate(artifactPrefabs[i]);
            //    PlaceThings(artifactPrefabs[0]);
            //}

            //treasure spawner
            PlaceThings(treasurePrefabs[Random.Range(0, treasurePrefabs.Count)]);

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
