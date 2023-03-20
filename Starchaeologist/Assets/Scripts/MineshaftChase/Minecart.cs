using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minecart : MonoBehaviour
{
    private const float SPEED = 0.05f;//we dont know what the proper speed should be yet

    public bool turningLeft;
    public bool turningRight;
    public bool isTilting;
    private float tiltAngle;

    public GameObject player;

    public Camera mainCam;

    public Collider headHb;
    public Collider bodyHb;
    public Collider leftBodyHb;
    public Collider rightBodyHb;

    [SerializeField]
    private AudioSource audSrc;
    [SerializeField]
    private AudioClip railGrind;
    [SerializeField]
    private AudioClip railRide;

    private Text txt;
    private Text txtBlip;
    private float showTime = 1f;
    private float hideTime = 0f;
    private float calibrateOn = 30f;
    private float calibrateOff = 0f;
    public static int Score;
    public static bool scoreMenu = false;

    public float TiltAngle
    {
        get { return tiltAngle; }
    }

    // Start is called before the first frame update
    void Start()
    {
        tiltAngle = 0f;
        turningLeft = false;
        turningRight = false;
        isTilting = false;
    }

    // Update is called once per frame
    void Update()
    {
        //turningLeft and turningRight should change values as the level progresses
        if (turningLeft)
        {
            //TurnLeft();
            FallOnTurn();
        }
        else if (turningRight)
        {
            //TurnRight();
            FallOnTurn();
        }
        else if (!turningLeft && !turningRight)
        {
            isTilting = false;
            FreeLean();
        }

        // Move();
        //mainCam.transform.forward = this.transform.forward;
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
                isTilting = true;
                tiltAngle += .2f;
            }
            LeanRight();
        }
        else if (turningLeft)
        {
            if (tiltAngle > -40f)
            {
                isTilting = true;
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
        if(headHb.bounds.Intersects(leftBodyHb.bounds)/* && !(headHb.bounds.Intersects(bodyHb.bounds)) */&& tiltAngle < 0f)
        {
            tiltAngle += .4f;
        }

        /*
         * if(tiltAngle > safeMax || tiltAngle < safeMin){
         *     score += 50;
         * }else{
         *     score += 5;
         * }
        */

        //----------------------------------------------------------------------------------------------------------------------------------
        /*
         * Sensor System Pseudocode
         * 
         *if(sensor angle is within the safe zone between certain angles) 
         *     tilt the minecart into the turn
         */
    }

    //Same as above but the other way
    public void LeanRight()
    {
        if (headHb.bounds.Intersects(rightBodyHb.bounds)/* && !(headHb.bounds.Intersects(bodyHb.bounds))*/ && tiltAngle > 0f)
        {
            tiltAngle -= .4f;
        }

        /*
         * if(tiltAngle > safeMax || tiltAngle < safeMin){
         *     score += 50;
         * }else{
         *     score += 5;
         * }
        */

        //----------------------------------------------------------------------------------------------------------------------------------
        /*
         * Sensor System Pseudocode
         * 
         *if(sensor angle is within the safe zone between certain angles) 
         *     tilt the minecart into the turn
         */
    }

    //Player can lean to either side without falling over while not turning
    public void FreeLean()
    {
        if (headHb.bounds.Intersects(leftBodyHb.bounds) && !(headHb.bounds.Intersects(bodyHb.bounds)) && tiltAngle < 40f)
        {
            Debug.Log("Free Lean Left");
            tiltAngle += 5f;
        }
        else if (headHb.bounds.Intersects(rightBodyHb.bounds) && !(headHb.bounds.Intersects(bodyHb.bounds)) && tiltAngle > -40f)
        {
            Debug.Log("Free Lean Right");
            tiltAngle -= 5f;
        }
        else if(headHb.bounds.Intersects(bodyHb.bounds))
        {
            if (tiltAngle < 0f)
            {
                tiltAngle += 2f;
            }
            else
            {
                tiltAngle -= 2f;
            }

            if (tiltAngle < 10f && tiltAngle > -10f)
            {
                tiltAngle = 0f;
            }
        }

        Vector3 curAngles = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(curAngles.x, curAngles.y, tiltAngle);

        //----------------------------------------------------------------------------------------------------------------------------------
        /*
         * Sensor System Pseudocode
         * 
         * float sensorAngle = data from board sensors
         * angle of minecart = sensorAngle
         * if(angle of minecart is between -10 and 10 degrees)
         *     don't rotate minecart
         * 
        */
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StraightTrack")
        {
            audSrc.Stop();
            audSrc.PlayOneShot(railRide);
            turningLeft = false;
            turningRight = false;
            //this.transform.eulerAngles = other.transform.eulerAngles;
            // player.transform.eulerAngles = other.transform.eulerAngles;
        }
        else if (other.gameObject.tag == "RightTrack")
        {
            audSrc.PlayOneShot(railGrind);
            turningLeft = false;
            turningRight = true;
        }
        else if (other.gameObject.tag == "LeftTrack")
        {
            audSrc.PlayOneShot(railGrind);
            turningLeft = true;
            turningRight = false;
        }
    }
}
