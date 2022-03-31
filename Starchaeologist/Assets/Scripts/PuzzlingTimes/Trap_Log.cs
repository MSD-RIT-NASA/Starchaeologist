using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Log : MonoBehaviour
{
    //rightLerp = 1
    Vector3 rightSwing = new Vector3(0, 0, -4f);
    //leftLerp = 0
    Vector3 leftSwing = new Vector3(0, 0, -176f);

    float lerpRatio = 1f;
    float swingSpeed = 2f;
    bool trapping = false;
    bool rightSide = true;
    int swingStep = 0;

    float pauseTimer = 0f;
    float slowTimer = 0f;

    public AudioSource log;

    PlateScript plateReference;


    // Start is called before the first frame update
    public void DataSetup(PlateScript getCurrent)
    {
        plateReference = getCurrent;

        pauseTimer = 0f;
        trapping = true;
        swingStep = 0;
    }

    //choose a side
    void Start()
    {
        log = GetComponent<AudioSource>();

        if (Random.Range(0, 2) == 1)
        {
            lerpRatio = 1f;
            rightSide = true;
            transform.GetChild(0).transform.rotation = Quaternion.Euler(rightSwing);
        }
        else
        {
            lerpRatio = 0f;
            rightSide = false;
            transform.GetChild(0).transform.rotation = Quaternion.Euler(leftSwing);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(trapping)
        {
            SwingLog();
        }
    }

    void SwingLog()
    {
        switch (swingStep)
        {
            case 0://pause for suspense
                Suspense(2f);
                log.PlayDelayed(.4f);
                break;
            case 1://slow swing
                if (slowTimer > 0.7f)
                {
                    swingStep++;
                    Debug.Log("Step 2");
                    break;
                }
                if(rightSide)
                {
                    lerpRatio = lerpRatio - (Time.deltaTime * 0.01f);
                }
                else
                {
                    lerpRatio = lerpRatio + (Time.deltaTime * 0.01f);
                }
                slowTimer = slowTimer + Time.deltaTime;
                break;
            case 2://fast swing
                if (rightSide)
                {
                    if(lerpRatio <= 0f)
                    {
                        swingStep++;
                        break;
                    }
                    lerpRatio = lerpRatio - (Time.deltaTime * swingSpeed);
                }
                else
                {
                    if (lerpRatio >= 1f)
                    {
                        swingStep++;
                        break;
                    }
                    lerpRatio = lerpRatio + (Time.deltaTime * swingSpeed);
                }
                break;
            case 3://cleanup
                plateReference.Reactivate();
                plateReference = null;
                trapping = false;
                rightSide = !rightSide;
                break;
        }

        //clamp and lerp
        lerpRatio = Mathf.Clamp(lerpRatio, 0f, 1f);
        transform.GetChild(0).transform.rotation = Quaternion.Slerp(Quaternion.Euler(leftSwing), Quaternion.Euler(rightSwing), lerpRatio);
    }

    //function that pauses for suspense
    void Suspense(float pauseFor)
    {
        if (pauseTimer >= pauseFor)
        {
            Debug.Log("Step 1");
            swingStep++;
            slowTimer = 0f;
        }
        else
        {
            pauseTimer = pauseTimer + Time.deltaTime;
        }
    }
}
