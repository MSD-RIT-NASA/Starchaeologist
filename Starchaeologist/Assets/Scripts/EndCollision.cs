using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [SerializeField]
    private Text score;

    void OnTriggerEnter(Collider other)
    {
        //when the player first comes into contact with the raft, tell the game to start playing then remove this script to save space
        if (other.gameObject.CompareTag("PlayerFoot") || other.gameObject.CompareTag("Minecart"))
        {
            canvasRef.SetActive(true);
            scoreDisplay.text = "" + score.text;
            rightHandRay.SetActive(true);
            leftHandRay.SetActive(true);
            if (timerCanvas != null)
            {
                timerCanvas.SetActive(false);
                audSrc.Stop();
            }
            Destroy(this);
        }
    }
}
