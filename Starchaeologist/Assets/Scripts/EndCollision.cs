using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCollision : MonoBehaviour
{
    public GameObject canvasRef;
    public GameObject rightHandRay;
    public GameObject leftHandRay;

    [SerializeField]
    private AudioSource audSrc;
    

    void OnTriggerEnter(Collider other)
    {
        //when the player first comes into contact with the raft, tell the game to start playing then remove this script to save space
        if (other.gameObject.CompareTag("PlayerFoot") || other.gameObject.CompareTag("Minecart"))
        {
            canvasRef.SetActive(true);
            rightHandRay.SetActive(true);
            leftHandRay.SetActive(true);
            audSrc.Stop();
            Destroy(this);
        }
    }
}
