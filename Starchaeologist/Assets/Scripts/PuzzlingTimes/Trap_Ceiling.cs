using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*DESCRIPTION
 * 
 * This script is attached to the prefab for the ceiling spike trap.
 * 
 * This script takes from 3 of the empty object children of the prefab,
 * spawns spikes, then stabs them down at the player. The center point
 * is always used, while the other two spikes spawn randomly around the 
 * edges
 * 
 * Suspense() is used to give the player time to react
 * 
 */

public class Trap_Ceiling : MonoBehaviour
{
    public GameObject spearReference;
    float spawnHeight = 7.5f;
    float pokeHeight = 7f;
    float stabHeight = 2.5f;

    GameObject[] spears;
    int[] selectedPorts;

    PlateScript plateReference;

    bool trapping = false;
    int trapStep = 0;
    float lerpRatio = 0f;

    float pauseTimer = 0f;

    Vector3[] fromHere;
    Vector3[] toThere;

    public AudioSource spike;

    void Start()
    {
        spike = GetComponent<AudioSource>();

        enabled = false;
    }

    // set up the data every time the trap is activated
    public void DataSetup(PlateScript getCurrent)
    {
        /*TO DO
         Make this spawn multiple random spikes from 9? optional positions
         */
        plateReference = getCurrent;

        //select which ports to use
        fromHere = new Vector3[3];
        toThere = new Vector3[3];
        spears = new GameObject[3];
        selectedPorts = new int[3];
        int i = 0;
        while(i < 3)
        {
            //the first spike will always be in the center
            int portIndex = 0;
            if(i != 0)
            {
                portIndex = Random.Range(1, 9);

                //make sure the third is not the same as the second
                if (i == 2 && portIndex == selectedPorts[1])
                {
                    continue;
                }
            }

            //if the port selected has not been used
            selectedPorts[i] = portIndex;
            Vector3 spearPosition = new Vector3(transform.GetChild(selectedPorts[i]).transform.position.x, spawnHeight, transform.GetChild(selectedPorts[i]).transform.position.z);
            spears[i] = Instantiate(spearReference, spearPosition, Quaternion.Euler(0, 0, 0));

            fromHere[i] = new Vector3(transform.GetChild(selectedPorts[i]).transform.position.x, spawnHeight, transform.GetChild(selectedPorts[i]).transform.position.z);
            toThere[i] = new Vector3(transform.GetChild(selectedPorts[i]).transform.position.x, pokeHeight, transform.GetChild(selectedPorts[i]).transform.position.z);

            i++;
        }

        trapping = true;
        lerpRatio = 0f;
        trapStep = 0;
        pauseTimer = 0f;
        //play an audio cue
    }

    // Update is called once per frame
    void Update()
    {
        if(trapping)
        {
            SpearStab();
        }
    }

    void SpearStab()
    {
        switch (trapStep)
        {
            case 0://spawn to poke
                //check if the location has been reached
                if(lerpRatio == 1f)
                {
                    //play an audio cue

                    lerpRatio = 0f;
                    for (int i = 0; i < 3; i++)
                    {
                        float getX = transform.GetChild(selectedPorts[i]).transform.position.x;
                        float getZ = transform.GetChild(selectedPorts[i]).transform.position.z;
                        fromHere[i] = new Vector3(getX, pokeHeight, getZ);
                        toThere[i] = new Vector3(getX, stabHeight, getZ);
                    }
                    trapStep++;
                    Debug.Log("step: " + trapStep);
                    break;
                }
                lerpRatio = lerpRatio + (Time.deltaTime * 4);
                break;
            case 1:
                Suspense(4f);
                spike.Play();
                break;
            case 2://poke to stab
                //check if the location has been reached
                if (lerpRatio == 1f)
                {
                    //play an audio cue
                    lerpRatio = 0f;
                    for (int i = 0; i < 3; i++)
                    {
                        float getX = transform.GetChild(selectedPorts[i]).transform.position.x;
                        float getZ = transform.GetChild(selectedPorts[i]).transform.position.z;
                        fromHere[i] = new Vector3(getX, stabHeight, getZ);
                        toThere[i] = new Vector3(getX, spawnHeight, getZ);
                    }
                    trapStep++;
                    Debug.Log("step: " + trapStep);
                    break;
                }
                lerpRatio = lerpRatio + (Time.deltaTime * 6);
                break;
            case 3:
                Suspense(1f);
                break;
            case 4://stab to spawn
                //check if the location has been reached
                if (lerpRatio == 1f)
                {
                    trapStep++;
                    Debug.Log("step: " + trapStep);
                    break;
                }
                lerpRatio = lerpRatio + (Time.deltaTime * 4);
                break;
            case 5://cleanup
                for (int i = 0; i < 3; i++)
                {
                    Destroy(spears[i]);
                    spears[i] = null;
                }
                plateReference.Reactivate();
                plateReference = null;
                trapping = false;
                enabled = false;
                return;
        }

        //clamp and lerp
        lerpRatio = Mathf.Clamp(lerpRatio, 0f, 1f);
        for (int i = 0; i < 3; i++)
        {
            spears[i].transform.localPosition = Vector3.Lerp(fromHere[i], toThere[i], lerpRatio);
        }
    }

    //function that pauses for suspense
    void Suspense(float pauseFor)
    {
        if(pauseTimer >= pauseFor)
        {
            pauseTimer = 0f;
            trapStep++;
            Debug.Log("step: " + trapStep);
        }
        else
        {
            pauseTimer = pauseTimer + Time.deltaTime;
        }
    }
}
