using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityEstimator : MonoBehaviour
{
    [Tooltip("The max number of elements allowed in the velocity bank. The oldest element will be removed if an " +
        "addition would exceed the capacity.")]
    [SerializeField] [Min(1)] private int velBankCapacity = 10;
    [Tooltip("Whether this estimator is actually estimating right now, or not. Basically, an on/off switch.")]
    [SerializeField] private bool estimating = true;

    private Vector3 previousPos;
    private LinkedList<Vector3> estVelocityBank;
    private Vector3? currentAvgVel = null;
    public Vector3? CurrentAvgVelocity
    {
        get => currentAvgVel;
        private set => currentAvgVel = value;
    }

    private void Start()
    {
        estVelocityBank = new LinkedList<Vector3>();
        previousPos = transform.localPosition;
    }

    /// <summary>
    /// Turn estimation on/off.<br/>
    /// If switching from on to off or vice-versa, clears <see cref="estVelocityBank"/> to avoid polluting averages 
    /// with old samples.
    /// </summary>
    /// <param name="shouldEstimate">If true, turn estimation on. If false, turn estimation off.</param>
    public void SetEstimationActve(bool shouldEstimate)
    {
        if (estimating != shouldEstimate)
        {
            estimating = shouldEstimate;
            estVelocityBank.Clear();
        }
    }

    private void FixedUpdate()
    {
        DebugEntryManager.updateEntry?.Invoke("V Est Local Pos", $"<color=#FF0000>{transform.localPosition.x}</color>, " +
            $"<color=#00FF00>{transform.localPosition.y}</color>, <color=#0000FF>{transform.localPosition.z}</color>", -1);

        if (estimating)
        {
            UpdateVelocityBank();
            CurrentAvgVelocity = GetAverageFromBank();

            //if (currentAvgVel is Vector3 cAvg)
            //    DebugEntryManager.updateEntry?.Invoke($"V Estimator", $"V = <color=#FF0000>{cAvg.x}</color>, " +
            //        $"<color=#00FF00>{cAvg.y}</color>, <color=#0000FF>{cAvg.z}</color>", 3);
        }

        //After doing everything else, set previousPos to be the current pos (since it's about to be "previous")
        previousPos = transform.localPosition;
    }

    /// <summary>
    /// Estimates the current velocity, adds it to <see cref="estVelocityBank"/>, and removes the oldest estimate 
    /// if the bank exceeds <see cref="velBankCapacity"/>.<br/>
    /// This method does nothing if <see cref="Time.fixedDeltaTime"/> is a near-zero float.
    /// </summary>
    private void UpdateVelocityBank()
    {
        if (!Mathf.Approximately(Time.fixedDeltaTime, 0))
        {
            Vector3 vel = (transform.localPosition - previousPos) / Time.fixedDeltaTime;
            if (estVelocityBank.Count >= velBankCapacity)
            {
                estVelocityBank.RemoveLast();
            }
            estVelocityBank.AddFirst(vel);
        }
    }

    /// <summary>
    /// Loops through <see cref="estVelocityBank"/> to return the mean average of its elements.
    /// </summary>
    /// <returns>The mean average of <see cref="estVelocityBank"/>.</returns>
    private Vector3? GetAverageFromBank()
    {
        if (estVelocityBank.Count < 1)
            return null;

        Vector3 avg = Vector3.zero;
        foreach (Vector3 vel in estVelocityBank)
        {
            avg += vel;
        }
        return avg / estVelocityBank.Count;
    }
}