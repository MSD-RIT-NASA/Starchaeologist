using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_2_Wobble : MonoBehaviour
{

    public float tiltRange = 5f;
    public float tiltSpeed = 1.0f;

    Quaternion forthTilt;
    Quaternion backTilt;
    float tiltRatio = 0.5f;
    bool flip = false;
    public bool back2Zero = true;
    bool dataReady = false;

    // Start is called before the first frame update
    public void DataSetup()
    {
        back2Zero = false;
        float wobbleX = Random.Range(-tiltRange, tiltRange);
        float wobbleZ = Random.Range(-tiltRange, tiltRange);
        forthTilt = Quaternion.Euler(wobbleX, 0, wobbleZ);
        backTilt = Quaternion.Euler(-wobbleX, 0, -wobbleZ);
        dataReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (dataReady)
        {
            tiltRatio = Mathf.Clamp(tiltRatio, 0f, 1f);
            TwoWayWobble();
        }
    }

    //TwoWayRough has a chosen rotation and lerps between it and its inverse
    void TwoWayWobble()
    {
        //check if the desired rotation has been reach
        if(back2Zero)
        {
            if(Mathf.Abs(Quaternion.Angle(transform.localRotation, Quaternion.Euler(0,0,0))) < 1f)
            {
                //if we're going back to zero and the platform has reached that, disable the script
                GetComponent<PlateScript>().desiredRotation = Quaternion.Euler(0, 0, 0);
                back2Zero = false;
                dataReady = false;
                enabled = false;
                //Debug.Log("I should be off");
                return;
            }
            else if(tiltRatio > 0.5f)//otherwise move toward zero (0.5 technically)
            {
                flip = false;
            }
            else
            {
                flip = true;
            }
        }
        else if((flip && transform.localRotation == forthTilt) || (!flip && transform.localRotation == backTilt))
        {
            //invert the rotation and set that as the new goal
            flip = !flip;
        }

        //increment time
        if (flip)
        {
            tiltRatio = tiltRatio + Time.deltaTime * tiltSpeed;
        }
        else
        {
            tiltRatio = tiltRatio - Time.deltaTime * tiltSpeed;
        }

        //lerp
        GetComponent<PlateScript>().desiredRotation = Quaternion.Slerp(backTilt, forthTilt, tiltRatio);
    }
}
