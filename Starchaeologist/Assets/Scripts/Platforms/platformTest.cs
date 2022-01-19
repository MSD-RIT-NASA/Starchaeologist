using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformTest : MonoBehaviour
{
    /* 
     * 
     */

    //Degrees the platform is able to turn
    public float tiltRange = 30f;
    public float tiltSpeed = 1.0f;

    //function bools
    public bool isRaft = false;
    public bool isSmoothRaft = false;
    public bool isTwoRough = false;
    public bool isTwoSmooth = false;
    public bool isFourCircle = false;
    public bool isFourFigure = false;

    //variables for the wobble rotations
    float primaryX;
    float primaryZ;
    float secondaryX;
    float secondaryZ;

    //variables for the 4-way wobbles
    bool flip = false;
    int flop = 0;

    //primary lerp
    Quaternion primaryNewTilt;
    Quaternion primaryOldTilt;
    Quaternion tiltThis;
    float timecount = 0.0f;

    //secondary lerp (used for the 'smooth' functions
    Quaternion secondaryNewTilt;
    Quaternion secondaryOldTilt;
    Quaternion tiltThat;
    float secondSpeed;
    float secondaryTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //set up the necessary variable for whichever function is chosen (could be done with a switch)
        if(isRaft)
        {
            primaryNewTilt = Quaternion.Euler(0, 0, 0);
            primaryOldTilt = Quaternion.Euler(0, 0, 0);
        }
        else if(isSmoothRaft)
        {
            primaryNewTilt = Quaternion.Euler(0, 0, 0);
            primaryOldTilt = Quaternion.Euler(0, 0, 0);
            tiltThis = Quaternion.Euler(0, 0, 0);

            secondaryNewTilt = Quaternion.Euler(0, 0, 0);
            secondaryOldTilt = Quaternion.Euler(0, 0, 0);
            tiltThat = Quaternion.Euler(0, 0, 0);

            secondSpeed = tiltSpeed;
        }
        else if(isTwoRough)
        {
            primaryX = Random.Range(0, (tiltRange * 2)) - tiltRange;
            primaryZ = Random.Range(0, (tiltRange * 2)) - tiltRange;
            primaryNewTilt = Quaternion.Euler(primaryX, 0, primaryZ);
            primaryOldTilt = Quaternion.Euler(-primaryX, 0, -primaryZ);

            tiltSpeed = 0.5f;
        }
        else if (isTwoSmooth)
        {
            primaryNewTilt = Quaternion.Euler(0, 0, 0);
            primaryOldTilt = Quaternion.Euler(0, 0, 0);
            tiltThis = Quaternion.Euler(0, 0, 0);
            primaryX = Random.Range(0, (tiltRange * 2)) - tiltRange;
            primaryZ = Random.Range(0, (tiltRange * 2)) - tiltRange;

            secondaryNewTilt = Quaternion.Euler(0, 0, 0);
            secondaryOldTilt = Quaternion.Euler(0, 0, 0);
            tiltThat = Quaternion.Euler(0, 0, 0);

            tiltSpeed = 0.5f;
            secondSpeed = tiltSpeed;
        }
        else if (isFourCircle)
        {
            primaryNewTilt = Quaternion.Euler(0, 0, 0);
            primaryOldTilt = Quaternion.Euler(0, 0, 0);
            primaryNewTilt = Quaternion.Euler(0, 0, 0);
            primaryOldTilt = Quaternion.Euler(0, 0, 0);
            primaryX = Random.Range(0, (tiltRange * 2)) - tiltRange;
            primaryZ = Random.Range(0, (tiltRange * 2)) - tiltRange;
            secondaryX = Random.Range(0, (tiltRange * 2)) - tiltRange;
            secondaryZ = Random.Range(0, (tiltRange * 2)) - tiltRange;
        }
        else if (isFourFigure)
        {
            primaryNewTilt = Quaternion.Euler(0, 0, 0);
            primaryOldTilt = Quaternion.Euler(0, 0, 0);
            primaryX = Random.Range(0, (tiltRange * 2)) - tiltRange;
            primaryZ = Random.Range(0, (tiltRange * 2)) - tiltRange;
            secondaryX = Random.Range(0, (tiltRange * 2)) - tiltRange;
            secondaryZ = Random.Range(0, (tiltRange * 2)) - tiltRange;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timecount = Mathf.Clamp(timecount, 0f, 1f);
        secondaryTime = Mathf.Clamp(secondaryTime, 0f, 1f);

        //call whichever function is chosen (should probably be done with a switch
        if (isRaft)
        {
            Raft();
        }
        else if(isSmoothRaft)
        {
            SmoothRaft();
        }
        else if (isTwoRough)
        {
            TwoWayRough();
        }
        else if (isTwoSmooth)
        {
            TwoWaySmooth();
        }
        else if (isFourCircle)
        {
            FourWayCircular();
        }
        else if (isFourFigure)
        {
            FourWayFigure();
        }
    }

    //Raft() simulates the motion of a raft on water by randomly choosing rotations and lerping to those rotations
    public void Raft()
    {
        //check if the new rotation has been reached
        if (transform.rotation == primaryNewTilt)
        {
            //choose a rotation to lerp to
            primaryNewTilt = Quaternion.Euler(Random.Range(0, (tiltRange * 2)) - tiltRange, 0, Random.Range(0, (tiltRange * 2)) - tiltRange);
            primaryOldTilt = transform.rotation;

            //choose a random speed to lerp at and reset the timer
            //tiltSpeed = Random.Range(0.75f, 2.0f);
            timecount = 0.0f;
        }

        //lerp
        transform.rotation = Quaternion.Slerp(primaryOldTilt, primaryNewTilt, timecount);

        //increment time
        timecount = timecount + Time.deltaTime * tiltSpeed;
    }

    //This is an experimental version of Raft() which attempts to smooth out the transition to new rotations by continuiing the old rotation
    public void SmoothRaft()
    {
        //check if it's taking too long or has reached the desired rotation
        if (timecount > 0.9f || transform.rotation == primaryNewTilt)
        {
            //Set the secondary data to continue the original path
            secondaryTime = timecount;
            secondaryNewTilt = primaryNewTilt;
            secondaryOldTilt = primaryOldTilt;
            secondSpeed = tiltSpeed;

            //grab the new rotation
            primaryNewTilt = Quaternion.Euler(Random.Range(0, (tiltRange * 2)) - tiltRange, 0, Random.Range(0, (tiltRange * 2)) - tiltRange);
            primaryOldTilt = transform.rotation;

            //choose a random speed to lerp at and reset the timer
            //tiltSpeed = Random.Range(0.75f, 2.0f);
            timecount = 0.0f;
        }

        //lerp the two paths
        tiltThis = Quaternion.Slerp(primaryOldTilt, primaryNewTilt, timecount);
        tiltThat = Quaternion.Slerp(secondaryOldTilt, secondaryNewTilt, secondaryTime);

        //get the average between the two paths
        transform.rotation = Quaternion.Slerp(tiltThat, tiltThis, 0.5f);

        //increment the time
        float keepDelta = Time.deltaTime;
        timecount = timecount + keepDelta * tiltSpeed;
        secondaryTime = secondaryTime + keepDelta * secondSpeed;

        //see if the original has been finished and shift to the new direction
        if (secondaryTime >= 1.0f)
        {
            secondaryOldTilt = secondaryNewTilt;
            secondaryNewTilt = primaryNewTilt;

            secondaryTime = 0.0f;
            secondSpeed = tiltSpeed;
        }
    }

    //TwoWayRough has a chosen rotation and lerps between it and its inverse
    public void TwoWayRough()
    {
        //check if the new rotation has been reach
        if (timecount == 0f || timecount == 1f)
        {
            //invert the rotation and set that as the new goal
            flip = !flip;
        }

        //lerp
        transform.rotation = Quaternion.Slerp(primaryOldTilt, primaryNewTilt, timecount);

        //increment time
        if(flip)
        {
            timecount = timecount + Time.deltaTime * tiltSpeed;
        }
        else
        {
            timecount = timecount - Time.deltaTime * tiltSpeed;
        }
    }

    //and experimental version of TwoWayRough() with attempts to smooth out the ride
    public void TwoWaySmooth()
    {
        //check if it's taking too long or has reached the desired rotation
        if (timecount > 0.9f || transform.rotation == primaryNewTilt)
        {
            //Set the secondary data to continue the original path
            secondaryTime = timecount;
            secondaryNewTilt = primaryNewTilt;
            secondaryOldTilt = primaryOldTilt;
            secondSpeed = tiltSpeed;

            //invert the rotation and set that as the new goal
            primaryX *= -1f;
            primaryZ *= -1f;
            primaryOldTilt = primaryNewTilt;
            timecount = 1f - timecount;
            primaryNewTilt = Quaternion.Euler(primaryX, 0, primaryZ);
        }

        if (secondaryTime >= 1.0f)
        {
            secondaryNewTilt = primaryNewTilt;
            secondaryOldTilt = tiltThat;

            secondaryTime = timecount;
            secondSpeed = tiltSpeed;
        }

        //lerp the two paths
        tiltThis = Quaternion.Slerp(primaryOldTilt, primaryNewTilt, timecount);
        tiltThat = Quaternion.Slerp(secondaryOldTilt, secondaryNewTilt, secondaryTime);

        //transform.rotation = tiltThis;
        transform.rotation = Quaternion.Slerp(tiltThat, tiltThis, 0.5f);

        //increment time
        float keepDelta = Time.deltaTime;
        timecount = timecount + keepDelta * tiltSpeed;
        secondaryTime = secondaryTime + keepDelta * secondSpeed;
    }

    //FourWayCircular() simulates a wobbling platform using 2 rotations and their inverses to creat a circular pattern
    public void FourWayCircular()
    {
        //check if the new rotation has been reach
        if (transform.rotation == primaryNewTilt)
        {
            
            //find the next rotation
            if(flip)
            {
                primaryX *= -1f;
                primaryZ *= -1f;
                primaryNewTilt = Quaternion.Euler(primaryX, 0, primaryZ);
            }
            else
            {
                secondaryX *= -1f;
                secondaryZ *= -1f;
                primaryNewTilt = Quaternion.Euler(secondaryX, 0, secondaryZ);
            }
            primaryOldTilt = transform.rotation;
            timecount = 0.0f;

            //switch to the next rotation
            flip = !flip;
        }

        //lerp
        transform.rotation = Quaternion.Slerp(primaryOldTilt, primaryNewTilt, timecount);

        //increment time
        timecount = timecount + Time.deltaTime * tiltSpeed;
    }

    //FourWayFigure() simulates a wobbling platform using 2 rotations and their inverses to creat a figure-8 pattern
    public void FourWayFigure()
    {
        //check if the new rotation has been reach
        if (transform.rotation == primaryNewTilt)
        {

            //find the next rotaion
            if (flip)
            {
                primaryX *= -1f;
                primaryZ *= -1f;
                primaryNewTilt = Quaternion.Euler(primaryX, 0, primaryZ);
            }
            else
            {
                secondaryX *= -1f;
                secondaryZ *= -1f;
                primaryNewTilt = Quaternion.Euler(secondaryX, 0, secondaryZ);
            }
            primaryOldTilt = transform.rotation;
            flop++;
            timecount = 0.0f;

            //switch to the secondary rotation once the current rotation has gone through it and its increment
            if(flop == 2)
            {
                flop = 0;
                flip = !flip;
            }
        }

        //lerp
        transform.rotation = Quaternion.Slerp(primaryOldTilt, primaryNewTilt, timecount);

        //increment time
        timecount = timecount + Time.deltaTime * tiltSpeed;
    }
}
