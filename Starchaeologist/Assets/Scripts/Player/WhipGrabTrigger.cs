using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipGrabTrigger : MonoBehaviour
{
    [SerializeField] [TagSelector] string[] tagsToGrab;
    [SerializeField] private Transform grabPullDestination;
    [Tooltip("On enable, the whip will get valid colliders within a sphere of this radius.")]
    [SerializeField] private float pretestRadius;

    Collider thisColl;

    private void Start()
    {
        Debug.Assert(grabPullDestination, $"GrabTrigger on {name} is missing a destination for grabbed objects. " +
            $"Did you forget to set one in the inspector?");

        thisColl = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        //DebugEntryManager.updateEntry("WhipTriggerActive", "true", -1);

        //Out of all the colliders within pretestRadius of this,
        Collider[] overlaps = Array.FindAll(
            Physics.OverlapSphere(transform.position, pretestRadius, Physics.AllLayers, QueryTriggerInteraction.Collide),

            //Find and stash each one that has any of the tags in `tagsToGrab`
            overlap => Array.Exists(tagsToGrab, tag => overlap.CompareTag(tag)));

        foreach (Collider olap in overlaps)
        {
            //Manual check if olap also overlaps thisColl
        }
    }

    //private void OnDisable() => DebugEntryManager.updateEntry("WhipTriggerActive", "false", -1);

    /// Does not work
    //private void OnTriggerEnter(Collider other)
    //{
    //    //If other's tag matches any of the tags in tagsToGrab, grab it.
    //    if (Array.Exists(tagsToGrab, tag => other.CompareTag(tag)))
    //    {
    //        // Debug.Log($"Grabbed {other.name}, pulling it to destination {grabPullDestination}");
    //        DebugEntryManager.updateEntry("Grabbed w/ Whip", $"True, {other.name}, pulling to {grabPullDestination}", -1);
    //        other.gameObject.GetComponent<WhipGrabbableItem>().FlyToGrabber(grabPullDestination);
    //    }
    //    else
    //    {
    //        DebugEntryManager.updateEntry("Grabbed w/ Whip", $"False, {other.name}'s tag, {other.tag}, is not in tagsToGrab", -1);
    //    }
    //}
}