using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalEnemy : MonoBehaviour
{
    //Starting variables 
    private GameObject raft;
    private float velocity = 3.0f;
    private bool goingRight = true;
    private bool hasEntered = false;
    private bool chaseEnded = false;
    private bool isAttacking = false;
    private SphereCollider playerInRangeCollider;

    void Start()
    {
        //Getting the raft object sphereCollider for player chase detection
        raft = GameObject.Find("Raft_Fake").transform.GetChild(1).GetChild(0).gameObject;
        playerInRangeCollider = this.GetComponent<SphereCollider>();
    }
   

    // Update is called once per frame
    void Update()
    {
        //States of enemies 
        //If the chase has ended deactivate
        if (chaseEnded)
        {
            Deactivate();
        }
        //if the player has yet to enter the sphere collider and has than call attacking
        else if (hasEntered == false && raft.GetComponent<BoxCollider>().bounds.Intersects(transform.GetComponent<SphereCollider>().bounds) || isAttacking)
        {
            Attack();
        }
        //if there has been no interactinos just partol until triggered
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
            scoreScript.Instance.hitScore();
            Text scoreText = GameObject.Find("ScoreText").GetComponentInChildren<Text>();
            scoreText.text = "Score: " + scoreScript.Score;
        }
        //if the animal was attacking but the player moved out of the way disable the creature
        if (hasEntered && raft.GetComponent<BoxCollider>().bounds.Intersects(transform.GetComponent<SphereCollider>().bounds) == false)
        {
            //if the attacking animal missed the player than state that the chase has ended deactivate in next loop
            chaseEnded = true;
            isAttacking = false;
        }
        else if (raft.GetComponent<BoxCollider>().bounds.Intersects(transform.GetComponent<SphereCollider>().bounds))
        {
            //set has entered equal to tree
            hasEntered = true;
            //find direction of player;
            Vector3 direction = Vector3.Normalize(raft.transform.position - transform.position);
            //move & rotate towards the player
            transform.position += velocity * direction * Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(-direction), Time.deltaTime * 40f);
            //set attacking equal to true
            isAttacking = true;
        }
    }

    //move back and forth between the banks of the river
    void Patrol()
    {
        //patroling loop for attacking animal
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
        //if deactiavted let the developer know and destory this game object
        Destroy(this.gameObject);
        Debug.Log("Player Hit");
    }
}
