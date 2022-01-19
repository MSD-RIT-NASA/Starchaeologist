using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_2_Wobble : MonoBehaviour
{

    public float tiltRange = 10f;
    public float tiltSpeed = 1.0f;

    Quaternion forthTilt;
    Quaternion backTilt;
    float tiltRatio = 0.5f;
    bool flip = false;

    // Start is called before the first frame update
    void Awake()
    {
        float wobbleX = Random.Range(-tiltRange, tiltRange);
        float wobbleZ = Random.Range(-tiltRange, tiltRange);
        forthTilt = Quaternion.Euler(wobbleX, 0, wobbleZ);
        backTilt = Quaternion.Euler(-wobbleX, 0, -wobbleZ);
    }

    // Update is called once per frame
    void Update()
    {
        tiltRatio = Mathf.Clamp(tiltRatio, 0f, 1f);
        TwoWayWobble();
    }

    //TwoWayRough has a chosen rotation and lerps between it and its inverse
    public void TwoWayWobble()
    {
        //check if the new rotation has been reach
        if (tiltRatio == 0f || tiltRatio == 1f)
        {
            //invert the rotation and set that as the new goal
            flip = !flip;
        }

        //lerp
        transform.localRotation = Quaternion.Slerp(backTilt, forthTilt, tiltRatio);

        //increment time
        if (flip)
        {
            tiltRatio = tiltRatio + Time.deltaTime * tiltSpeed;
        }
        else
        {
            tiltRatio = tiltRatio - Time.deltaTime * tiltSpeed;
        }
    }
}
