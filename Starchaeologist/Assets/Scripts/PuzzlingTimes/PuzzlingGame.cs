using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlingGame : MonoBehaviour
{

    /*DESCRIPTION
     * 
     * This script is the main gameplay managing script for Puzzling Times.
     * Methods in this script are mostly called via other objects, since 
     * player advances the gameplay/
     * 
     * When the player teleports to a new tile, PlateScript will call 
     * DeactivatePlates() to keep them from escaping the plate rotation
     * and trap early. Then depending on the trap status of the plate,
     * it will then call ActivatePlates(), enabling telportation to 
     * adjacent platforms. 
     * 
     * TrapTime() is also called by plate scripts and activates one of
     * 4 traps that is relevent to the plate's position.
     * 
     * Communicate constantly sends the rotation of the current plate
     * to PythonCommunicator and give the rotation of the real platform
     * to the current platform
     * 
     */

    //singleton
    public static PuzzlingGame singleton;


    public List<GameObject>[] tileArray;
    public List<GameObject>[] ceilingArray;
    public List<GameObject>[] wallArray;
    public List<GameObject> swingList;
    public List<GameObject>[] pillarArray;
    List<Vector2> activePlates = new List<Vector2>();
    Vector2 currentPosition;
    PlateScript currentScript;
    public bool activateTrap = false;
    public bool trapDone = false;
    bool healing = false;

    public GameObject startPlatform;
    public GameObject endPlatform;

    public AudioSource trap_warning;
    public AudioClip trap_warning1;
    public AudioClip trap_warning2;
    public AudioClip trap_warning3;
    public AudioClip trap_warning4;

    //vignette stuff
    public GameObject vignetteWarning;
    public GameObject vignetteHit;

    PythonCommunicator communicateReference;

    public GameObject UIManager;

    private Vector3 activeTrapPos;

    void Start()
    {
        if(singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }

        trap_warning = GetComponent<AudioSource>();
        communicateReference = GetComponent<PythonCommunicator>();
    }

    // Start is called before the first frame update
    public void DataSetup()
    {
        //make these a direct reference to the floor
        startPlatform = startPlatform.transform.GetChild(0).gameObject;
        endPlatform = endPlatform.transform.GetChild(0).gameObject;

        //the player always starts on the starting platform
        currentPosition = new Vector2(0, -1);
        currentScript = GameObject.Find("StartPlatform").transform.GetChild(0).GetComponent<PlateScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Communication();
    }

    //called by the plate the player lands on to activate teleportation for adjacent plates
    public void ActivatePlates(List<Vector2> getAdjacent)
    {
        UIManager.GetComponent<Trap_Indicator>().SetTrapActive(false);
        //activate plates based on the given list
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
            tileArray[xIndex][yIndex].transform.GetChild(0).GetChild(0).GetComponent<TeleportationAnchor>().enabled = true;
        }

        //save the list of activated plates
        activePlates = getAdjacent;
        healing = false;
    }

    //called by the plate the player lands on to deactivate teleportation for adjacent plates
    public void DeactivatePlatforms(Vector2 getCurrent)
    {
        //deactivate the currently activated plates
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
            tileArray[xIndex][yIndex].transform.GetChild(0).GetChild(0).GetComponent<TeleportationAnchor>().enabled = false;
        }

        //set the global variables to the new position
        currentPosition = getCurrent;
        if((int)getCurrent.y < 0)
        {
            currentScript = startPlatform.GetComponent<PlateScript>();
        }
        else if((int)getCurrent.y >= tileArray[0].Count)
        {
            currentScript = endPlatform.GetComponent<PlateScript>();
        }
        else
        {
            currentScript = tileArray[(int)getCurrent.x][(int)getCurrent.y].transform.GetChild(0).GetChild(0).GetComponent<PlateScript>();
        }
    }

    //python communication function
    void Communication()
    {
        float desiredX = currentScript.desiredRotation.eulerAngles.x;
        float desiredZ = currentScript.desiredRotation.eulerAngles.z;

        //keep everything uniform to hopefully avoid errors
        //keep rotation values within the range of (-10) to (10)
        if(desiredX > 180)
        {
            desiredX -= 360f;
        }
        if (desiredZ > 180)
        {
            desiredZ -= 360f;
        }

        if (currentScript.trapped)
        {
            Vector2 giveRotation = new Vector2(desiredX, desiredZ);
            communicateReference.desiredRotation = giveRotation;
            currentScript.transform.parent.transform.localRotation = Quaternion.Euler(communicateReference.realRotation.x, -45, communicateReference.realRotation.y);
        }
        else
        {
            communicateReference.desiredRotation = new Vector2(0, 0);
        }
    }

    //called from the current platform to set off the trap
    public void TrapTime()
    {
        Debug.Log("Trap Time");
        int xIndex = (int)currentPosition.x;
        int yIndex = (int)currentPosition.y;
        int thisTrap = currentScript.trapList[Random.Range(0,currentScript.trapList.Count)];
        //trap_warning.Play();
        //Turn on warning vingette
        vignetteOn();
        Invoke("vignetteOff", 3.0f); //set inactive after 3 seconds have passed

        

        //set up the trap list
        //0 = ceiling spikes
        //1 = arrows
        //2 = log swing          
        //3 = pillar swipe
        switch (thisTrap)
        {
            case 0:
                trap_warning.PlayOneShot(trap_warning1);
                Debug.Log("Ceiling Spikes!");
                ceilingArray[xIndex][yIndex].GetComponent<Trap_Ceiling>().enabled = true;
                ceilingArray[xIndex][yIndex].GetComponent<Trap_Ceiling>().DataSetup(currentScript);
                activeTrapPos = ceilingArray[xIndex][yIndex].GetComponent<Trap_Ceiling>().transform.position;
                activeTrapPos.y += 1000;
                break;
            case 1:
                trap_warning.PlayOneShot(trap_warning2);
                Debug.Log("Arrows!");
                int thisSide = Random.Range(0, 2);
                wallArray[yIndex][thisSide].GetComponent<Trap_Arrow>().enabled = true;
                wallArray[yIndex][thisSide].GetComponent<Trap_Arrow>().DataSetup(currentScript);
                activeTrapPos = wallArray[yIndex][thisSide].GetComponent<Trap_Arrow>().transform.position;
                break;
            case 2:
                trap_warning.PlayOneShot(trap_warning3);
                Debug.Log("Log Swing!");
                swingList[yIndex].GetComponent<Trap_Log>().enabled = true;
                swingList[yIndex].GetComponent<Trap_Log>().DataSetup(currentScript);
                activeTrapPos = swingList[yIndex].GetComponent<Trap_Log>().transform.position;
                break;
            case 3:
                trap_warning.PlayOneShot(trap_warning4);
                Debug.Log("Pillar Swipe!");
                //figure out which pillar to use
                int pillarSide = 0;
                if(xIndex > 3)
                {
                    pillarSide = 1;
                }
                int pillarDepth = yIndex / 3;
                pillarArray[pillarDepth][pillarSide].GetComponent<Trap_Pillar>().enabled = true;
                pillarArray[pillarDepth][pillarSide].GetComponent<Trap_Pillar>().DataSetup(currentScript);
                activeTrapPos = pillarArray[pillarDepth][pillarSide].GetComponent<Trap_Pillar>().transform.position;
                break;
            default:
                break;
        }
        Debug.Log("Got to the Trap_Indicaor Setters");
        UIManager.GetComponent<Trap_Indicator>().SetTrapActive(true);
        UIManager.GetComponent<Trap_Indicator>().SetTarget(activeTrapPos);
    }

    //a method called by obstacles when the player hits them which will increment points
    public void TrapHit()
    {
        //healing should stop multiple objects from causing damage from the same trap
        if(!healing)
        {
            vignetteOnHit();
            Invoke("vignetteOffHit", 5.0f); //set inactive after 5 seconds have passed
            healing = true;
            Debug.Log("The player hit me!");
        }
    }

    void vignetteOn()
    {
        vignetteWarning.SetActive(true);
    }

    void vignetteOff()
    {
        vignetteWarning.SetActive(false);
    }

    
    void vignetteOnHit()
    {
        vignetteWarning.SetActive(true);
    }

    void vignetteOffHit()
    {
        vignetteWarning.SetActive(false);
    }
}
