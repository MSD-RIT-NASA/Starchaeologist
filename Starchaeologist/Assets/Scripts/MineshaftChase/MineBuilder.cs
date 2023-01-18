using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBuilder : MonoBehaviour
{
    public int segmentCount = 10;

    public List<GameObject> obstaclePrefabs = new List<GameObject>();

    List<List<Vector3>> obstacleSpawns; 

    // Start is called before the first frame update
    void Start()
    {
        DataSetup();
        //remove the script 
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DataSetup()
    {
        obstacleSpawns = new List<List<Vector3>>(segmentCount);

    }
}
