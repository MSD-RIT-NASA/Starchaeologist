using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class S_RiverBuilder : MonoBehaviour
{
    //The river is split into segments each segemnt has 3 empty gameobjects that will hold 
    [SerializeField] GameObject RiverPlaysection;
    [SerializeField] List<GameObject> obstaclePrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> treasurePrefabs = new List<GameObject>();
    private bool itemsAdjusted = false;

    [SerializeField] Camera playerCamera;

    [SerializeField] GameObject playerHitBox;
    static protected List<Vector3> checkpoints = new List<Vector3>();

    [SerializeField] S_RiverGame riverGameScript;
    [SerializeField] Text scoreText;
    //using system random system for varited seeds in a loop
    System.Random rand = new System.Random();
    private const int maxObsticaleRotation = 30;
    private const float adjustedObsticleHeight = 0.5f;

    private const int testVar = 0;
    [SerializeField] List<GameObject> segmentPrefabs_2M = new List<GameObject>();
    [SerializeField] List<GameObject> segmentPrefabs_3M = new List<GameObject>();
    [SerializeField] List<GameObject> segmentPrefabs_4M = new List<GameObject>();
    [SerializeField] List<GameObject> segmentPrefabs_5M = new List<GameObject>();
    [SerializeField] List<GameObject> transitionPrefabs_to_2M = new List<GameObject>();
    [SerializeField] List<GameObject> transitionPrefabs_to_3M = new List<GameObject>();
    [SerializeField] List<GameObject> transitionPrefabs_to_4M = new List<GameObject>();
    [SerializeField] List<GameObject> transitionPrefabs_to_5M = new List<GameObject>();
    List<GameObject>[] segmentArray;
    List<GameObject>[] transitionArray;
    public int segmentCount = 10;
    public List<List<Vector3>> obstacleSpawns;
    private List<GameObject> spawnedSegments = new List<GameObject>();

    // Start is called before the first frame update
void Start()
    {
        // UdpSocket Testing
        //GetComponent<UdpSocket>().test = true; 
        
        GetComponent<UdpSocket>().GameMode = 1;
        GetComponent<UdpSocket>().SendData("gameMode 1");
        GetComponent<UdpSocket>().CalibrateRig = true;

        //remove this script
        //Destroy(this);

        scoreText.text = "Score: 0";
        DataSetup();

        //SegmentSetup();
        SegmentSetupTwo();

        //give the game script the list of river pieces
        GetComponent<S_RiverGame>().riverReferences = spawnedSegments;
        //Destroy(this);
    }

private void Update()
    {
        if (!itemsAdjusted && riverGameScript.timeToMove)
        {
            Debug.Log(playerCamera.transform.position.y);
            //RiverObjBuilder(playerCamera.transform.position.y + adjustedObsticleHeight);
            itemsAdjusted = true;
        }
    }

    //will build all obsticales and treasures found throuough the game
private void RiverObjBuilder(float playerHeight)
    {
        for (int x = 0;x < RiverPlaysection.transform.childCount;x++)
        {
            //skip transistion sections 
            if (RiverPlaysection.transform.GetChild(x).childCount > 3)
            {
                //randomization of obsicaleType,placement, and position/rotation
                GameObject randomObstacle1 = obstaclePrefabs[rand.Next(0, obstaclePrefabs.Count)];
                GameObject randomObstacle2 = obstaclePrefabs[rand.Next(0, obstaclePrefabs.Count)];
                GameObject randomTreasure = treasurePrefabs[rand.Next(0, treasurePrefabs.Count)];

                //each segment will spawn obsticales and treasures at differing orientations
                /*
                 * 1. treasure, ob1,ob2
                 *  2. ob1,treasure,ob2
                 *  3. ob1,ob2,treasure
                 */
                int randomTreasurePos = rand.Next(0, 3);

                //variables that make setting local object positions easier
                GameObject objPoint1 = RiverPlaysection.transform.GetChild(x).GetChild(2).gameObject;
                float point1Z = RiverPlaysection.transform.GetChild(x).GetChild(2).gameObject.transform.localPosition.z;

                GameObject objPoint2 = RiverPlaysection.transform.GetChild(x).GetChild(3).gameObject;
                float point2Z = RiverPlaysection.transform.GetChild(x).GetChild(3).gameObject.transform.localPosition.z;

                GameObject objPoint3 = RiverPlaysection.transform.GetChild(x).GetChild(4).gameObject;
                float point3Z = RiverPlaysection.transform.GetChild(x).GetChild(4).gameObject.transform.localPosition.z;

                float obj1Rotation = 0.0f;
                float obj2Rotation = 0.0f;

                //treasure can either be in object positions 1,2, or 3 when chosen the other 2 points will be obstcales
                if (randomTreasurePos == 0)
                {
                    //Setting the postion of treasure ob1 in this case and setting the rotations of the 2 other obsticales
                    objPoint1.transform.localPosition = new Vector3(rand.Next(-7, 7 + 1), 2.0f, point1Z);
                    GameObject tresure = Instantiate(randomTreasure, objPoint1.transform);
                    tresure.GetComponentInChildren<TreasureCollision>().txt = scoreText;
                    Vector3 treasureRiverCheckpoint = new Vector3(tresure.transform.position.x, 0.0f, tresure.transform.position.z);
                    checkpoints.Add(treasureRiverCheckpoint);

                    if (randomObstacle1.name == "Obst_TreeLeftNS" || randomObstacle1.name == "Obst_TreeLeftS")
                    {
                        objPoint2.transform.localPosition = new Vector3(-10,0.0f,point2Z);
                        objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight, objPoint2.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle1, objPoint2.transform);
                        checkpoints.Add(new Vector3(-5.5f, 0.0f, ob1.transform.position.z));
                    }
                    else if (randomObstacle1.name == "Obst_TreeRightNS" || randomObstacle1.name == "Obst_TreeRightS")
                    {
                        objPoint2.transform.localPosition = new Vector3(10, 0.0f, point2Z);
                        objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight, objPoint2.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle1, objPoint2.transform);
                        checkpoints.Add(new Vector3(5.5f, 0.0f, ob1.transform.position.z));
                    }
                    else
                    {
                        objPoint2.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));

                        //setting the height of all obsticales to correspond with the players height 
                        objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight, objPoint2.transform.position.z);

                        //initalizing all treasures 
                        GameObject ob1 = Instantiate(randomObstacle1, objPoint2.transform);

                        //MathF.Tan() takes radians so we must convert angle from degrees to radians
                        float rad1 = obj1Rotation * Mathf.PI / 180.0f;

                        //Tan(theta) = opposite(playerHeight)/adjacent(desiredX or Calculaded Adjacent) --> opposite(playerHeight) * Tan(theta) = DesiredX
                        float calculatedAdjacent1 = (playerHeight) * Mathf.Tan(rad1);

                        //treasure is the same but we set the checkpoints Y to zero so we dont go flying into the air

                        checkpoints.Add(new Vector3((float)calculatedAdjacent1, 0.0f, ob1.transform.position.z));
                    }

                    if (randomObstacle2.name == "Obst_TreeLeftNS" || randomObstacle2.name == "Obst_TreeLeftS")
                    {
                        objPoint3.transform.localPosition = new Vector3(-10, 0.0f, point3Z);
                        objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);
                        GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);
                        checkpoints.Add(new Vector3(-5.5f, 0.0f, ob2.transform.position.z));
                    }
                    else if (randomObstacle2.name == "Obst_TreeRightNS" || randomObstacle2.name == "Obst_TreeRightS")
                    {
                        objPoint3.transform.localPosition = new Vector3(10, 0.0f, point3Z);
                        objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);
                        GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);
                        checkpoints.Add(new Vector3(5.5f, 0.0f, ob2.transform.position.z));
                    }
                    else
                    {
                        objPoint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                        objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);
                        GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);
                        float rad2 = obj2Rotation * Mathf.PI / 180.0f;
                        float calculatedAdjacent2 = (playerHeight) * Mathf.Tan(rad2);
                        checkpoints.Add(new Vector3((float)calculatedAdjacent2, 0.0f, ob2.transform.position.z));
                    }
                    



                }
                else if (randomTreasurePos == 1)
                {
                    if (randomObstacle1.name == "Obst_TreeLeftNS" || randomObstacle1.name == "Obst_TreeLeftS")
                    {
                        objPoint1.transform.localPosition = new Vector3(-10, 2.0f, point1Z);
                        objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle2, objPoint1.transform);
                        checkpoints.Add(new Vector3(-5.5f, 0.0f, ob1.transform.position.z));
                    }
                    else if (randomObstacle1.name == "Obst_TreeRightNS" || randomObstacle1.name == "Obst_TreeRightS")
                    {
                        objPoint1.transform.localPosition = new Vector3(10, 0.0f, point1Z);
                        objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                        checkpoints.Add(new Vector3(5.5f, 0.0f, ob1.transform.position.z));
                    }
                    else
                    {
                        objPoint1.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                        objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                        float rad1 = obj1Rotation * Mathf.PI / 180.0f;
                        float calculatedAdjacent1 = (playerHeight) * Mathf.Tan(rad1);
                        checkpoints.Add(new Vector3((float)calculatedAdjacent1, 0.0f, ob1.transform.position.z));
                    }

                    objPoint2.transform.localPosition = new Vector3(rand.Next(-6, 7), 0.0f, point2Z);
                    GameObject tresure = Instantiate(randomTreasure, objPoint2.transform);

                    Vector3 treasureRiverCheckpoint = new Vector3(tresure.transform.position.x, 0.0f, tresure.transform.position.z);
                    checkpoints.Add(treasureRiverCheckpoint);

                    if (randomObstacle2.name == "Obst_TreeLeftNS" || randomObstacle2.name == "Obst_TreeLeftS")
                    {
                        objPoint3.transform.localPosition = new Vector3(-10, 0.0f, point3Z);
                        objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);
                        GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);
                        checkpoints.Add(new Vector3(-5.5f, 0.0f, ob2.transform.position.z));
                    }
                    else if (randomObstacle2.name == "Obst_TreeRightNS" || randomObstacle2.name == "Obst_TreeRightS")
                    {
                        objPoint3.transform.localPosition = new Vector3(10, 0.0f, point3Z);
                        objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);
                        GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);
                        checkpoints.Add(new Vector3(5.5f, 0.0f, ob2.transform.position.z));
                    }
                    else
                    {
                        objPoint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                        objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);
                        GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);
                        float rad2 = obj2Rotation * Mathf.PI / 180.0f;
                        float calculatedAdjacent2 = (playerHeight) * Mathf.Tan(rad2);
                        checkpoints.Add(new Vector3((float)calculatedAdjacent2, 0.0f, ob2.transform.position.z));
                    }

                }
                else
                {
                    
                    if (randomObstacle1.name == "Obst_TreeLeftNS" || randomObstacle1.name == "Obst_TreeLeftS")
                    {
                        objPoint1.transform.localPosition = new Vector3(-10, 0.0f, point1Z);
                        objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                        checkpoints.Add(new Vector3(-5.5f, 0.0f, ob1.transform.position.z));
                    }
                    else if (randomObstacle1.name == "Obst_TreeRightNS" || randomObstacle1.name == "Obst_TreeRightS")
                    {
                        objPoint1.transform.localPosition = new Vector3(10, 0.0f, point1Z);
                        objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                        checkpoints.Add(new Vector3(5.5f, 0.0f, ob1.transform.position.z));
                    }
                    else
                    {
                        objPoint1.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                        objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                        float rad1 = obj1Rotation * Mathf.PI / 180.0f;
                        float calculatedAdjacent1 = (playerHeight) * Mathf.Tan(rad1);
                        checkpoints.Add(new Vector3((float)calculatedAdjacent1, 0.0f, ob1.transform.position.z));
                    }

                    if (randomObstacle2.name == "Obst_TreeLeftNS" || randomObstacle2.name == "Obst_TreeLeftS")
                    {
                        objPoint2.transform.localPosition = new Vector3(-10, 0.0f, point2Z);
                        objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight, objPoint2.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle2, objPoint2.transform);
                        checkpoints.Add(new Vector3(-5.5f, 0.0f, ob1.transform.position.z));
                    }
                    else if (randomObstacle2.name == "Obst_TreeRightNS" || randomObstacle2.name == "Obst_TreeRightS")
                    {
                        objPoint2.transform.localPosition = new Vector3(10, 0.0f, point2Z);
                        objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight, objPoint2.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle2, objPoint2.transform);
                        checkpoints.Add(new Vector3(5.5f, 0.0f, ob1.transform.position.z));
                    }
                    else
                    {
                        objPoint2.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                        objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight, objPoint2.transform.position.z);
                        GameObject ob1 = Instantiate(randomObstacle2, objPoint2.transform);
                        float rad2 = obj2Rotation * Mathf.PI / 180.0f;
                        float calculatedAdjacent2 = (playerHeight) * Mathf.Tan(rad2);
                        checkpoints.Add(new Vector3((float)calculatedAdjacent2, 0.0f, ob1.transform.position.z));
                    }

                    objPoint3.transform.localPosition = new Vector3(rand.Next(-6, 7), 2.0f, point3Z);
                    GameObject tresure = Instantiate(randomTreasure, objPoint3.transform);
                    tresure.GetComponentInChildren<TreasureCollision>().txt = scoreText;
                    Vector3 treasureRiverCheckpoint = new Vector3(tresure.transform.position.x, 0.0f, tresure.transform.position.z);
                    checkpoints.Add(treasureRiverCheckpoint);


                }
            }
        }
        //Adding the end of the river to the checkpoints 
        checkpoints.Add(GameObject.Find("RiverEnd").gameObject.transform.GetChild(1).transform.position);
        
    }
