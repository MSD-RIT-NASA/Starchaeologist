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
    static protected List<Vector3> checkpoints = new List<Vector3>();
    //using system random system for varited seeds in a loop
    System.Random rand = new System.Random();
    private const int maxObsticaleRotation = 50;
    private const int maxCheckpointPosX = 7;
    // Start is called before the first frame update
    void Start()
    {


        //SegmentSetup();
        RiverObjBuilder();

        //give the game script the list of river pieces
        GetComponent<PythonCommunicator>().gameMode = 1;

        //remove this script
        Destroy(this);
    }

    //will build all obsticales and treasures found throuough the game
    private void RiverObjBuilder()
    {
        for (int x = 0;x < RiverPlaysection.transform.childCount;x++)
        {
            //skip transistion sections 
            if (RiverPlaysection.transform.GetChild(x).childCount > 3)
            {
                //randomization of obsicaleType,placement, and position/rotation
                //only the rotation of the attack animals are affected
                GameObject randomObstacle1 = obstaclePrefabs[rand.Next(0,1)];
                GameObject randomObstacle2 = obstaclePrefabs[rand.Next(0, 1)];
                GameObject randomTreasure = treasurePrefabs[rand.Next(0, 8)];
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
                    /*
                     * Why local space? 
                     * This object will be the children within each object point
                     * its easier to set the same local space over and over than place
                     * and adjust in world space
                     */

                    objPoint1.transform.localPosition = new Vector3(rand.Next(-maxCheckpointPosX, maxCheckpointPosX + 1), 0, point1Z);
                    objPoint2.transform.eulerAngles = new Vector3(0.0f,0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation+1));
                    objPoint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));



                    GameObject tresure = Instantiate(randomTreasure,objPoint1.transform);
                    GameObject ob1 = Instantiate(randomObstacle1, objPoint2.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);

                    //object point 1 is the first one encountered by the player so as checkpoints go they are handled in order
                    //player will be moved towards treasures while they are moved away from obsticales
                    tresure.transform.position.Set(tresure.transform.position.x + 0.5f, tresure.transform.position.y, tresure.transform.position.z);
                    checkpoints.Add(tresure.transform.position);


                    if (obj1Rotation < -maxObsticaleRotation/2)//in excess of -35 degrees we want to move the player closer to the obstcale so it is still a threat
                    {
                        Vector3 tempObsticale1 = new Vector3(obj1Rotation/maxObsticaleRotation * -maxCheckpointPosX - 7, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation < 0)//between 0 and -35 degrees, position should increase to the edges 
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation) * maxCheckpointPosX, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation > maxObsticaleRotation / 2)
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation) * -maxCheckpointPosX + 7, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation >= 0)
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation ) * maxCheckpointPosX, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }

                    //
                    if (obj2Rotation < -maxObsticaleRotation / 2)//in excess of -35 degrees we want to move the player closer to the obstcale so it is still a threat
                    {
                        Vector3 tempObsticale2 = new Vector3(obj2Rotation / maxObsticaleRotation * -maxCheckpointPosX - 7, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation < 0)//between 0 and -35 degrees, position should increase to the edges 
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation ) * maxCheckpointPosX, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation > maxObsticaleRotation / 2)
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation) * -maxCheckpointPosX + 7, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation >= 0)
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation ) * maxCheckpointPosX, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }


                }
                else if (randomTreasurePos == 1)
                {
                    objPoint2.transform.localPosition = new Vector3(rand.Next(-6, 7), 0, point2Z);
                    objPoint1.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                    objPoint3.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));

                    GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                    GameObject tresure = Instantiate(randomTreasure, objPoint2.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint3.transform);


                    if (obj1Rotation < -maxObsticaleRotation / 2)//in excess of -35 degrees we want to move the player closer to the obstcale so it is still a threat
                    {
                        Vector3 tempObsticale1 = new Vector3(obj1Rotation / maxObsticaleRotation * -maxCheckpointPosX - 7, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation < 0)//between 0 and -35 degrees, position should increase to the edges 
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation ) * maxCheckpointPosX, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation > maxObsticaleRotation / 2)
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation) * -maxCheckpointPosX + 7, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation >= 0)
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation ) * maxCheckpointPosX, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }

                    tresure.transform.position.Set(tresure.transform.position.x + 0.5f, tresure.transform.position.y, tresure.transform.position.z);
                    checkpoints.Add(tresure.transform.position);

                    if (obj2Rotation < -maxObsticaleRotation / 2)//in excess of -35 degrees we want to move the player closer to the obstcale so it is still a threat
                    {
                        Vector3 tempObsticale2 = new Vector3(obj2Rotation / maxObsticaleRotation * -maxCheckpointPosX - 7, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation < 0)//between 0 and -35 degrees, position should increase to the edges 
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation) * maxCheckpointPosX, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation > maxObsticaleRotation / 2)
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation) * -maxCheckpointPosX + 7, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation >= 0)
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation) * maxCheckpointPosX, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }

                }
                else
                {
                    objPoint3.transform.localPosition = new Vector3(rand.Next(-6, 7), 0, point3Z);
                    objPoint1.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj1Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));
                    objPoint2.transform.eulerAngles = new Vector3(0.0f, 0.0f, obj2Rotation = rand.Next(-maxObsticaleRotation, maxObsticaleRotation + 1));

                    GameObject ob1 = Instantiate(randomObstacle1, objPoint1.transform);
                    GameObject ob2 = Instantiate(randomObstacle2, objPoint2.transform);
                    GameObject tresure = Instantiate(randomTreasure, objPoint3.transform);


                    if (obj1Rotation < -maxObsticaleRotation / 2)//in excess of -35 degrees we want to move the player closer to the obstcale so it is still a threat
                    {
                        Vector3 tempObsticale1 = new Vector3(obj1Rotation / maxObsticaleRotation * -maxCheckpointPosX - 7, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation < 0)//between 0 and -35 degrees, position should increase to the edges 
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation) * maxCheckpointPosX, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation > maxObsticaleRotation / 2)
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation) * -maxCheckpointPosX + 7, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }
                    else if (obj1Rotation >= 0)
                    {
                        Vector3 tempObsticale1 = new Vector3((obj1Rotation / maxObsticaleRotation ) * maxCheckpointPosX, 0.0f, ob1.transform.position.z);
                        checkpoints.Add(tempObsticale1);
                    }

                    //
                    if (obj2Rotation < -maxObsticaleRotation / 2)//in excess of -35 degrees we want to move the player closer to the obstcale so it is still a threat
                    {
                        Vector3 tempObsticale2 = new Vector3(obj2Rotation / maxObsticaleRotation * -maxCheckpointPosX - 7, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation < 0)//between 0 and -35 degrees, position should increase to the edges 
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation) * maxCheckpointPosX, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation > maxObsticaleRotation / 2)
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation) * -maxCheckpointPosX + 7, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }
                    else if (obj2Rotation >= 0)
                    {
                        Vector3 tempObsticale2 = new Vector3((obj2Rotation / maxObsticaleRotation ) * maxCheckpointPosX, 0.0f, ob2.transform.position.z);
                        checkpoints.Add(tempObsticale2);
                    }

                    tresure.transform.position.Set(tresure.transform.position.x + 0.5f, tresure.transform.position.y, tresure.transform.position.z);
                    checkpoints.Add(tresure.transform.position);


                }
            }
            
        }
        

        checkpoints.Add(GameObject.Find("RiverEnd").gameObject.transform.GetChild(1).transform.position);
        Debug.Log(checkpoints);

        foreach (Vector3 checkpoint in checkpoints)
        {
            Debug.Log(checkpoint);
        }
    }

}
