using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlingBuilder : MonoBehaviour
{
    /*DESCRIPTION
     * 
     * The script is attached to the game manager and serves to build 
     * the level upon entry.
     * 
     * The script takes in several prefabs which it uses to build the 
     * temple room. It takes in the floor tile prefab, as well as the 
     * four traps, and the interior walls. It builds the room in tile
     * sets of 3 based on the roomLength variable. This can be set to
     * lengthen or shorten the room via the Unity editor.
     * 
     * When each platform is placed, random tiles will be 'trapped', 
     * causing them to set off on of the traps when the palyer comes
     * in contact with them
     * 
     * Once everything is set the room will place each tile and trap. 
     * Each tile will be given the nearby tiles and applicable traps
     * for to store and use in-game.
     * 
     * This script will send the necessary data to PuzzlingGame then 
     * delete itself once it has finished.
     * 
     */

    //treasure variables
    public List<GameObject> treasurePrefabs;
    public List<bool>[] treasureArray;

    //floor tile variables
    public GameObject tilePrefab;
    List<GameObject>[] tileArray;
    public List<bool>[] trapArray;

    //trap variables
    public GameObject ceilingPrefab;
    public GameObject swingPrefab;
    public GameObject wallPrefab;
    public GameObject pillarPrefab;
    List<GameObject>[] ceilingArray;
    List<GameObject>[] wallArray;
    List<GameObject> swingList;
    List<GameObject>[] pillarArray;


    public GameObject perimeterPrefab;
    List<GameObject> perimeterList = new List<GameObject>();

    public int roomLength = 4;

    // Start is called before the first frame update
    void DataSetup()
    {
        tileArray = new List<GameObject>[6];
        trapArray = new List<bool>[6];
        treasureArray = new List<bool>[6];

        ceilingArray = new List<GameObject>[6];
        wallArray = new List<GameObject>[roomLength * 3];
        swingList = new List<GameObject>();
        pillarArray = new List<GameObject>[roomLength];
    }

    // Update is called once per frame
    void Start()
    {
        //set up the game script before continuing
        //PuzzlingGame.singleton.DataSetup();
        GetComponent<PuzzlingGame>().DataSetup();
        DataSetup();
        GetComponent<UdpSocket>().gameMode = 2;
        GetComponent<UdpSocket>().calibrateRig = true; 

        int lengthValue = (roomLength * 3);

        //start and end adjacent tiles (includes themselves)
        List<Vector2> startAdjacent = new List<Vector2>();
        startAdjacent.Add(new Vector2(0, -1));
        List<Vector2> endAdjacent = new List<Vector2>();
        endAdjacent.Add(new Vector2(0, lengthValue));

        for (int i = 0; i < tileArray.Length; i++)
        {

            tileArray[i] = new List<GameObject>();
            trapArray[i] = new List<bool>();
            treasureArray[i] = new List<bool>();
            ceilingArray[i] = new List<GameObject>();
            int pillarIndex = 0;


            for (int j = 0; j < lengthValue; j++)
            {
                //place the room tiles
                tileArray[i].Add(Instantiate(tilePrefab));
                //tileArray[i][j].transform.rotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
                tileArray[i][j].transform.position = new Vector3((i * 2) + 1, 0, (j * 2) + 1);
                trapArray[i].Add(false);
                treasureArray[i].Add(false);

                //place the ceiling traps
                ceilingArray[i].Add(Instantiate(ceilingPrefab));
                ceilingArray[i][j].transform.position = new Vector3((i * 2) + 1, 0, (j * 2) + 1);

                PlateScript scriptReference = tileArray[i][j].transform.GetChild(0).GetChild(0).GetComponent<PlateScript>();
                scriptReference.GetComponent<TeleportationAnchor>().enabled = false;
                scriptReference.DataSetup(new Vector2(i, j));

                //create the list of plates adjacent to the created plate
                List<Vector2> giveAdjacent = new List<Vector2>();
                //middle
                giveAdjacent.Add(new Vector2(i, j + 1));
                giveAdjacent.Add(new Vector2(i, j - 1));
                //left
                if(i != 0)
                {
                    giveAdjacent.Add(new Vector2(i - 1, j));
                    //avoid duplicates of the start/end platform
                    if (j != 0)
                    {
                        giveAdjacent.Add(new Vector2(i - 1, j - 1));
                    }
                    if(j != lengthValue - 1)
                    {
                        giveAdjacent.Add(new Vector2(i - 1, j + 1));
                    }
                }
                //Right
                if (i != (tileArray.Length - 1))
                {
                    giveAdjacent.Add(new Vector2(i + 1, j));
                    //avoid duplicates of the start/end platform
                    if (j != 0)
                    {
                        giveAdjacent.Add(new Vector2(i + 1, j - 1));
                    }
                    if (j != lengthValue - 1)
                    {
                        giveAdjacent.Add(new Vector2(i + 1, j + 1));
                    }
                }
                scriptReference.adjacentPlates = giveAdjacent;

                //place the ceiling swings, wall traps, and pillar traps
                if (i == 0)
                {
                    //swings
                    swingList.Add(Instantiate(swingPrefab));
                    swingList[j].transform.position = new Vector3(6, 5.5f, (j * 2) + 1);

                    //walls
                    wallArray[j] = new List<GameObject>();
                    if (j % 3 != 1)//avoid the pillar
                    {
                        wallArray[j].Add(Instantiate(wallPrefab));
                        wallArray[j].Add(Instantiate(wallPrefab));
                        //0 = left wall
                        wallArray[j][0].transform.position = new Vector3(-2, 0, (j * 2) + 1);
                        wallArray[j][0].GetComponent<Trap_Arrow>().rightSide = false;
                        //1 = right wall
                        wallArray[j][1].transform.position = new Vector3(14, 0, (j * 2) + 1);
                        wallArray[j][1].GetComponent<Trap_Arrow>().rightSide = true;
                    }
                    else//pillars
                    {
                        /*Pillar building here*/
                        pillarArray[pillarIndex] = new List<GameObject>();
                        Vector3 pillarPosition;
                        Vector3 pillarRotation;
                        //0 = left pillar
                        pillarPosition = new Vector3(-1, 1.5f, (j * 2) + 1);
                        pillarRotation = new Vector3(0, 0, 0);
                        pillarArray[pillarIndex].Add(Instantiate(pillarPrefab, pillarPosition, Quaternion.Euler(pillarRotation)));
                        //1 = right pillar
                        pillarPosition = new Vector3(13, 1.5f, (j * 2) + 1);
                        pillarRotation = new Vector3(0, 180, 0);
                        pillarArray[pillarIndex].Add(Instantiate(pillarPrefab, pillarPosition, Quaternion.Euler(pillarRotation)));
                        pillarIndex++;
                    }
                }
            }

            //set trap platforms and place treasure
            int k = 0;
            while(k < roomLength * 2)
            {
                bool placing = true;
                //traps
                while(placing)
                {
                    int yIndex = Random.Range(0, trapArray[i].Count);
                    if (trapArray[i][yIndex])
                    {
                        continue;
                    }
                    trapArray[i][yIndex] = true;
                    //tileArray[i][kIndex].transform.GetChild(0).gameObject.AddComponent<PlateScript>();
                    tileArray[i][yIndex].transform.GetChild(0).GetChild(0).gameObject.GetComponent<PlateScript>().trapped = true;
                    placing = false;
                }

                //treasure
                if (k < roomLength)
                {
                    placing = true;
                    while (placing)
                    {
                        int yIndex = Random.Range(0, trapArray[i].Count);
                        if (treasureArray[i][yIndex])
                        {
                            continue;
                        }
                        treasureArray[i][yIndex] = true;
                        GameObject placeTreasure = Instantiate(treasurePrefabs[Random.Range(0, treasurePrefabs.Count)]);
                        Vector3 anchorPosition = tileArray[i][yIndex].transform.GetChild(1).transform.position;
                        placeTreasure.transform.position = new Vector3(anchorPosition.x + Random.Range(-0.5f, 0.5f), anchorPosition.y, anchorPosition.z + Random.Range(0f, 0.5f));
                        placeTreasure.transform.localRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                        placing = false;
                    }
                }
                


                k++;
            }


            //add to the start and end adjacent platforms
            startAdjacent.Add(new Vector2(i, 0));
            endAdjacent.Add(new Vector2(i, lengthValue - 1));
        }

        //give the adjacent platforms to the start/end platforms
        PlateScript endScript = GameObject.Find("EndPlatform").transform.GetChild(0).GetComponent<PlateScript>();
        endScript.adjacentPlates = endAdjacent;
        endScript.DataSetup(endAdjacent[0]);
        endScript.GetComponent<TeleportationArea>().enabled = false;

        PlateScript startScript = GameObject.Find("StartPlatform").transform.GetChild(0).GetComponent<PlateScript>();
        startScript.adjacentPlates = startAdjacent;
        startScript.DataSetup(startAdjacent[0]);

        //move the last platform to the end of the room
        GameObject.Find("EndPlatform").transform.position = new Vector3(6, 0, roomLength * 6);

        //place the outer walls of the room
        for (int i = 0; i < roomLength; i++)
        {
            perimeterList.Add(null);
            perimeterList[i] = Instantiate(perimeterPrefab);
            int zPosition = (((i * 3) + 1) * 2) + 1;
            perimeterList[i].transform.position = new Vector3(6, 0, zPosition);
        }

        //send the data over to the game script
        GetComponent<PuzzlingGame>().tileArray = tileArray;
        GetComponent<PuzzlingGame>().ceilingArray = ceilingArray;
        GetComponent<PuzzlingGame>().wallArray = wallArray;
        GetComponent<PuzzlingGame>().swingList = swingList;
        GetComponent<PuzzlingGame>().pillarArray = pillarArray;


        GetComponent<PuzzlingGame>().ActivatePlates(startScript.adjacentPlates);

        //delete the script at the end
        Destroy(this);
    }
}
