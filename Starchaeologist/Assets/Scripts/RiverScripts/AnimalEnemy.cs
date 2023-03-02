using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject raft;
    private float velocity = 3.0f;
    private bool goingRight = true;
    private bool hasEntered = false;
    private bool chaseEnded = false;
    private bool isAttacking = false;
    private SphereCollider playerInRangeCollider;
    void Start()
    {
        raft = GameObject.Find("Raft_Fake").transform.GetChild(1).GetChild(0).gameObject;
        playerInRangeCollider = this.GetComponent<SphereCollider>();
    }
   

    // Update is called once per frame
    void Update()
    {
        if (chaseEnded)
        {
            Deactivate();
        }
        else if (hasEntered == false && raft.GetComponent<BoxCollider>().bounds.Intersects(transform.GetComponent<SphereCollider>().bounds) || isAttacking)
        {
            Attack();
        }
        else
        {
            Patrol();
        }
    }

    //when player in range chase player
    void Attack()
    {
        Debug.Log("Attacking");
        //if the player is caught reduce points by 10 and deactivate 
        if (raft.GetComponent<BoxCollider>().bounds.Intersects(transform.GetComponent<BoxCollider>().bounds))
        {
            chaseEnded = true;
            isAttacking = false;
        }
        //if the animal was attacking but the player moved out of the way disable the creature
        if (hasEntered && raft.GetComponent<BoxCollider>().bounds.Intersects(transform.GetComponent<SphereCollider>().bounds) == false)
        {
            //good place of a courutine;
            chaseEnded = true;
            isAttacking = false;
        }
        else if (raft.GetComponent<BoxCollider>().bounds.Intersects(transform.GetComponent<SphereCollider>().bounds))
        {
            hasEntered = true;
            //find direction of player;
            Vector3 direction = Vector3.Normalize(raft.transform.position - transform.position);
            //move towards the player
            transform.position += velocity * direction * Time.deltaTime;
            isAttacking = true;
        }
    }

    //move back and forth between the banks of the river
    void Patrol()
    {
        if (goingRight)
        {
            Vector3 direction = new Vector3(-1.0f, 0, 0);
            transform.position += direction * velocity * Time.deltaTime;
            if (transform.position.x <= -7.2f)
            {
                goingRight = !goingRight;
            }
        }
        else
        {
            Vector3 direction = new Vector3(1.0f, 0, 0);
            transform.position += direction * velocity * Time.deltaTime;
            if (transform.position.x >= 7.2f)
            {
                goingRight = !goingRight;
            }
        }
    }

    //when player out of range after attack sink and deactivate
    void Deactivate()
    {
        Destroy(this.gameObject);
        Debug.Log("Player Hit");
    }
}
