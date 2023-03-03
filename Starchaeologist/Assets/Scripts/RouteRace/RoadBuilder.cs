using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    [SerializeField] GameObject roadPiece;
    [SerializeField] GameObject currentRoad;
    [SerializeField] List<GameObject> roadSegments;
    [SerializeField] List<GameObject> vehicleObstacles;

    public List<GameObject>[] roadObjects;
    List<List<Vector3>> obstacleSpawns;

    public int distance = 10;


    // Start is called before the first frame update
    void Start()
    {
        BuildRoad();

        PlaceObstacles();

        BuildFinishLine();
    }

    private void BuildRoad()
    {
        //Build a road as long as 
        currentRoad = GameObject.Find("Road Segments");
        Vector3 spawnPosition = new Vector3(0, 0, 375);

        //Take the starting road segment and add them to the list of existing pieces 
        foreach (Transform roadP in currentRoad.transform)
        {
            roadSegments.Add(roadP.gameObject);
        }

        //Place pieces of the road
        int i = 0;
        while (i < distance)
        {
            GameObject nextPiece = Instantiate(roadPiece);
            roadSegments.Add(nextPiece);
            nextPiece.transform.parent = currentRoad.transform;
            nextPiece.transform.position = spawnPosition;

            spawnPosition = nextPiece.transform.GetChild(1).transform.position;
            i++;
        }
    }

    private void PlaceObstacles()
    {
        
    }

    private void BuildFinishLine()
    {

    }
}
