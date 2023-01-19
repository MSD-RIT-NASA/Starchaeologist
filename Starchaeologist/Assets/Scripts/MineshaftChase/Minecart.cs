using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minecart : MonoBehaviour
{
    private const float SPEED = 0;//we dont know what the proper speed should be yet

    private bool turningLeft;
    private bool turningRight;
    private float tiltAngle;

    // Start is called before the first frame update
    void Start()
    {
        tiltAngle = 0f;
        turningLeft = false;
        turningRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FallOnTurn()
    {
        if (turningLeft)
        {
            tiltAngle += 2f;
        }
        else if (turningRight)
        {
            tiltAngle -= 2f;
        }

        this.transform.Rotate(new Vector3(0f, 0f, tiltAngle));
    }

    public void LeanLeft()
    {
        //If headbox is rotated a certain number of degrees in the opposite direction, the cart leans back toward the center
    }

    public void LeanRight()
    {
        //Same as above but the other way
    }
}
