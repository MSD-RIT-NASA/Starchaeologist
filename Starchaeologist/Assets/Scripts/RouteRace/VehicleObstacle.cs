using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleObstacle : MonoBehaviour
{
    [SerializeField] private BoxCollider proximityBehind;
    [SerializeField] private Rigidbody carBody;

    public float speed = 15f;

    private void Update()
    {
        //carBody.AddForce(transform.forward * m_Speed *  50f);
        transform.Translate(transform.forward * speed * Time.deltaTime);//new Vector3(speed, 0, 0) * Time.deltaTime);
    }
}
