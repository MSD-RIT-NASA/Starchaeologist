using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipGrabTrigger : MonoBehaviour
{
    [SerializeField] [TagSelector] string[] tagsToGrab;
    [SerializeField] private Transform grabPullDestination;

    private void Start()
    {
        Debug.Assert(grabPullDestination, $"GrabTrigger on {name} is missing a destination for grabbed objects. " +
            $"Did you forget to set one in the inspector?");
    }

    // private void OnEnable() => Debug.Log($"WhipGrabTrigger on {name} is now on.");
    // private void OnDisable() => Debug.Log($"WhipGrabTrigger on {name} is now off.");
    //private void OnEnable() => DebugEntryManager.updateEntry("WhipTriggerActive", "true", -1);
    //private void OnDisable() => DebugEntryManager.updateEntry("WhipTriggerActive", "false", -1);

    private void OnTriggerEnter(Collider other)
    {
        //If other's tag matches any of the tags in tagsToGrab, grab it.
        if (Array.Exists(tagsToGrab, tag => other.CompareTag(tag)))
        {
            // Debug.Log($"Grabbed {other.name}, pulling it to destination {grabPullDestination}");
            DebugEntryManager.updateEntry("Grabbed w/ Whip", $"True, {other.name}, pulling to {grabPullDestination}", -1);
            other.gameObject.GetComponent<WhipGrabbableItem>().FlyToGrabber(grabPullDestination);
        }
        else
        {
            // Debug.Log($"{other.name} was not grabbed; it wasn't tagged with one of " +
            //     $"the following:" + string.Join(" ", tagsToGrab));
            DebugEntryManager.updateEntry("Grabbed w/ Whip", $"False, {other.name}, not in tagsToGrab", -1);
        }
    }
}