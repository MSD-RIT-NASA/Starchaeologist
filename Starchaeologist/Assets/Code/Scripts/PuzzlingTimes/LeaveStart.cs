using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveStart : MonoBehaviour
{
    [SerializeField]
    private UdpSocket server;

    private void Start()
    {
        server = FindObjectOfType<PuzzlingGame>().GetComponent<UdpSocket>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerFoot"))
        {
            Debug.Log("Player is on the first platform");
            server.GameStart = true;
            Destroy(this.gameObject);
        }
    }
}
