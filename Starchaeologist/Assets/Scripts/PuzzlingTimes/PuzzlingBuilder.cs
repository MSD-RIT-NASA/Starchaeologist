using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlingBuilder : MonoBehaviour
{
    /*
     TO DO:
        -figure out what traps will go with what plates
     */

    public GameObject tilePrefab;
    List<GameObject>[] tileArray;
    public List<bool>[] trapArray;

    public GameObject perimeterPrefab;
    List<GameObject> perimeterList = new List<GameObject>();

    public int roomLength = 4;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Start()
    {
        //instantiate the tile array
        tileArray = new List<GameObject>[6];
        trapArray = new List<bool>[6];
        for (int i = 0; i < tileArray.Length; i++)
        {
            tileArray[i] = new List<GameObject>();
            trapArray[i] = new List<bool>();
            for (int j = 0; j < (roomLength * 3); j++)
            {
                //place the room tiles
                tileArray[i].Add(null);
                tileArray[i][j] = Instantiate(tilePrefab);
                tileArray[i][j].transform.rotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
                tileArray[i][j].transform.position = new Vector3((i * 2) + 1, 0, (j * 2) + 1);
                trapArray[i].Add(false);
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
                tileArray[i][kIndex].transform.GetChild(0).gameObject.AddComponent<PlateScript>();
                PlateScript scriptReference = tileArray[i][kIndex].transform.GetChild(0).gameObject.GetComponent<PlateScript>();
                scriptReference.xIndex = i;
                scriptReference.zIndex = kIndex;

                k++;
            }
        }

        //place the outer walls of the room
        for (int i = 0; i < roomLength; i++)
        {
            perimeterList.Add(null);
            perimeterList[i] = Instantiate(perimeterPrefab);
            int zPosition = (((i * 3) + 1) * 2) + 1;
            perimeterList[i].transform.position = new Vector3(6, 0, zPosition);
        }

        //move the last platform to the end of the room
        GameObject.Find("EndPlatform").transform.position = new Vector3(6, 0, roomLength * 6);
    }
}
