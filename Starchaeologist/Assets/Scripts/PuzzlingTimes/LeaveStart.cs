using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveStart : MonoBehaviour
{
    [SerializeField]
    private UdpSocket server;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerFoot"))
        {
            server.GameStart = true;
            Destroy(this);
        }
    }
}
