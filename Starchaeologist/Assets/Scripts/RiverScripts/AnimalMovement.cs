using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimalMovement : MonoBehaviour
{
    //6.5 -> 1.3
    // Start is called before the first frame update
    private float velocity = 5;
    [SerializeField] Vector3 movementDirection;
    
    void Start()
    {
        System.Random rand = new System.Random();
        velocity = rand.Next(1, 5);
        movementDirection = new Vector3(0,0,-1);
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        Vector3 direction = gameObject.transform.forward;
        transform.position += movementDirection * velocity * Time.deltaTime;
        if (transform.localPosition.z > 46 || transform.localPosition.z < 0)
        {
            Vector3 rotateAround = new Vector3(0,180,0);
            gameObject.transform.Rotate(rotateAround);
            movementDirection *= -1;
        }
    }
}
