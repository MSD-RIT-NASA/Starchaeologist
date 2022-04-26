using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipGrabTrigger : MonoBehaviour
{
    [SerializeField] [TagSelector] string[] tagsToGrab;
    [SerializeField] private Transform grabPullDestination;
    [Tooltip("On enable, the whip will get valid colliders within a sphere of this radius.")]
    [SerializeField] [Min(0)] private float pretestRadius;
    [SerializeField] private Vector3 destinationOffset;

    private Collider thisColl;
    private Collider[] cachedOverlaps;
    private Coroutine checkerCorout;

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
        cachedOverlaps = Array.FindAll(
            Physics.OverlapSphere(transform.position, pretestRadius, Physics.AllLayers, QueryTriggerInteraction.Collide),

            //Find and stash each one that has any of the tags in `tagsToGrab`
            overlap => Array.Exists(tagsToGrab, tag => overlap.CompareTag(tag)));

        if (cachedOverlaps != null)
        {
            DebugEntryManager.updateEntry("Pretest Overlaps",
                $"{cachedOverlaps.Length} results: {string.Join<Collider>(" ", cachedOverlaps)}",
                -1);
            Debug.Log($"Pretest Overlaps: {cachedOverlaps}");

            //Now repeatedly check for hits (actual overlaps) indefinitely—or, rather, until disabled
            //  Note the interval; we don't actually need to check every frame
            CheckForHits();
            checkerCorout = Coroutilities.DoUntil(this, CheckForHits, () => false, 0.1f);
        }
    }

    private void OnDisable()
    {
        //DebugEntryManager.updateEntry("WhipTriggerActive", "false", -1);

        Coroutilities.TryStopCoroutine(this, ref checkerCorout);
    }

    private void CheckForHits()
    {
        List<int> hitIndices = new List<int>(cachedOverlaps.Length);

        for (int i = 0; i < cachedOverlaps.Length; i++)
        {
            Collider olap = cachedOverlaps[i];

            //Check if olap and thisColl are overlapping
            if (olap && Physics.ComputePenetration(
                thisColl, transform.position, transform.rotation,
                olap, olap.transform.position, olap.transform.rotation,
                out _, out _))
            {
                //Since olap is confirmed to have a tag we want, it should also have a fly to grabber method.
                olap.GetComponent<WhipGrabbableItem>().FlyToGrabber(grabPullDestination, destinationOffset);
                hitIndices.Add(i);
            }
        }

        DebugEntryManager.updateEntry("Grab Hits",
            $"{hitIndices.Count} hits; hit indices are [{string.Join(" ", hitIndices)}]",
            -1);
        //Nullify all our references to any hits, to prevent repeat calls
        foreach (int index in hitIndices)
            cachedOverlaps[index] = null;
    }

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