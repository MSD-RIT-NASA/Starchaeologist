using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RiverBuilder : MonoBehaviour
{

    public int segmentCount = 10;
    //Vector3 spawnPosition = new Vector3(0,0,0);
    
    List<GameObject> spawnedSegments = new List<GameObject>();
    //GameObject newSpawn;

    [SerializeField] List<GameObject> segmentPrefabs_2M = new List<GameObject>();
    [SerializeField] GameObject riverStart;

    [SerializeField] List<GameObject> obstaclePrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> treasurePrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> artifactPrefabs = new List<GameObject>();

    List<GameObject>[] segmentArray;
    List<GameObject>[] transitionArray;

    List<List<Vector3>> obstacleSpawns;

    // Start is called before the first frame update
    void Start()
    {
        //SegmentSetup();
        BuildRiver();

        //give the game script the list of river pieces
        GetComponent<S_RiverGame>().riverReferences = spawnedSegments;
        GetComponent<PythonCommunicator>().gameMode = 1;

        //remove this script
        Destroy(this);
    }


    private void BuildRiver()
    {
        //spawn the amount of river segemnts requested
        spawnedSegments.Add(riverStart);
        //spawnedSegments[0].transform.position.Set(0,0,0);

        PlaceThings(spawnedSegments[0]);
    }

    //choose a location from the list to place the item
    private void PlaceThings(GameObject spawnThis)
    {
        GameObject newSpawn = Instantiate(spawnThis, spawnedSegments[0].transform);
    }
}
