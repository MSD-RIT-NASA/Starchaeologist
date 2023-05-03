using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    [SerializeField] GameObject roadPiece;
    [SerializeField] GameObject currentRoad;
    [SerializeField] GameObject finishLine;
    [SerializeField] List<GameObject> roadSegments;
    [SerializeField] List<GameObject> vehicleObstacles;
    [SerializeField] List<GameObject> availableObstacles;

    //Each lane on the road and center
    [SerializeField] List<Transform> lane1;
    [SerializeField] List<Transform> lane2;
    [SerializeField] List<Transform> lane3;

    private int lane1Index;
    private int lane2Index;
    private int lane3Index;

    public int distance = 13;


    // Start is called before the first frame update
    void Start()
    {
        lane1Index = 0;
        lane2Index = 0;
        lane3Index = 0;
    }

    public void BuildRoad()
    {
        //Build a road as long as the distance
        currentRoad = GameObject.Find("Road Segments");
        Vector3 spawnPosition = new Vector3(0, 0, 150);

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
            PlaceObstacles(nextPiece);

            spawnPosition = nextPiece.transform.Find("Endpoint").transform.position;

            i++;
        }

        finishLine.transform.position = spawnPosition;
        roadSegments.Add(finishLine);
    }

    private void PlaceObstacles(GameObject placedPiece)
    {
        GameObject currentObstacle = GameObject.Find("Obstacles");

        //Get lane position for obstacles to follow
        lane1.Add(placedPiece.transform.Find("Lane1Point"));
        lane2.Add(placedPiece.transform.Find("Lane2Point"));
        lane3.Add(placedPiece.transform.Find("Lane3Point"));

        lane1Index++;
        lane2Index++;
        lane3Index++;

        //Spawn obstacles on road piece
        List<GameObject> possibleSpawns = new List<GameObject>();
        for (int j = 0; j < 8; j++)
        {
            possibleSpawns.Add(placedPiece.transform.GetChild(j + 2).gameObject);
        }

        //Pick an obstacle and place it on a specific spot on the road
        int obst = Random.Range(0, availableObstacles.Count);
        int roadSpot = Random.Range(0, possibleSpawns.Count);
        GameObject newObstacle = Instantiate(availableObstacles[obst], possibleSpawns[roadSpot].transform);
        vehicleObstacles.Add(newObstacle);
        
        //If the placed obstacle is a vehicle, have it follow the road
        if(newObstacle.name == "TestObstacleVehicle(Clone)")
        {
            VehicleObstacle temp = newObstacle.GetComponent<VehicleObstacle>();
            if(roadSpot < 3)
            {
                temp.CurentLane = lane3;
                temp.PointIndex = lane3Index;
            }
            else if (roadSpot > 7)
            {
                temp.CurentLane = lane1;
                temp.PointIndex = lane1Index;
            }
            else
            {
                temp.CurentLane = lane2;
                temp.PointIndex = lane2Index;
            }
        }


        newObstacle.transform.parent = currentObstacle.transform;
    }
}
