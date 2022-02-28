using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlingGame : MonoBehaviour
{

    public List<GameObject>[] tileArray;
    List<Vector2> activePlates = new List<Vector2>();
    Vector2 currentPosition;

    public GameObject startPlatform;
    public GameObject endPlatform;


    // Start is called before the first frame update
    public void DataSetup()
    {
        //make these a direct reference to the floor
        startPlatform = startPlatform.transform.GetChild(0).gameObject;
        endPlatform = endPlatform.transform.GetChild(0).gameObject;

        currentPosition = new Vector2(0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //called by the plate the player lands on to activate teleportation for adjacent plates
    public void ActivatePlates(List<Vector2> getAdjacent)
    {
        for (int i = 0; i < getAdjacent.Count; i++)
        {
            int xIndex = (int)getAdjacent[i].x;
            int yIndex = (int)getAdjacent[i].y;

            if(yIndex < 0)
            {
                startPlatform.GetComponent<TeleportationArea>().enabled = true;
                continue;
            }
            else if(yIndex >= tileArray[0].Count)
            {
                endPlatform.GetComponent<TeleportationArea>().enabled = true;
                continue;
            }
            tileArray[xIndex][yIndex].transform.GetChild(0).GetComponent<TeleportationAnchor>().enabled = true;
        }

        activePlates = getAdjacent;
    }

    //called by the plate the player lands on to deactivate teleportation for adjacent plates
    public void DeactivatePlatforms(Vector2 getCurrent)
    {
        for (int i = 0; i < activePlates.Count; i++)
        {
            int xIndex = (int)activePlates[i].x;
            int yIndex = (int)activePlates[i].y;

            if (yIndex < 0)
            {
                startPlatform.GetComponent<TeleportationArea>().enabled = false;
                continue;
            }
            else if (yIndex >= tileArray[0].Count)
            {
                endPlatform.GetComponent<TeleportationArea>().enabled = false;
                continue;
            }
            tileArray[xIndex][yIndex].transform.GetChild(0).GetComponent<TeleportationAnchor>().enabled = false;
        }

        currentPosition = getCurrent;
    }
}
