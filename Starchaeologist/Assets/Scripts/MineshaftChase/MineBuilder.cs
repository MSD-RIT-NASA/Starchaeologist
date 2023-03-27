using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MineBuilder : MonoBehaviour
{
    [SerializeField] GameObject shadowReference;
    public int segmentCount = 6;

    List<GameObject> spawnedSegments = new List<GameObject>();
    List<Transform> spawnedTransforms = new List<Transform>();

    public List<GameObject> segmentPrefabs_2M = new List<GameObject>();
    public List<GameObject> obstaclePrefabs = new List<GameObject>();
    public List<GameObject> treasurePrefabs = new List<GameObject>();
    [SerializeField]
    private List<GameObject> supports = new List<GameObject>();
    [SerializeField]
    private List<GameObject> sceneryObjects = new List<GameObject>();
    [SerializeField]
    private List<GameObject> planks = new List<GameObject>();

    List<GameObject>[] segmentArray;
    private List<int> usedSegments = new List<int>();
    private bool hasBeenUsed;

    // Start is called before the first frame update
    void Start()
    {
        DataSetup();
        SegmentSetup();

        GetComponent<MineGame>().trackReferences = spawnedSegments;
        GetComponent<MineGame>().routes = spawnedTransforms;

        hasBeenUsed = false;

        //remove the script 
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DataSetup()
    {
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
        while (i < segmentCount-1)
        {
            hasBeenUsed = false;

            //choose one of the available segment prefabs and place it at the end of the last placed piece
            int randIndex = (int)Random.Range(0, segmentArray[0].Count);

            for(int j = 0; j < usedSegments.Count; j++)
            {
                if(usedSegments[j] == randIndex)
                {
                    hasBeenUsed = true;
                    break;
                }
            }
            while (hasBeenUsed)
            {
                if(usedSegments.Count >= 8)
                {
                    usedSegments.Clear();
                }
                hasBeenUsed = false;
                randIndex = (int)Random.Range(0, segmentArray[0].Count);

                for (int j = 0; j < usedSegments.Count; j++)
                {
                    if (usedSegments[j] == randIndex)
                    {
                        hasBeenUsed = true;
                        break;
                    }
                }
            }

            GameObject newSpawn = Instantiate(segmentArray[0][randIndex]);
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

            //If the track is a repeating turns track, add each turn as its own segment
            if(segmentArray[0][randIndex].gameObject.name == "RepeatedTurns_Track")
            {
                for (int j = 12; j < 17; j++)
                {
                    newSpawn.transform.GetChild(j).gameObject.transform.position = spawnPosition;
                    newSpawn.transform.GetChild(j).gameObject.transform.rotation = spawnRotation;
                    spawnPosition = newSpawn.transform.GetChild(j).gameObject.transform.GetChild(0).transform.position;
                    spawnRotation = newSpawn.transform.GetChild(j).gameObject.transform.GetChild(0).transform.rotation;

                    spawnedSegments.Add(newSpawn.transform.GetChild(j).gameObject);
                    spawnedTransforms.Add(newSpawn.transform.GetChild(j).transform.GetChild(2).transform);
                    PlaceSupports(spawnedSegments[i], supports);
                    PlaceScenery(spawnedSegments[i], sceneryObjects);
                    i++;
                }
            }

            //spawn objects on the segment
            if (spawnedSegments[i].GetComponent<SpawnObstacles>().ShouldObstSpawn)
            {
                Debug.Log(spawnedSegments[i].gameObject.name + "should spawn obstacles: " + spawnedSegments[i].GetComponent<SpawnObstacles>().ShouldObstSpawn);
                PlaceObjects(spawnedSegments[i], obstaclePrefabs);
                PlaceObjects(spawnedSegments[i], treasurePrefabs);
            }
            else
            {
                Debug.Log(spawnedSegments[i].gameObject.name + "did not yield obstacles");
            }
            
            PlaceSupports(spawnedSegments[i], supports);
            PlaceScenery(spawnedSegments[i], sceneryObjects);
            i++;

            usedSegments.Add(randIndex);

            Transform newSpawnTrans = SpawnTrack(spawnPosition, spawnRotation, i);
            spawnPosition = newSpawnTrans.position;
            spawnRotation = newSpawnTrans.rotation;
            if (spawnedSegments[i].GetComponent<SpawnObstacles>().ShouldObstSpawn)
            {
                Debug.Log(spawnedSegments[i].gameObject.name + "should spawn obstacles: " + spawnedSegments[i].GetComponent<SpawnObstacles>().ShouldObstSpawn);
                PlaceObjects(spawnedSegments[i], obstaclePrefabs);
                PlaceObjects(spawnedSegments[i], treasurePrefabs);
            }
            PlaceSupports(spawnedSegments[i], supports);
            PlaceScenery(spawnedSegments[i], sceneryObjects);
            i++;
        }
        
        GameObject endReference = GameObject.Find("TrackEnd");
        endReference.transform.position = spawnPosition;
        endReference.transform.rotation = spawnRotation;
        endReference.SetActive(false);
        spawnedSegments.Add(endReference);
        spawnedTransforms.Add(endReference.transform.GetChild(2).transform);
        PlaceObjects(spawnedSegments[i], obstaclePrefabs);
        PlaceObjects(spawnedSegments[i+1], obstaclePrefabs);
        PlaceSupports(spawnedSegments[i], supports);
        PlaceSupports(spawnedSegments[i+1], supports);
        PlaceScenery(spawnedSegments[i], sceneryObjects);
        //place shadow 
        shadowReference.transform.position = spawnedSegments[0].transform.GetChild(0).transform.position;
        shadowReference.transform.rotation = spawnedSegments[0].transform.GetChild(0).transform.rotation;

    }

    //choose a location from the list of positions to place the item
    private void PlaceObjects(GameObject segment, List<GameObject> objects)
    {
        List<GameObject> placements = new List<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            placements.Add(segment.transform.GetChild(i + 3).gameObject);
        }

        int objectToPlace = Random.Range(0, objects.Count);
        int count = Random.Range(1, 3);
        int place = Random.Range(0, 3);
        while (count > 0)
        {
            
            //Put the stalactites in the middle
            if (placements[place].name == "ObstaclePointMid")
            {
                objectToPlace = (int)Random.Range(0, 3);
            }
            else
            {
                objectToPlace = (int)Random.Range(3, 5);
            }
            GameObject obst = Instantiate(objects[objectToPlace], placements[place].transform);


            //Rotate, scale up, and move signs down
            if (objectToPlace >= 3 && objectToPlace < 5)
            {
                obst.transform.localScale = new Vector3(
                    obst.transform.localScale.x * 1f,
                    obst.transform.localScale.y * 1.5f,
                    obst.transform.localScale.z * 2.5f
                );

                obst.transform.position = new Vector3(
                    obst.transform.position.x,
                    obst.transform.position.y - 3f,
                    obst.transform.position.z
                    );

                obst.transform.eulerAngles = new Vector3(
                    obst.transform.eulerAngles.x,
                    obst.transform.eulerAngles.y + 90,
                    obst.transform.eulerAngles.z
                    );
            }
            else
            {
                //Scale up stalactites
                obst.transform.localScale = new Vector3(
                    obst.transform.localScale.x * 1.5f,
                    obst.transform.localScale.y * 1.5f,
                    obst.transform.localScale.z * 1.5f
                    );
                //Move Up 
                obst.transform.position = new Vector3(
                    obst.transform.position.x,
                    obst.transform.position.y + 0.6f,
                    obst.transform.position.z
                    );
                
            }
            placements.Remove(placements[place]);
            objectToPlace = Random.Range(0, objects.Count);
            place = Random.Range(0, 2);
            count--;
        }
        
    }

    /// <summary>
    /// Places one of three support models at the support point positions in each track segment along with possible connector planks
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="beams"></param>
    private void PlaceSupports(GameObject segment, List<GameObject> beams)
    {
        List<Transform> placements = new List<Transform>();
        for(int i = 6; i < 9; i++)
        {
            if (segment.transform.GetChild(i).gameObject.tag == "SupportPoint")
            {
                placements.Add(segment.transform.GetChild(i).gameObject.transform);
            }
        }

        for (int i = 0; i < placements.Count; i++)
        {
            int randBeam = (int)Random.Range(0, 3);

            //Place support beams
            GameObject obst = Instantiate(beams[randBeam], placements[i]);


            //Scale Supports correctly
            obst.transform.localScale = new Vector3(
                obst.transform.localScale.x,
                0.005f,
                obst.transform.localScale.z
            );
        }

        placements.Clear();

        if (segment.transform.GetChild(9).gameObject.tag == "PlankPointList")
        {
            GameObject plankPointList = segment.transform.GetChild(9).gameObject;
            for (int i = 0; i < 8; i++)
            {
                placements.Add(plankPointList.transform.GetChild(i).gameObject.transform);
            }

            for (int i = 0; i < placements.Count; i++)
            {
                float randChance = Random.Range(0, 2);
                if (randChance < 1f)
                {
                    int randPlank = (int)Random.Range(0, 3);
                    GameObject plank = Instantiate(planks[randPlank], placements[i]);

                    plank.transform.localScale = new Vector3(
                        plank.transform.localScale.x * 1.3f,
                        plank.transform.localScale.y * 1.3f,
                        plank.transform.localScale.z * 1.3f
                    );

                    plank.transform.eulerAngles = new Vector3(
                        0f,
                        plank.transform.eulerAngles.y,
                        plank.transform.eulerAngles.z
                    );
                }
            }
        }
    }


    private void PlaceScenery(GameObject segment, List<GameObject> objs)
    {
        List<Transform> placements = new List<Transform>();
        for (int i = 7; i < 11; i++)
        {
            if (segment.transform.GetChild(i).gameObject.tag == "SceneryPointList")
            {
                for (int j = 0; j < 6; j++)
                {
                    placements.Add(segment.transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.transform);
                }
            }
        }

        for(int i = 0; i < placements.Count; i++)
        {
            int randObj = (int)Random.Range(0, 8);
            Instantiate(objs[randObj], placements[i]);
        }
    }

    private Transform SpawnTrack(Vector3 spawnPos, Quaternion spawnRot, int curSeg)
    {
        GameObject newSpawn = Instantiate(segmentArray[0][0]);
        newSpawn.transform.position = spawnPos;
        newSpawn.transform.rotation = spawnRot;

        if (curSeg >= 2)
        {
            newSpawn.SetActive(false);
        }
        spawnedSegments.Add(newSpawn);
        spawnedTransforms.Add(newSpawn.transform.GetChild(2).transform);
        return newSpawn.transform.GetChild(0).transform;
    }
}
