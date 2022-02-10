using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipGrabTrigger : MonoBehaviour
{
    [SerializeField] private Transform grabPullDestination;

    private void Start()
    {
        if (!grabPullDestination)
        {
            Debug.LogError($"A destination for things grabbed by the whip was not supplied to {gameObject.name}!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WhipGrabbable"))
        {
            //Temp
            Debug.Log($"Grabbed {other.gameObject.name} with the whip!");
            //Get component of object, call method on it using grabPullDestination as a parameter
        }
    }
}
