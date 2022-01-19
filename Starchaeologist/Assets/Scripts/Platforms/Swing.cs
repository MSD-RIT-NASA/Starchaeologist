using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    Quaternion backSwing;
    Quaternion forthSwing;

    public float tiltRange = 30f;
    public float tiltIncrement = 0.2f;
    public float tiltSpeed = 1.0f;
    public float halfLength = 10f;

    float nextSwing = 0.0f;
    float pushForce = 1.0f;
    bool forth = true;

    float turnRatio = 0.0f;
    float balanceBuffer;

    int fakeInput = 1;
    bool swingReady = true;

    // Start is called before the first frame update
    void Awake()
    {
        backSwing = Quaternion.Euler(0, 0, 0);
        forthSwing = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(swingReady && fakeInput == 1)//replace with player input
            {
                swingReady = false;
                if (backSwing == Quaternion.Euler(0, 0, 0) && forthSwing == Quaternion.Euler(0, 0, 0))
                {
                    turnRatio = 0f;
                    nextSwing += tiltRange * tiltIncrement;
                    pushForce = 2.0f;

                    //set the next destination after a command
                    forthSwing = Quaternion.Euler(nextSwing, 0, 0);
                }
                else if(turnRatio <= 0.5f && nextSwing != tiltRange)//faster
                {
                    float swingBuffer = nextSwing + tiltRange * tiltIncrement;
                    turnRatio = turnRatio * (nextSwing / swingBuffer);
                    nextSwing = swingBuffer;
                    pushForce = 2.0f;

                    //set the next destination after a command
                    forthSwing = Quaternion.Euler(nextSwing, 0, 0);
                }
                else//slower
                {
                    //the player is slowing down

                }      
            }
            else if(swingReady && fakeInput == -1)//replace with player input
            {
                swingReady = false;
                if (backSwing == Quaternion.Euler(0, 0, 0) && forthSwing == Quaternion.Euler(0, 0, 0))
                {
                    turnRatio = 0f;
                    nextSwing += tiltRange * tiltIncrement;
                    pushForce = 2.0f;

                    //set the next destination after a command
                    backSwing = Quaternion.Euler(-nextSwing, 0, 0);
                }
                else if (turnRatio > 0.5f && nextSwing != tiltRange)//faster
                {
                    float swingBuffer = nextSwing + tiltRange * tiltIncrement;
                    turnRatio = turnRatio * (swingBuffer / nextSwing);
                    nextSwing = swingBuffer;
                    pushForce = 2.0f;

                    //set the next destination after a command
                    backSwing = Quaternion.Euler(-nextSwing, 0, 0);
                }
                else//slower
                {
                    //the player is slowing down

                }
            }

            turnRatio = Mathf.Clamp(turnRatio, 0f, 1f);
            nextSwing = Mathf.Clamp(nextSwing, -tiltRange, tiltRange);

            //set the speed based on the current turnRatio
            if (turnRatio <= 0.5)
            {
                tiltSpeed = Mathf.Clamp((turnRatio * pushForce), 0.1f, 1.5f);
            }
            else
            {
                tiltSpeed = Mathf.Clamp(((1f - turnRatio) * pushForce), 0.1f, 1.5f);
            }

            //either add of subtract to turnration based on the direction
            if(forth)
            {
                turnRatio = turnRatio + Time.deltaTime * tiltSpeed;
            }
            else
            {
                turnRatio = turnRatio - Time.deltaTime * tiltSpeed;
            }
            turnRatio = Mathf.Clamp(turnRatio, 0f, 1f);

            

            //if the rotation has been reached
            if((turnRatio == 1.0f && forth) || (turnRatio == 0.0f && !forth))
            {
                fakeInput = -fakeInput;
                forth = !forth;
                swingReady = true;
                pushForce = 1.0f;
                backSwing = Quaternion.Euler(-nextSwing, 0, 0);
                forthSwing = Quaternion.Euler(nextSwing, 0, 0);
            }
        }

        //lerp towards the desired rotation
        transform.localRotation = Quaternion.Slerp(backSwing, forthSwing, turnRatio);

        //if the rotation has been reached
        if ((turnRatio == 1.0f && forth) || (turnRatio == 0.0f && !forth))
        {
            fakeInput = -fakeInput;
            forth = !forth;
            swingReady = true;
            pushForce = 1.0f;
            backSwing = Quaternion.Euler(-nextSwing, 0, 0);
            forthSwing = Quaternion.Euler(nextSwing, 0, 0);
        }
    }
}
