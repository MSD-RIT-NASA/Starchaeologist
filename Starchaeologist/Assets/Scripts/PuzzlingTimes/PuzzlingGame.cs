using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlingGame : MonoBehaviour
{
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

    PythonCommunicator communicateReference;

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
        /*To DO
        -This script is the link between the python script and the plate scripts
        -get the desired rotation from the plate script
        -send it to the python script
        -grab the rotation of the real platform
        -set the platform's rotation as such
        -for now just send the desired rotation right into the platform's localRotation
         */

        //if (currentScript.trapped)
        //{
        //    currentScript.transform.localRotation = Quaternion.Euler(currentScript.desiredRotation);
        //}

        //currentScript.transform.localRotation = currentScript.desiredRotation;
        //if(currentScript.trapped)
        //{
        //    Debug.Log(currentScript.desiredRotation.eulerAngles);
        //}

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
            //currentScript.transform.parent.transform.localRotation = currentScript.desiredRotation;
        }
        else
        {
            communicateReference.desiredRotation = new Vector2(0, 0);
        }
    }

    //called from the current platform to set off the trap
    public void TrapTime()
    {
        /*TO DO
         -This function will go through the process of setting off the trap once the
            current plate tells it to
        -set off the corresponding trap
        -activate the adjacent platforms
         */
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
                break;
            case 1:
                trap_warning.PlayOneShot(trap_warning2);
                Debug.Log("Arrows!");
                int thisSide = Random.Range(0, 2);
                wallArray[yIndex][thisSide].GetComponent<Trap_Arrow>().enabled = true;
                wallArray[yIndex][thisSide].GetComponent<Trap_Arrow>().DataSetup(currentScript);
                break;
            case 2:
                trap_warning.PlayOneShot(trap_warning3);
                Debug.Log("Log Swing!");
                swingList[yIndex].GetComponent<Trap_Log>().enabled = true;
                swingList[yIndex].GetComponent<Trap_Log>().DataSetup(currentScript);
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
                break;
            default:
                break;
        }
    }

    //a method called by obstacles when the player hits them which will increment points
    public void TrapHit()
    {
        //healing should stop multiple objects from causing damage from the same trap
        if(!healing)
        {
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
}
