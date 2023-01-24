using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minecart : MonoBehaviour
{
    private const float SPEED = 0.05f;//we dont know what the proper speed should be yet

    public bool turningLeft;
    public bool turningRight;
    private float tiltAngle;
    private float counterLean;

    public GameObject player;


    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        tiltAngle = 0f;
        turningLeft = false;
        turningRight = false;
        counterLean = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        //turningLeft and turningRight should change values as the level progresses
        if (turningLeft)
        {
            TurnLeft();
            FallOnTurn();
        }
        else if (turningRight)
        {
            TurnRight();
            FallOnTurn();
        }
        else if (!turningLeft && !turningRight)
        {
            FreeLean();
        }

        Move();
        mainCam.transform.forward = this.transform.forward;
    }

    //The cart should move at a constant speed in the direction of the cart's forward vector
    public void Move()
    {
        this.transform.position += this.transform.forward * SPEED;
        player.transform.position += this.transform.forward * SPEED;
    }

    //If the cart is riding on a curve, it will start to fall towards the outside of the curve.
    //Right now, the cart stops leaning at a certain point, but eventually, the cart should fall or the player take damage
    public void FallOnTurn()
    {
        if (turningRight)
        {
            if (tiltAngle < 40f)
            {
                tiltAngle += .2f;
            }
            LeanRight();
        }
        else if (turningLeft)
        {
            if (tiltAngle > -40f)
            {
                tiltAngle -= .2f;
            }
            LeanLeft();
        }

        Vector3 curAngles = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(curAngles.x, curAngles.y, tiltAngle);
    }

    //If headbox is rotated a certain number of degrees in the opposite direction, the cart leans back toward the center
    public void LeanLeft()
    {
        if(mainCam.transform.rotation.z > .35f && tiltAngle < 0)
        {
            tiltAngle += .4f;
        }
    }

    //Same as above but the other way
    public void LeanRight()
    {
        if (mainCam.transform.rotation.z < -.35f && tiltAngle > 0)
        {
            tiltAngle -= .4f;
        }
    }

    //Player can lean to either side without falling over while not turning
    public void FreeLean()
    {
        if (mainCam.transform.rotation.z > .35f && tiltAngle < 40f)
        {
            tiltAngle += 2f;
        }
        else if (mainCam.transform.rotation.z < -.35f && tiltAngle > -40f)
        {
            tiltAngle -= 2f;
        }
        else if(mainCam.transform.rotation.z > -.35f && mainCam.transform.rotation.z < .35f/* && mainCam.transform.rotation.z < -0.15f && mainCam.transform.rotation.z > 0.15f*/)
        {
            if(mainCam.transform.rotation.z < 0)
            {
                tiltAngle += 2f;
            }
            else
            {
                tiltAngle -= 2f;
            }

            if (tiltAngle < 10f && tiltAngle > -10f)
            {
                tiltAngle = 0;
            }
        }

        Vector3 curAngles = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(curAngles.x, curAngles.y, tiltAngle);
    }


    private void TurnLeft()
    {
        Vector3 curCartAngles = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(curCartAngles.x, curCartAngles.y - .48f, curCartAngles.z);
        Vector3 curCamAngle = player.transform.eulerAngles;
        player.transform.eulerAngles = new Vector3(curCamAngle.x, curCamAngle.y - .48f, curCamAngle.z);
    }


    private void TurnRight()
    {
        Vector3 curCartAngles = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(curCartAngles.x, curCartAngles.y + .48f, curCartAngles.z);
        Vector3 curCamAngle = player.transform.eulerAngles;
        player.transform.eulerAngles = new Vector3(curCamAngle.x, curCamAngle.y + .48f, curCamAngle.z);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StraightTrack")
        {
            turningLeft = false;
            turningRight = false;
            this.transform.eulerAngles = other.transform.eulerAngles;
        }
        else if (other.gameObject.tag == "RightTrack")
        {
            turningLeft = false;
            turningRight = true;
        }
        else if (other.gameObject.tag == "LeftTrack")
        {
            turningLeft = true;
            turningRight = false;
        }
    }
}
