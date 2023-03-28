using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndCollision : MonoBehaviour
{
    public GameObject canvasRef;
    public GameObject rightHandRay;
    public GameObject leftHandRay;

    [SerializeField]
    private AudioSource audSrc;
    [SerializeField]
    private GameObject timerCanvas;

    [SerializeField]
    private TMP_Text scoreDisplay;

    static public int Score;

    void OnTriggerEnter(Collider other)
    {
        //when the player first comes into contact with the raft, tell the game to start playing then remove this script to save space
        if (other.gameObject.CompareTag("PlayerFoot") || other.gameObject.CompareTag("Minecart"))
        {
            canvasRef.SetActive(true);
            scoreDisplay.text = "" + Score;
            rightHandRay.SetActive(true);
            leftHandRay.SetActive(true);
            timerCanvas.SetActive(false);
            audSrc.Stop();
            Destroy(this);
        }
    }
}
