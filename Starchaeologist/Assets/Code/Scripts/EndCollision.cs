using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class EndCollision : MonoBehaviour
{
    public GameObject canvasRef;
    public GameObject rightHandRay;
    public GameObject leftHandRay;
    [SerializeField]
    private UdpSocket server;
    [SerializeField] private GameObject XR_Rig;
    [SerializeField] private Vector3 LockPos;

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
            server.GameStart = false;
            server.GameOver = true;
            canvasRef.SetActive(true);
            scoreDisplay.text = score.text.Split(' ')[1];
            if (timerCanvas != null && audSrc != null)
            {
                timerCanvas.SetActive(false);
                audSrc.Stop();
            }

            if(XR_Rig!= null)
            {
                XR_Rig.GetComponent<TeleportationProvider>().enabled = false;
                XR_Rig.transform.position = LockPos;
                rightHandRay.SetActive(false);
                leftHandRay.SetActive(false);
            }
            Destroy(this);
        }
    }
}
