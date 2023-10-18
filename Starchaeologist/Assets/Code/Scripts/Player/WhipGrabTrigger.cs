using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipGrabTrigger : MonoBehaviour
{
    [SerializeField] [TagSelector] string[] tagsToGrab;
    [SerializeField] private Transform grabPullDestination;
    [Tooltip("On enable, the whip will get valid colliders within a sphere of this radius.")]
    [SerializeField] private Vector3 destinationOffset;

    private void Start()
    {
        Debug.Assert(grabPullDestination, $"GrabTrigger on {name} is missing a destination for grabbed objects. " +
            $"Did you forget to set one in the inspector?");
    }

    private void OnTriggerEnter(Collider other)
    {
        //If other's tag matches any of the tags in tagsToGrab, grab it.
        if (Array.Exists(tagsToGrab, tag => other.CompareTag(tag)))
        {
            other.gameObject.GetComponent<WhipGrabbableItem>().FlyToGrabber(grabPullDestination);
        }
    }
}