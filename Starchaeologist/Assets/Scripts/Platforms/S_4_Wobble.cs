using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_4_Wobble : MonoBehaviour
{
    public float tiltRange = 10f;
    public float tiltSpeed = 1.0f;

    Quaternion newTilt;
    Quaternion oldTilt;
    float tiltRatio = 1.0f;

    float primaryX;
    float primaryZ;
    float secondaryX;
    float secondaryZ;

    public bool isFigure = false;

    bool flip = false;
    int flop = 0;

    // Start is called before the first frame update
    void Awake()
    {
        newTilt = Quaternion.Euler(0, 0, 0);
        oldTilt = Quaternion.Euler(0, 0, 0);
        primaryX = Random.Range(-tiltRange, tiltRange);
        primaryZ = Random.Range(-tiltRange, tiltRange);
        secondaryX = Random.Range(-tiltRange, tiltRange);
        secondaryZ = Random.Range(-tiltRange, tiltRange);
    }

    // Update is called once per frame
    void Update()
    {
        tiltRatio = Mathf.Clamp(tiltRatio, 0f, 1f);
        FourWay();
    }

    //FourWayCircular() simulates a wobbling platform using 2 rotations and their inverses to creat a circular pattern
    public void FourWay()
    {
        //check if the new rotation has been reach
        if (transform.localRotation == newTilt)
        {
            //find the next rotation
            if (flip)
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
        transform.localRotation = Quaternion.Slerp(oldTilt, newTilt, tiltRatio);

        //increment time
        tiltRatio = tiltRatio + Time.deltaTime * tiltSpeed;
    }
}
