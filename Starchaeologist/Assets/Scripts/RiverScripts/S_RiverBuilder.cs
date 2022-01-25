using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RiverBuilder : MonoBehaviour
{

    public int riverSegments = 10;
    Vector3 spawnPosition = new Vector3(0,0,0);
    GameObject newSegment;
    public List<GameObject> segmentPrefabs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //spawn the amount of river segemnts requested
        while (riverSegments > 0)
        {
            newSegment = Instantiate(segmentPrefabs[Random.Range(0, segmentPrefabs.Count)]);
            newSegment.transform.position = spawnPosition;
            spawnPosition = newSegment.transform.GetChild(1).transform.position;

            riverSegments--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
