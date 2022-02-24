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

    private void OnEnable()
    {
        Debug.Log($"WhipGrabTrigger on {gameObject.name} is now on.");
    }
    private void OnDisable()
    {
        Debug.Log($"WhipGrabTrigger on {gameObject.name} is now off.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WhipGrabbable"))
        {
            Debug.Log($"Grabbing {other.gameObject.name} with the whip...");
            other.gameObject.GetComponent<WhipGrabbableItem>().FlyToGrabber(grabPullDestination);
        }
    }
}
