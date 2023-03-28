//NASA x RIT author: Noah Flanders

//This script displays the rotation of the balance board the player
//is standing on and the "ideal zone" in which the balance board should
//be within

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceUI : MonoBehaviour
{
    [SerializeField]
    private Minecart cart;
    [SerializeField]
    private Camera main;

    [SerializeField]
    private Image balanceBar;
    [SerializeField]
    private Image safeZone;

    private float balanceRot;
    private float headsetOffset;

    [SerializeField]
    private UdpSocket server;

    // Start is called before the first frame update
    void Start()
    {
        headsetOffset = 0f;
        balanceRot = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        headsetOffset = main.transform.eulerAngles.z;
        balanceRot = server.BoardRotation;
        balanceBar.transform.eulerAngles = new Vector3(
            balanceBar.transform.eulerAngles.x,
            balanceBar.transform.eulerAngles.y,
            balanceRot + headsetOffset
            );
    }

    //The ideal zone changes depending on if the player is turning or not
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StraightTrack")
        {
            safeZone.transform.eulerAngles = new Vector3(
                safeZone.transform.eulerAngles.x,
                safeZone.transform.eulerAngles.y,
                0f);
        }
        else if (other.gameObject.tag == "RightTrack")
        {
            safeZone.transform.eulerAngles = new Vector3(
                safeZone.transform.eulerAngles.x,
                safeZone.transform.eulerAngles.y,
                -20f);
        }
        else if (other.gameObject.tag == "LeftTrack")
        {
            safeZone.transform.eulerAngles = new Vector3(
                safeZone.transform.eulerAngles.x,
                safeZone.transform.eulerAngles.y,
                20f);
        }
    }
}