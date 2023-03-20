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

    public int distance = 13;


    // Start is called before the first frame update
    void Start()
    {
        BuildRoad();
    }

    private void BuildRoad()
    {
        //Build a road as long as the distance
        currentRoad = GameObject.Find("Road Segments");
        Vector3 spawnPosition = new Vector3(0, 0, 75);

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

            spawnPosition = nextPiece.transform.GetChild(1).transform.position;

            i++;
        }

        finishLine.transform.position = spawnPosition;
        roadSegments.Add(finishLine);
    }

    private void PlaceObstacles(GameObject placedPiece)
    {
        GameObject currentObstacle = GameObject.Find("Obstacles");

        /*
        int i = 0;
        while(i < distance)
        {
            //Choose a random segment to add obstacles to
            int segmentNumber = Random.Range(0, roadSegments.Count/2);
            GameObject segment = roadSegments[segmentNumber];

            //Only spawn on the road built
            if (segment.name == "Ground(Clone)")
            {
                //Get a possible spawn point
                List<GameObject> possibleSpawns = new List<GameObject>();
                for (int j = 0; j < 5; j++)
                {
                    possibleSpawns.Add(segment.transform.GetChild(j + 2).gameObject);
                }

                GameObject newObstacle = Instantiate(currentObstacle, possibleSpawns[Random.Range(0, possibleSpawns.Count)].transform);
                vehicleObstacles.Add(newObstacle);
                //newObstacle.transform.parent = currentObstacle.transform;
            }
            i++;
        }
        */

        //Spawn obstacles on road piece
        List<GameObject> possibleSpawns = new List<GameObject>();
        for (int j = 0; j < 5; j++)
        {
            possibleSpawns.Add(placedPiece.transform.GetChild(j + 2).gameObject);
        }

        //Pick an obstacle and place it
        int obst = Random.Range(0, availableObstacles.Count);
        GameObject newObstacle = Instantiate(availableObstacles[obst], possibleSpawns[Random.Range(0, possibleSpawns.Count)].transform);
        vehicleObstacles.Add(newObstacle);
        newObstacle.transform.parent = currentObstacle.transform;
    }
}
