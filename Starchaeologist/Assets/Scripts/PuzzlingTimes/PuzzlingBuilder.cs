using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlingBuilder : MonoBehaviour
{
    /*
     TO DO:
        -figure out what traps will go with what plates
            -place the traps
     */

    //floor tile variables
    public GameObject tilePrefab;
    List<GameObject>[] tileArray;
    public List<bool>[] trapArray;

    //trap variables
    public GameObject ceilingPrefab;
    public GameObject swingPrefab;
    public GameObject wallPrefab;
    List<GameObject>[] ceilingArray;
    List<GameObject>[] wallArray;
    List<GameObject> swingList;

    public GameObject perimeterPrefab;
    List<GameObject> perimeterList = new List<GameObject>();

    public int roomLength = 4;

    // Start is called before the first frame update
    void DataSetup()
    {
        tileArray = new List<GameObject>[6];
        trapArray = new List<bool>[6];

        ceilingArray = new List<GameObject>[6];
        wallArray = new List<GameObject>[roomLength * 3];
        swingList = new List<GameObject>();
    }

    // Update is called once per frame
    void Start()
    {
        //set up the game script before continuing
        GetComponent<PuzzlingGame>().DataSetup();
        DataSetup();

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
            ceilingArray[i] = new List<GameObject>();
            

            for (int j = 0; j < lengthValue; j++)
            {
                //place the room tiles
                tileArray[i].Add(null);
                tileArray[i][j] = Instantiate(tilePrefab);
                //tileArray[i][j].transform.rotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
                tileArray[i][j].transform.position = new Vector3((i * 2) + 1, 0, (j * 2) + 1);
                trapArray[i].Add(false);

                //place the ceiling traps
                ceilingArray[i].Add(null);
                ceilingArray[i][j] = Instantiate(ceilingPrefab);
                ceilingArray[i][j].transform.position = new Vector3((i * 2) + 1, 0, (j * 2) + 1);

                PlateScript scriptReference = tileArray[i][j].transform.GetChild(0).GetComponent<PlateScript>();
                scriptReference.GetComponent<TeleportationAnchor>().enabled = false;
                scriptReference.myPosition = new Vector2(i, j);

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

                //place the ceiling swings and wall traps
                if (i == 0)
                {
                    //swings
                    swingList.Add(null);
                    swingList[j] = Instantiate(swingPrefab);
                    swingList[j].transform.position = new Vector3(6, 0, (j * 2) + 1);

                    //walls
                    wallArray[j] = new List<GameObject>();
                    if (j % 3 != 1)//avoid the pillar
                    {
                        wallArray[j].Add(null);
                        wallArray[j].Add(null);
                        //0 = left wall
                        wallArray[j][0] = Instantiate(wallPrefab);
                        wallArray[j][0].transform.position = new Vector3(-2, 0, (j * 2) + 1);
                        //1 = right wall
                        wallArray[j][1] = Instantiate(wallPrefab);
                        wallArray[j][1].transform.position = new Vector3(14, 0, (j * 2) + 1);

                    }
                }
            }

            //place trap platforms
            int k = 0;
            while(k < roomLength * 2)
            {
                int kIndex = Random.Range(0, trapArray[i].Count);
                if(trapArray[i][kIndex])
                {
                    continue;
                }
                trapArray[i][kIndex] = true;
                //tileArray[i][kIndex].transform.GetChild(0).gameObject.AddComponent<PlateScript>();
                tileArray[i][kIndex].transform.GetChild(0).gameObject.GetComponent<PlateScript>().trapped = true;

                k++;
            }

            //add to the start and end adjacent platforms
            startAdjacent.Add(new Vector2(i, 0));
            endAdjacent.Add(new Vector2(i, lengthValue - 1));
        }

        //give the adjacent platforms to the start/end platforms
        PlateScript endScript = GameObject.Find("EndPlatform").transform.GetChild(0).GetComponent<PlateScript>();
        endScript.adjacentPlates = startAdjacent;
        endScript.myPosition = endScript.adjacentPlates[0];
        endScript.GetComponent<TeleportationArea>().enabled = false;

        PlateScript startScript = GameObject.Find("StartPlatform").transform.GetChild(0).GetComponent<PlateScript>();
        startScript.adjacentPlates = startAdjacent;
        startScript.myPosition = startScript.adjacentPlates[0];

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


        GetComponent<PuzzlingGame>().ActivatePlates(startScript.adjacentPlates);

        //delete the script at the end
        Destroy(this);
    }
}
