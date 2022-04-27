using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*DESCRIPTION
 * 
 * This script is attached to the prefab for the pillar trap.
 * 
 * This script extends the arm from the pillar, then swings 
 * it around to hit the player, then retracts the arm.
 * 
 * Suspense() is used to give the player time to react
 * 
 */


public class Trap_Pillar : MonoBehaviour
{
    float pauseTimer = 0f;
    int bladeStep = 0;
    float lerpRatio = 0f;
    bool trapping = false;
    bool rightSide;
    float swingSpeed = 2f;
    float extendSpeed = 2f;

    //Game Sound Effects
    public AudioSource pillar;

    PlateScript plateReference;

    public void DataSetup(PlateScript getCurrent)
    {
        plateReference = getCurrent;

        bladeStep = 0;
        lerpRatio = 0f;
        trapping = true;
    }

    void Start()
    {
        //Set up Audio Components
        pillar = GetComponent<AudioSource>();

        //decided where the blade will start
        if (Random.Range(0, 2) == 1)//left side of the pillar
        {
            rightSide = true;
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
        }
        else//right side of the pillar
        {
            rightSide = false;
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(trapping)
        {
            SwingBlade();
        }
    }

    void SwingBlade()
    {
        switch (bladeStep)
        {
            case 0://extend blade
                if(lerpRatio == 1f)
                {
                    //set the lerp up for the swivel (it will already be on 1)
                    if(rightSide)
                    {
                        lerpRatio = 0f;
                    }
                    bladeStep++;
                    break;
                }
                ExtendRetract(true);
                break;
            case 1://brief pause
                Suspense(2.5f);
                pillar.Play();
                break;
            case 2://swing blade
                if (rightSide)
                {
                    if (lerpRatio == 1f)
                    {
                        bladeStep++;
                        break;
                    }
                    lerpRatio = lerpRatio + (Time.deltaTime * swingSpeed);
                }
                else
                {
                    if (lerpRatio == 0f)
                    {
                        lerpRatio = 1f;
                        bladeStep++;
                        break;
                    }
                    lerpRatio = lerpRatio - (Time.deltaTime * swingSpeed);
                }
                //clamp
                lerpRatio = Mathf.Clamp(lerpRatio, 0f, 1f);
                //swivel lerp
                transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, lerpRatio * 180f, 0));
                break;
            case 3://brief pause
                Suspense(1.5f);
                pillar.Play();
                break;
            case 4://retract blade
                if (lerpRatio == 0f)
                {
                    bladeStep++;
                    break;
                }
                ExtendRetract(false);
                break;
            case 5://cleanup
                trapping = false;
                plateReference.Reactivate();
                plateReference = null;
                rightSide = !rightSide;
                enabled = false;
                return;
        }
    }

    //function that pauses for suspense
    void Suspense(float pauseFor)
    {
        if (pauseTimer >= pauseFor)
        {
            pauseTimer = 0f;
            bladeStep++;
        }
        else
        {
            pauseTimer = pauseTimer + Time.deltaTime;
        }
    }

    //function used to extend and retract the blade arm
    void ExtendRetract(bool extending)
    {
        //extend
        if(extending)
        {
            lerpRatio = lerpRatio + (Time.deltaTime * extendSpeed);
        }
        else//retract
        {
            lerpRatio = lerpRatio - (Time.deltaTime * extendSpeed);
        }
        //clamp
        lerpRatio = Mathf.Clamp(lerpRatio, 0f, 1f);
        //lerp shoulder
        transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(lerpRatio * 90f, 0, 0));
        //lerp elbow
        transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(lerpRatio * -180f, 0, 0));
    }
}
