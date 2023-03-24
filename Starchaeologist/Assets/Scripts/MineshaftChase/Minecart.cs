//NASA x RIT author: Noah Flanders

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

    //For score display
    private Text txt;
    private Text txtBlip;
    private float showTime = 1f;
    private float hideTime = 0f;
    private float calibrateOn = 30f;
    private float calibrateOff = 0f;
    public static int Score;
    public static bool scoreMenu = false;

    [SerializeField]
    private UdpSocket server;
    private float boardRot;
    private float safeMax;
    private float safeMin;

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
        safeMax = 3f;
        safeMin = -3f;
    }

    // Update is called once per frame
    void Update()
    {
        //Taking rotation data from the server
        boardRot = server.BoardRotation;

        //turningLeft and turningRight should change values as the level progresses
        if (turningLeft)
        {
            FallOnTurn();
        }
        else if (turningRight)
        {
            FallOnTurn();
        }
        else if (!turningLeft && !turningRight)
        {
            isTilting = false;
            FreeLean();
        }
    }

    //If the cart is riding on a curve, it will start to fall towards the outside of the curve.
    //Right now, the cart stops leaning at a certain point, but eventually, the cart should fall or the player take damage
    public void FallOnTurn()
    {
        if (turningRight)
        {
            if (tiltAngle/*boardRot*/ < 40f)
            {
                isTilting = true;
                tiltAngle += .2f;
            }
            LeanRight();
        }
        else if (turningLeft)
        {
            if (tiltAngle/*boardRot*/ > -40f)
            {
                isTilting = true;
                tiltAngle -= .2f;
            }
            LeanLeft();
        }

        //The rotation is applied here
        Vector3 curAngles = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(curAngles.x, curAngles.y, tiltAngle);
    }

    //If headbox(soon to be balance board) is rotated a certain number of degrees in the opposite direction, the cart leans back toward the center
    public void LeanLeft()
    {
        //if(headHb.bounds.Intersects(leftBodyHb.bounds)/* && !(headHb.bounds.Intersects(bodyHb.bounds)) */&& tiltAngle < 0f)
        //{
        //    tiltAngle += .4f;
        //}

        
         if(boardRot > safeMax || boardRot < safeMin){
             tiltAngle += .4f;
             //score += 50;
         }else{
             //score += 5;
         }
        

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
        //if (headHb.bounds.Intersects(rightBodyHb.bounds)/* && !(headHb.bounds.Intersects(bodyHb.bounds))*/ && tiltAngle > 0f)
        //{
        //    tiltAngle -= .4f;
        //}


        if (boardRot > safeMax || boardRot < safeMin)
        {
           tiltAngle -= .4f;
           //score += 50;
        }
        else
        {
           //score += 5;
        }


        //----------------------------------------------------------------------------------------------------------------------------------
        /*
         * Sensor System Pseudocode
         * 
         *if(sensor angle is within the safe zone between certain angles) 
         *     tilt the minecart into the turn
         */
    }

    //Player can lean to either side without falling over while not turning
    //This provides a visual indicator to the player as to how generally well they
    //are balancing on straight tracks
    //Original idea was to have broken rails where the player had to lean to avoid them like obstacles
    public void FreeLean()
    {
        //If player leans far enough one way, they will sit at a 40deg angle in that direction
        if (/*headHb.bounds.Intersects(leftBodyHb.bounds) && !(headHb.bounds.Intersects(bodyHb.bounds))*/boardRot < 40f && tiltAngle < 40f)
        {
            Debug.Log("Free Lean Left");
            tiltAngle += 5f;
        }
        else if (/*headHb.bounds.Intersects(rightBodyHb.bounds) && !(headHb.bounds.Intersects(bodyHb.bounds))*/boardRot > -40f && tiltAngle > -40f)
        {
            Debug.Log("Free Lean Right");
            tiltAngle -= 5f;
        }
        //If they aren't leaning far enough, they will remain at 0 tilt
        else /*if(headHb.bounds.Intersects(bodyHb.bounds))*/
        {
            if (tiltAngle < 0f)
            {
                tiltAngle += 2f;
            }
            else
            {
                tiltAngle -= 2f;
            }

            if (/*tiltAngle*/boardRot < 10f && /*tiltAngle*/boardRot > -10f)
            {
                tiltAngle = 0f;
            }
        }

        //The rotation is applied here
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

    /// <summary>
    /// Each track has differently tagged colliders so that the cart knows when to tilt
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StraightTrack")
        {
            audSrc.Stop();
            audSrc.PlayOneShot(railRide);
            turningLeft = false;
            turningRight = false;
            safeMax = 3f;
            safeMin = -3f;
            //this.transform.eulerAngles = other.transform.eulerAngles;
            // player.transform.eulerAngles = other.transform.eulerAngles;
        }
        else if (other.gameObject.tag == "RightTrack")
        {
            audSrc.PlayOneShot(railGrind);
            turningLeft = false;
            turningRight = true;
            safeMax = -23f;
            safeMin = -17f;
        }
        else if (other.gameObject.tag == "LeftTrack")
        {
            audSrc.PlayOneShot(railGrind);
            turningLeft = true;
            turningRight = false;
            safeMax = 23f;
            safeMin = 17f;
        }
    }
}
