using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Raft : MonoBehaviour
{
    public float maxRange = 10f;
    public float tiltRange = 0f;
    public float tiltSpeed = 1.0f;
    public bool tilting = false;
    public Vector3 plannedRotation;

    Quaternion newTilt;
    Quaternion oldTilt;
    float tiltRatio = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        newTilt = Quaternion.Euler(0, 0, 0);
        oldTilt = Quaternion.Euler(0, 0, 0);

        plannedRotation = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //Raft();
    }

    //Raft() simulates the motion of a raft on water by randomly choosing rotations and lerping to those rotations
    public void Raft()
    {
        tiltRatio = Mathf.Clamp(tiltRatio, 0f, 1f);

        //check if the new rotation has been reached
        if (transform.localRotation == newTilt)
        {
            if(tilting)
            {
                //choose a rotation to lerp to
                newTilt = Quaternion.Euler(Random.Range(-tiltRange, tiltRange), 0, Random.Range(-tiltRange, tiltRange));
            }
            else
            {
                //rotate back to zero
                newTilt = Quaternion.Euler(0, 0, 0);
            }

            oldTilt = transform.localRotation;

            tiltRatio = 0.0f;
        }

        //lerp
        //transform.localRotation = Quaternion.Slerp(oldTilt, newTilt, tiltRatio);
        plannedRotation = transform.localEulerAngles;

        //increment time
        tiltRatio = tiltRatio + Time.deltaTime * tiltSpeed;
    }
}
