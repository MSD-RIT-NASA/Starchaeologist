using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createSpheres : MonoBehaviour
{
    [SerializeField] private int sphereNum;
    [SerializeField] private GameObject spherePrimitive;
    [SerializeField] public int speedOfRotation;

    // Update is called once per frame
    void Update()
    {
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.transform.localEulerAngles.z + speedOfRotation * Time.deltaTime);
        //this.transform.RotateAround(Vector3.zero, Vector3.forward, speedOfRotation * Time.deltaTime);
    }
}
