using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityEstimator : MonoBehaviour
{
    /*TODO:
     * Only update hand velocity if holding button? Move velocity calculation to a new script independent of whip?
     * While holding button, if velocity matches or exceeds minimum swing speed, extend whip
     * Make extended whip detect if it hit a yoinkable thing, and engage the yoink if so
    */

    [Tooltip("The max number of elements allowed in the velocity bank. The oldest element will be removed if an " +
        "addition would exceed the capacity.")]
    [SerializeField] private int velBankCapacity = 10;

    private Vector3 previousPos;
    private LinkedList<Vector3> estVelocityBank;
    public Vector3 CurrentAvgVelocity { get; private set; }

    private void Start()
    {
        estVelocityBank = new LinkedList<Vector3>();
        previousPos = transform.position;
    }

    private void FixedUpdate()
    {
        UpdateVelocityBank();
        CurrentAvgVelocity = GetAverageFromBank();
    }

    /// <summary>
    /// Estimates the current velocity, adds it to <see cref="estVelocityBank"/>, and removes the oldest estimate 
    /// if the bank exceeds <see cref="velBankCapacity"/>.<br/>
    /// This method does nothing if <see cref="Time.fixedDeltaTime"/> is a near-zero float.
    /// </summary>
    private void UpdateVelocityBank()
    {
        if (Mathf.Approximately(Time.fixedDeltaTime, 0))
        {
            Vector3 vel = (transform.position - previousPos) / Time.fixedDeltaTime;
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
    private Vector3 GetAverageFromBank()
    {
        Vector3 avg = Vector3.zero;
        foreach (Vector3 vel in estVelocityBank)
        {
            avg += vel;
        }
        return avg / estVelocityBank.Count;
    }
}
