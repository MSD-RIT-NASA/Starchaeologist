using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleObstacle : MonoBehaviour
{
    //To handle player collision
    [SerializeField] private BoxCollider proximityBehind;
    [SerializeField] private Rigidbody carBody;

    //Handle movement on road
    public List<Transform> curentLane;
    public int pointIndex;

    //Set the lane that the obstacle is on and its location on the road
    public List<Transform> CurentLane
    {
        get { return curentLane; }
        set { curentLane = value; }
    }

    public int PointIndex
    {
        get { return pointIndex; }
        set { pointIndex = value; }
    }


    public float speed = 15f;

    private void Update()
    {
        
        if(pointIndex < curentLane.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, curentLane[pointIndex].transform.position, speed * Time.deltaTime);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, curentLane[pointIndex].transform.rotation, 5 * Time.deltaTime);

            if (transform.position == curentLane[pointIndex].transform.position)
            {
                pointIndex++;
            }
        }
        else
        {
            transform.Translate(transform.forward * speed * Time.deltaTime);
        }
        
        //carBody.AddForce(transform.forward * m_Speed *  50f);
    }
}
