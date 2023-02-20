using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class S_RiverBuilder : MonoBehaviour
{
    List<GameObject> spawnedSegments = new List<GameObject>();
    //GameObject newSpawn;
    [SerializeField] GameObject RiverPlaysection;
    [SerializeField] List<GameObject> obstaclePrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> treasurePrefabs = new List<GameObject>();
    //[SerializeField] List<GameObject> artifactPrefabs = new List<GameObject>();

    List<List<Vector3>> obstacleSpawns;
    System.Random rand = new System.Random();

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
        int counter = 0;
        for (int x = 0;x < RiverPlaysection.transform.childCount;x++)
        {
            //skip transistion sections 
            if (RiverPlaysection.transform.GetChild(x).childCount > 2)
            {
                //randomization of obsicaleType,placement, and position/rotation
                GameObject randomObstacle1 = obstaclePrefabs[rand.Next(0,2)];
                GameObject randomObstacle2 = obstaclePrefabs[rand.Next(0, 2)];
                GameObject randomTreasure = treasurePrefabs[rand.Next(0, 8)];
                int randomTreasurePos = rand.Next(0, 3);


                GameObject objPoint1 = RiverPlaysection.transform.GetChild(x).GetChild(2).gameObject;
                float point1Z = RiverPlaysection.transform.GetChild(x).GetChild(2).gameObject.transform.position.z;

                GameObject objPoint2 = RiverPlaysection.transform.GetChild(x).GetChild(3).gameObject;
                float point2Z = RiverPlaysection.transform.GetChild(x).GetChild(3).gameObject.transform.position.z;

                GameObject objPoint3 = RiverPlaysection.transform.GetChild(x).GetChild(4).gameObject;
                float point3Z = RiverPlaysection.transform.GetChild(x).GetChild(4).gameObject.transform.position.z;

                //objPoint1.transform.position.Set();
                if (randomTreasurePos == 0)
                {
                    objPoint1.transform.localPosition = new Vector3(rand.Next(-6, 9), randomTreasure.transform.position.y, randomTreasure.transform.position.z);
                    GameObject tresure = Instantiate(randomTreasure,objPoint1.transform);
                    GameObject ob1 = Instantiate(randomObstacle1, objPoint2.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);
                }
                else if (randomTreasurePos == 1)
                {
                    objPoint2.transform.localPosition = new Vector3(rand.Next(-6, 9), randomTreasure.transform.position.y, randomTreasure.transform.position.z);
                    GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                    GameObject tresure = Instantiate(randomTreasure, objPoint2.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);
                }
                else
                {
                    objPoint3.transform.localPosition = new Vector3(rand.Next(-6, 9), randomTreasure.transform.position.y, randomTreasure.transform.position.z);
                    GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint2.transform);
                    GameObject tresure = Instantiate(randomTreasure, objPoint3.transform);
                }
                counter += 3;
            }
            
        }
        Debug.Log(counter);
        
        //spawn the amount of river segemnts requested
        spawnedSegments.Add(GameObject.Find("RiverStart"));

        PlaceThings(spawnedSegments[0]);
    }

    //choose a location from the list to place the item
    private void PlaceThings(GameObject spawnThis)
    {
        GameObject newSpawn = Instantiate(spawnThis, spawnedSegments[0].transform);
    }
}
