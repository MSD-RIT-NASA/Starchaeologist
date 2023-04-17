using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class S_RiverBuilder : MonoBehaviour
{
    //The river is split into segments each segemnt has 3 empty gameobjects that will hold 
    [SerializeField] GameObject RiverPlaysection;
    [SerializeField] List<GameObject> obstaclePrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> treasurePrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> spawnedSegments = new List<GameObject>();
    private bool itemsAdjusted = false;

    [SerializeField] Camera playerCamera;

    [SerializeField] GameObject playerHitBox;
    static protected List<Vector3> checkpoints = new List<Vector3>();

    [SerializeField] S_RiverGame riverGameScript;
    //using system random system for varited seeds in a loop
    System.Random rand = new System.Random();
    private const int maxObsticaleRotation = 35;
    private const float adjustedObsticleHeight = 0.5f;

    private const int testVar = 0;
    // Start is called before the first frame update
    void Start()
    {


        //SegmentSetup();
        RiverObjBuilder(adjustedObsticleHeight);

        //give the game script the list of river pieces
        GetComponent<S_RiverGame>().riverReferences = spawnedSegments;
        
        // UdpSocket Testing
        //GetComponent<UdpSocket>().test = true; 
        
        GetComponent<UdpSocket>().GameMode = 1;
        GetComponent<UdpSocket>().CalibrateRig = true;

        //remove this script
        //Destroy(this);
    }

    private void Update()
    {
        if (!itemsAdjusted && riverGameScript.timeToMove)
        {
            Debug.Log(playerCamera.transform.position.y);
            RiverObjBuilder(playerCamera.transform.position.y + adjustedObsticleHeight);
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
                GameObject randomObstacle1 = obstaclePrefabs[rand.Next(0,1)];
                GameObject randomObstacle2 = obstaclePrefabs[rand.Next(0, 1)];
                GameObject randomTreasure = treasurePrefabs[rand.Next(0, 8)];

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

                float obj1Rotation;
                float obj2Rotation;

                //treasure can either be in object positions 1,2, or 3 when chosen the other 2 points will be obstcales
                if (randomTreasurePos == 0)
                {
                    //Setting the postion of treasure ob1 in this case and setting the rotations of the 2 other obsticales
                    objPoint1.transform.localPosition = new Vector3(rand.Next(-7, 7 + 1), 0.0f, point1Z);
                    objPoint2.transform.eulerAngles = new Vector3(0.0f,0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation+1));
                    objPoint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));

                    //setting the height of all obsticales to correspond with the players height 
                    objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                    objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight, objPoint2.transform.position.z);
                    objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);

                    //initalizing all treasures 
                    GameObject tresure = Instantiate(randomTreasure,objPoint1.transform);
                    GameObject ob1 = Instantiate(randomObstacle1, objPoint2.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);

                    //MathF.Tan() takes radians so we must convert angle from degrees to radians
                    float rad1 = obj1Rotation * Mathf.PI / 180.0f;

                    /*
                     * For this game we need to have the player duck/displace thier Y position
                     * no matter the rotation the same amount each obstacle to do this we employ
                     * some simple trigonometry to get the X position 
                     */

                    //Tan(theta) = opposite(playerHeight)/adjacent(desiredX or Calculaded Adjacent) --> opposite(playerHeight) * Tan(theta) = DesiredX
                    float calculatedAdjacent1 = (playerHeight) * Mathf.Tan(rad1);

                    float rad2 = obj2Rotation * Mathf.PI / 180.0f;
                    float calculatedAdjacent2 = (playerHeight) * Mathf.Tan(rad2);

                    //treasure is the same but we set the checkpoints Y to zero so we dont go flying into the air
                    Vector3 treasureRiverCheckpoint = new Vector3(tresure.transform.position.x, 0.0f, tresure.transform.position.z);
                    checkpoints.Add(treasureRiverCheckpoint);
                    checkpoints.Add(new Vector3((float)calculatedAdjacent1, 0.0f, ob1.transform.position.z));
                    checkpoints.Add(new Vector3((float)calculatedAdjacent2, 0.0f, ob2.transform.position.z));


                }
                else if (randomTreasurePos == 1)
                {
                    objPoint2.transform.localPosition = new Vector3(rand.Next(-6, 7), 0.0f, point2Z);
                    objPoint1.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                    objPoint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));

                    objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                    objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight - adjustedObsticleHeight, objPoint2.transform.position.z);
                    objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);

                    GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                    GameObject tresure = Instantiate(randomTreasure, objPoint2.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);

                    float rad1 = obj1Rotation * Mathf.PI / 180.0f;
                    float calculatedAdjacent1 = (playerHeight) * Mathf.Tan(rad1);

                    float rad2 = obj2Rotation * Mathf.PI / 180.0f;
                    float calculatedAdjacent2 = (playerHeight) * Mathf.Tan(rad2);

                    checkpoints.Add(new Vector3((float)calculatedAdjacent1, 0.0f, ob1.transform.position.z));

                    Vector3 treasureRiverCheckpoint = new Vector3(tresure.transform.position.x, 0.0f, tresure.transform.position.z);
                    checkpoints.Add(treasureRiverCheckpoint);

                    checkpoints.Add(new Vector3((float)calculatedAdjacent2, 0.0f, ob2.transform.position.z));

                }
                else
                {
                    objPoint3.transform.localPosition = new Vector3(rand.Next(-6, 7), 0.0f, point3Z);
                    objPoint1.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                    objPoint2.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));

                    objPoint1.transform.position = new Vector3(objPoint1.transform.position.x, playerHeight, objPoint1.transform.position.z);
                    objPoint2.transform.position = new Vector3(objPoint2.transform.position.x, playerHeight, objPoint2.transform.position.z);
                    objPoint3.transform.position = new Vector3(objPoint3.transform.position.x, playerHeight, objPoint3.transform.position.z);

                    GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint2.transform);
                    GameObject tresure = Instantiate(randomTreasure, objPoint3.transform);


                    float rad1 = obj1Rotation * Mathf.PI / 180.0f;
                    float calculatedAdjacent1 = (playerHeight) * Mathf.Tan(rad1);

                    float rad2 = obj2Rotation * Mathf.PI / 180.0f;
                    float calculatedAdjacent2 = (playerHeight) * Mathf.Tan(rad2);

                    checkpoints.Add(new Vector3((float)calculatedAdjacent1, 0.0f, ob1.transform.position.z));
                    checkpoints.Add(new Vector3((float)calculatedAdjacent2, 0.0f, ob2.transform.position.z));

                    Vector3 treasureRiverCheckpoint = new Vector3(tresure.transform.position.x, 0.0f, tresure.transform.position.z);
                    checkpoints.Add(treasureRiverCheckpoint);


                }
            }
            
        }
        
        //Adding the end of the river to the checkpoints 
        checkpoints.Add(GameObject.Find("RiverEnd").gameObject.transform.GetChild(1).transform.position);
    }

}
