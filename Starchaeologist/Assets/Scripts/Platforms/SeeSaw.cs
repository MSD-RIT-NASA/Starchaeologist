using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeSaw : MonoBehaviour
{
    Quaternion backTilt;
    Quaternion forthTilt;

    public float tiltRange = 30f;
    public float tiltSpeed = 1.0f;
    public float halfLength = 10f;

    float turnRatio = 0.5f;
    float balanceBuffer;



    void Awake()
    {
        backTilt = Quaternion.Euler(0f, 0f, tiltRange);
        forthTilt = Quaternion.Euler(0f, 0f, -tiltRange);
    }

    private void Update()
    {
        balanceBuffer += Time.deltaTime;
        if(balanceBuffer > 0.5f)
        {
            if (transform.localRotation != Quaternion.Euler(0, 0, 0))
            {
                Balance();
            }
            //else
            //{
            //    turnRatio = 0.5f;
            //}
        }
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            balanceBuffer = 0f;
            turnRatio = Mathf.Clamp(turnRatio, 0f, 1f);
            float distance = Vector3.Distance(transform.position, other.transform.position);
            tiltSpeed = (distance / halfLength) * 0.5f;
            if(transform.InverseTransformPoint(other.transform.position).x > 0 && distance > 1.5f)
            {
                turnRatio = turnRatio + Time.deltaTime * tiltSpeed;
                transform.localRotation = Quaternion.Slerp(backTilt, forthTilt, turnRatio);
            }
            else if (transform.InverseTransformPoint(other.transform.position).x < 0 && distance > 1.5f)
            {
                turnRatio = turnRatio - Time.deltaTime * tiltSpeed;
                transform.localRotation = Quaternion.Slerp(backTilt, forthTilt, turnRatio);
            }
            else if(transform.localRotation != Quaternion.Euler(0,0,0))
            {
                Balance();
            }        
        }
    }
    public void Balance()
    {
        if (turnRatio > 0.51f)
        {
            turnRatio = turnRatio - Time.deltaTime * 0.25f;
            transform.localRotation = Quaternion.Slerp(backTilt, forthTilt, turnRatio);
        }
        else if (turnRatio < 0.49f)
        {
            turnRatio = turnRatio + Time.deltaTime * 0.25f;
            transform.localRotation = Quaternion.Slerp(backTilt, forthTilt, turnRatio);
        }
        else
        {
            turnRatio = 0.5f;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
