using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Ceiling : MonoBehaviour
{
    public GameObject spearReference;
    float spawnHeight = 8f;
    float pokeHeight = 7.5f;
    float stabHeight = 3.5f;

    GameObject spearObject;

    PlateScript plateReference;

    bool trapping = false;
    int trapStep = 0;
    float lerpRatio = 0f;

    bool pauseSpear = false;
    float pauseTimer = 0f;

    Vector3 fromHere;
    Vector3 toThere;

    // set up the data every time the trap is activated
    public void DataSetup(PlateScript getCurrent)
    {
        plateReference = getCurrent;

        spearObject = null;
        spearObject = Instantiate(spearReference, new Vector3(0, spawnHeight, 0), Quaternion.Euler(0, 0, 0), transform);

        fromHere = new Vector3(0, spawnHeight, 0);
        toThere = new Vector3(0, pokeHeight, 0);

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
            case 0:
                Suspense();
                break;
            case 1://spawn to poke
                //check if the location has been reached
                if(lerpRatio == 1f)
                {
                    //play an audio cue
                    lerpRatio = 0f;
                    fromHere = new Vector3(0, pokeHeight, 0);
                    toThere = new Vector3(0, stabHeight, 0);
                    trapStep++;
                    Debug.Log("step: " + trapStep);
                    break;
                }
                lerpRatio = lerpRatio + (Time.deltaTime * 1);
                break;
            case 2:
                Suspense();
                break;
            case 3://poke to stab
                //check if the location has been reached
                if (lerpRatio == 1f)
                {
                    //play an audio cue
                    lerpRatio = 0f;
                    fromHere = new Vector3(0, stabHeight, 0);
                    toThere = new Vector3(0, spawnHeight, 0);
                    trapStep++;
                    Debug.Log("step: " + trapStep);
                    break;
                }
                lerpRatio = lerpRatio + (Time.deltaTime * 4);
                break;
            case 4:
                Suspense();
                break;
            case 5://stab to spawn
                //check if the location has been reached
                if (lerpRatio == 1f)
                {
                    trapStep++;
                    Debug.Log("step: " + trapStep);
                    break;
                }
                lerpRatio = lerpRatio + (Time.deltaTime * 2);
                break;
            case 6://cleanup
                Destroy(spearObject);
                spearObject = null;
                plateReference.reactivate = true;
                plateReference = null;
                trapping = false;
                return;
        }

        //clamp and lerp
        lerpRatio = Mathf.Clamp(lerpRatio, 0f, 1f);
        spearObject.transform.localPosition = Vector3.Lerp(fromHere, toThere, lerpRatio);
    }

    //function that pauses for suspense
    void Suspense()
    {
        if(pauseTimer >= 1.5f)
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
