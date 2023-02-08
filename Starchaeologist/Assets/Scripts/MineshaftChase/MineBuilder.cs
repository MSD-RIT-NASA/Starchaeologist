using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBuilder : MonoBehaviour
{
    [SerializeField] GameObject shadowReference;
    public int segmentCount = 5;

    List<GameObject> spawnedSegments = new List<GameObject>();
    List<Transform> spawnedTransforms = new List<Transform>();

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
        GetComponent<MineGame>().routes = spawnedTransforms;
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
        spawnedSegments.Add(GameObject.Find("Track"));
        spawnedTransforms.Add(GameObject.Find("Track").transform.GetChild(2).transform);

        Vector3 spawnPosition = new Vector3(0, -1, 20);
        Quaternion spawnRotation = new Quaternion(0, 0, 0, 0);
        int i = 0;
        while (i < segmentCount)
        {
            //choose one of the available segment prefabs and place it at the end of the last placed piece
            GameObject newSpawn = Instantiate(segmentArray[0][Random.Range(0, segmentArray[0].Count)]);
            newSpawn.transform.position = spawnPosition;
            newSpawn.transform.rotation = spawnRotation;

            if (i >= 2)
            {
                newSpawn.SetActive(false);
            }
            spawnPosition = newSpawn.transform.GetChild(0).transform.position;
            spawnRotation = newSpawn.transform.GetChild(0).transform.rotation;

            spawnedSegments.Add(newSpawn);
            spawnedTransforms.Add(newSpawn.transform.GetChild(2).transform);
            //record the positions available for spawning obstacles and artifacts
            obstacleSpawns.Add(new List<Vector3>());
            int j = 3;
            while (j < 6)
            {
                obstacleSpawns[i].Add(newSpawn.transform.GetChild(j).transform.localPosition);
                j++;
            }
            i++;
        }

        GameObject endReference = GameObject.Find("TrackEnd");
        endReference.transform.position = spawnPosition;
        endReference.transform.rotation = spawnRotation;
        endReference.SetActive(false);
        spawnedSegments.Add(endReference);
        spawnedTransforms.Add(endReference.transform.GetChild(2).transform);

        //place shadow 
        shadowReference.transform.position = spawnedSegments[1].transform.GetChild(0).transform.position;
        shadowReference.transform.rotation = spawnedSegments[1].transform.GetChild(0).transform.rotation;

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
            PlaceThings(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)], i);
            i++;
        }

    }

    //choose a location from the list to place the item
    private void PlaceThings(GameObject spawnThis, int segment)
    {
        //spawn 0-2 obstacles per segment
        int obstacles = Random.Range(1, 3);
        while (obstacles > 0)
        {
            //see if there is a location to spawn at
            Vector3 givePosition = Vector3.zero;
            int i = 0;
            while (givePosition == Vector3.zero)
            {
                //choose a river segment to spawn on
                if (obstacleSpawns[i].Count != 0)
                {
                    //choose a checkpoint on said river to spawn at
                    int j = Random.Range(0, obstacleSpawns[i].Count);
                    givePosition = obstacleSpawns[i][j];
                    obstacleSpawns[i].RemoveAt(j);
                }
                if (obstacleSpawns[i].Count == 0)
                {
                    obstacleSpawns.RemoveAt(i);
                }
            }
            obstacles--;

            i++;
            GameObject newSpawn = Instantiate(spawnThis, spawnedSegments[segment].transform);
            givePosition.y += 1;
            newSpawn.transform.localPosition = givePosition;
            //newSpawn.transform.rotation = Quaternion.Euler(0, (Random.Range(0, 2) * 180), 0);
        }
    }
}
