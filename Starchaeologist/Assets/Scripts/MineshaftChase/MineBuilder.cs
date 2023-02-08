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
    public List<GameObject> treasurePrefabs = new List<GameObject>();

    List<GameObject>[] segmentArray;


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
            
            //spawn objects on the segment
            PlaceObjects(spawnedSegments[i], obstaclePrefabs);
            PlaceObjects(spawnedSegments[i], treasurePrefabs);
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
        //place shadow 
        shadowReference.transform.position = spawnedSegments[1].transform.GetChild(0).transform.position;
        shadowReference.transform.rotation = spawnedSegments[1].transform.GetChild(0).transform.rotation;

    }

    //choose a location from the list of positions to place the item
    private void PlaceObjects(GameObject segment, List<GameObject> objects)
    {
        List<GameObject> placements = new List<GameObject>();
        for(int i = 0; i < 3; i++)
        {
            placements.Add(segment.transform.GetChild(i + 3).gameObject);
        }

        int count = Random.Range(1, 3);
        int place = Random.Range(0, 3);
        while(count>0)
        {
            Instantiate(objects[0], placements[place].transform);
            placements.Remove(placements[place]);
            place = Random.Range(0, 2);
            count--;
        }
    }
}
