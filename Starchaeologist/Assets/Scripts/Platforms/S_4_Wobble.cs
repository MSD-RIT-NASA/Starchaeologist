using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_4_Wobble : MonoBehaviour
{
    public float tiltRange = 5f;
    public float tiltSpeed = 1.0f;
    public bool back2Zero = false;

    Quaternion newTilt;
    Quaternion oldTilt;
    float tiltRatio = 1.0f;

    float primaryX;
    float primaryZ;
    float secondaryX;
    float secondaryZ;

    bool isFigure = false;
    bool dataReady = false;

    bool flip = false;
    int flop = 0;

    // Start is called before the first frame update
    public void DataSetup(int myType = 1)
    {
        if (myType == 1)
        {
            isFigure = false;
        }
        else
        {
            isFigure = true;
        }
        back2Zero = false;
        newTilt = Quaternion.Euler(0, 0, 0);
        oldTilt = Quaternion.Euler(0, 0, 0);
        primaryX = Random.Range(-tiltRange, tiltRange);
        primaryZ = Random.Range(-tiltRange, tiltRange);
        secondaryX = Random.Range(-tiltRange, tiltRange);
        secondaryZ = Random.Range(-tiltRange, tiltRange);
        dataReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(dataReady)
        {
            tiltRatio = Mathf.Clamp(tiltRatio, 0f, 1f);
            FourWay();
        }
    }

    //FourWayCircular() simulates a wobbling platform using 2 rotations and their inverses to creat a circular pattern
    public void FourWay()
    {
        //check if the new rotation has been reach
        if (transform.localRotation == newTilt)
        {
            //find the next rotation
            if(back2Zero)
            {
                if(transform.localRotation == Quaternion.Euler(0,0,0))
                {
                    //if we're going back to zero and the platform has reached that, disable the script
                    GetComponent<PlateScript>().desiredRotation = new Vector3(0, 0, 0);
                    back2Zero = false;
                    dataReady = false;
                    //Debug.Log("I should be off");
                    enabled = false;
                    return;
                }
                newTilt = Quaternion.Euler(0, 0, 0);
            }
            else if (flip)
            {
                primaryX *= -1f;
                primaryZ *= -1f;
                newTilt = Quaternion.Euler(primaryX, 0, primaryZ);
            }
            else
            {
                secondaryX *= -1f;
                secondaryZ *= -1f;
                newTilt = Quaternion.Euler(secondaryX, 0, secondaryZ);
            }
            oldTilt = transform.localRotation;
            tiltRatio = 0.0f;


            //switch to the next rotation
            if (isFigure)
            {
                flop++;
                if(flop == 2)
                {
                    flop = 0;
                    flip = !flip;
                }
            }
            else
            {
                flip = !flip;
            }
        }

        //lerp
        GetComponent<PlateScript>().desiredRotation = Quaternion.Slerp(oldTilt, newTilt, tiltRatio).eulerAngles;

        //increment time
        tiltRatio = tiltRatio + Time.deltaTime * tiltSpeed;
    }
}
