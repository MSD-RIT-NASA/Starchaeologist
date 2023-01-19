using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minecart : MonoBehaviour
{
    private const float SPEED = 0;//we dont know what the proper speed should be yet

    private bool turningLeft;
    private bool turningRight;
    private float tiltAngle;

    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        tiltAngle = 0f;
        turningLeft = false;
        turningRight = true;
        this.transform.forward = mainCam.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //turningLeft and turningRight should change values as the level progresses
        if(turningLeft || turningRight)
        {
            FallOnTurn();
        }
    }

    //If the cart is riding on a curve, it will start to fall towards the outside of the curve.
    //Right now, the cart stops leaning at a certain point, but eventually, the cart should fall or the player take damage
    public void FallOnTurn()
    {
        if (turningLeft)
        {
            if (tiltAngle < 40f)
            {
                tiltAngle += .2f;
            }
            LeanRight();
        }
        else if (turningRight)
        {
            if (tiltAngle > -40f)
            {
                tiltAngle -= .2f;
            }
            LeanLeft();
        }

        this.transform.eulerAngles = new Vector3(0f, 0f, tiltAngle);
    }

    //If headbox is rotated a certain number of degrees in the opposite direction, the cart leans back toward the center
    public void LeanLeft()
    {
        if(mainCam.transform.rotation.z > .45f && tiltAngle < 0)
        {
            tiltAngle += .4f;
        }
    }

    //Same as above but the other way
    public void LeanRight()
    {
        if (mainCam.transform.rotation.z < -.45f && tiltAngle > 0)
        {
            tiltAngle -= .4f;
        }
    }
}
