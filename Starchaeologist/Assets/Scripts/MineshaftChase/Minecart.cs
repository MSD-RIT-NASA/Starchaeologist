using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private UdpSocket server;

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

    //The cart should move at a constant speed in the direction of the cart's forward vector
    //public void Move()
    //{
    //    this.transform.position += this.transform.forward * SPEED;
    //    player.transform.position += this.transform.forward * SPEED;
    //}

    //If the cart is riding on a curve, it will start to fall towards the outside of the curve.
    //Right now, the cart stops leaning at a certain point, but eventually, the cart should fall or the player take damage
    public void FallOnTurn()
    {
        if (turningRight)
        {
            if (tiltAngle/*server.boardRotation*/ < 40f)
            {
                isTilting = true;
                tiltAngle += .2f;
            }
            LeanRight();
        }
        else if (turningLeft)
        {
            if (tiltAngle/*server.boardRotation*/ > -40f)
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
         * if(server.boardRotation > safeMax || server.boardRotation < safeMin){
         *     tiltAngle += .4f;
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
         * if(server.boardRotation > safeMax || server.boardRotation < safeMin){
         *     tiltAngle -= .4f;
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
        if (headHb.bounds.Intersects(leftBodyHb.bounds) && !(headHb.bounds.Intersects(bodyHb.bounds)) && tiltAngle < 40f/*server.boardRotation < 40f*/)
        {
            Debug.Log("Free Lean Left");
            tiltAngle += 5f;
        }
        else if (headHb.bounds.Intersects(rightBodyHb.bounds) && !(headHb.bounds.Intersects(bodyHb.bounds)) && tiltAngle > -40f /*server.boardRotation > -40f*/)
        {
            Debug.Log("Free Lean Right");
            tiltAngle -= 5f;
        }
        else if(headHb.bounds.Intersects(bodyHb.bounds)/*nothing*/)
        {
            if (tiltAngle < 0f)
            {
                tiltAngle += 2f;
            }
            else
            {
                tiltAngle -= 2f;
            }

            if (tiltAngle/*server.boardRotation*/ < 10f && tiltAngle/*server.boardRotation*/ > -10f)
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


    //private void TurnLeft()
    //{
    //    //Vector3 curCartAngles = this.transform.eulerAngles;
    //    //this.transform.eulerAngles = new Vector3(curCartAngles.x, curCartAngles.y - .1f, curCartAngles.z);
    //    //Vector3 curCamAngle = player.transform.eulerAngles;
    //    //player.transform.eulerAngles = new Vector3(curCamAngle.x, curCamAngle.y - .48f, curCamAngle.z);
    //}


    //private void TurnRight()
    //{
    //    //Vector3 curCartAngles = this.transform.eulerAngles;
    //    //this.transform.eulerAngles = new Vector3(curCartAngles.x, curCartAngles.y + .1f, curCartAngles.z);
    //    //Vector3 curCamAngle = player.transform.eulerAngles;
    //    //player.transform.eulerAngles = new Vector3(curCamAngle.x, curCamAngle.y + .48f, curCamAngle.z);
    //}


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StraightTrack")
        {
            turningLeft = false;
            turningRight = false;
            //this.transform.eulerAngles = other.transform.eulerAngles;
            // player.transform.eulerAngles = other.transform.eulerAngles;
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
