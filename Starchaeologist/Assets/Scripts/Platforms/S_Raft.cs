using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Raft : MonoBehaviour
{
    public float tiltRange = 10f;
    public float tiltSpeed = 1.0f;

    Quaternion newTilt;
    Quaternion oldTilt;
    float tiltRatio = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        newTilt = Quaternion.Euler(0, 0, 0);
        oldTilt = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        tiltRatio = Mathf.Clamp(tiltRatio, 0f, 1f);
        Raft();
    }

    //Raft() simulates the motion of a raft on water by randomly choosing rotations and lerping to those rotations
    public void Raft()
    {
        //check if the new rotation has been reached
        if (transform.rotation == newTilt)
        {
            //choose a rotation to lerp to
            newTilt = Quaternion.Euler(Random.Range(-tiltRange, tiltRange), 0, Random.Range(-tiltRange, tiltRange));
            oldTilt = transform.rotation;

            //choose a random speed to lerp at and reset the timer
            //tiltSpeed = Random.Range(0.75f, 2.0f);
            tiltRatio = 0.0f;
        }

        //lerp
        transform.localRotation = Quaternion.Slerp(oldTilt, newTilt, tiltRatio);

        //increment time
        tiltRatio = tiltRatio + Time.deltaTime * tiltSpeed;
    }
}
