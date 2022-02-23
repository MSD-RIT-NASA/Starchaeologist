using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlingBuilder : MonoBehaviour
{
    public GameObject tilePrefab;
    List<GameObject>[] tileArray;

    int roomLength = 10;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Start()
    {
        //instantiate the tile array
        tileArray = new List<GameObject>[6];
        for (int i = 0; i < tileArray.Length; i++)
        {
            tileArray[i] = new List<GameObject>();
            for (int j = 0; j < roomLength; j++)
            {
                //place the room tiles
                tileArray[i].Add(null);
                tileArray[i][j] = Instantiate(tilePrefab);
                tileArray[i][j].transform.rotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
                tileArray[i][j].transform.position = new Vector3((i * 2) + 1, 0, (j * 2) + 1);
            }
        }
    }
}