private void DataSetup()
{
    obstacleSpawns = new List<List<Vector3>>(segmentCount);

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

private void SegmentSetupTwo()
{
    //spawn the amount of river segemnts requested
    spawnedSegments.Add(GameObject.Find("RiverPlayerSpawn"));
    Vector3 spawnPosition = new Vector3(0, 0, -530);
    int i = 0;
    while (i < segmentCount)
    {
        //place the transition piece
        GameObject transitionPiece = Instantiate(transitionArray[0][0]);
        transitionPiece.transform.position = spawnPosition;
        spawnPosition = transitionPiece.transform.GetChild(1).transform.position;

            int segementArrayIndex = Random.Range(0, 3);
        //choose one of the available segment prefabs and place it at the end of the last placed piece
        GameObject newSpawn = Instantiate(segmentArray[segementArrayIndex][Random.Range(0, segmentArray[segementArrayIndex].Count)]);
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
        obstacleSpawns.Add(new List<Vector3>());
        obstacleSpawns[i].Add(newSpawn.transform.position);
        int j = 2;
        while (j < newSpawn.transform.childCount)
        {
            obstacleSpawns[i].Add(newSpawn.transform.GetChild(j).transform.position);
            j++;
        }
        //obstacleSpawns[i].Add(newSpawn.transform.GetChild(1).transform.position);

        i++;
    }

    //place the end of the river at the end of the river
    GameObject endReference = GameObject.Find("RiverEnd");
    endReference.transform.position = spawnPosition;
    endReference.SetActive(false);
    spawnedSegments.Add(endReference);

    //spawn artifact pieces, treasure, and obstacles along the river
    i = 0;
    while (i < segmentCount * 2)
    {

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
    int i = -1;
    while (givePosition == Vector3.zero)
    {
        //choose a river segment to spawn on
        i = Random.Range(0, obstacleSpawns.Count);

        if (obstacleSpawns[i].Count != 0)
        {
            //choose a checkpoint on said river to spawn at
            int j = Random.Range(0, obstacleSpawns[i].Count);
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
    newSpawn.transform.position = givePosition;
    newSpawn.transform.rotation = Quaternion.Euler(0, (Random.Range(0, 2) * 180), 0);
}
}
