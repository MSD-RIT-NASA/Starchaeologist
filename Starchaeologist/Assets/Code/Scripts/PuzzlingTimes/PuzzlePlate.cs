using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlePlate : MonoBehaviour
{
    [SerializeField] Trap trap;
    [SerializeField] TeleportationArea tpArea;

    [Header("Anim Settings")]
    [SerializeField] float pressDownSpeed;
    [SerializeField] AnimationCurve curve;

    // When pushed down how far does it move on the y-axis 
    [SerializeField] float downAmount;


    private bool hasBeenTriggered = false;
    private int index; 

    public int Index { get { return index; } }   
 
    /// <summary>
    /// Sets the index of this plate along the grid.
    /// Does not store x and y seperately 
    /// </summary>
    public void SetIndex(int index )
    {
        this.index = index;
    }

    /// <summary>
    /// Allows this plate to be teleported onto or not 
    /// </summary>
    /// <param name="isWalkable"></param>
    public void SetWalkStatus(bool isWalkable)
    {
        tpArea.enabled = isWalkable;
    }

    /// <summary>
    /// Begins the trap logic if this plate has
    /// not already been activated 
    /// </summary>
    public void ActivateTrap()
    {
        if (hasBeenTriggered)
            return;

        hasBeenTriggered = true;

        // Animate the plate 
        StartCoroutine(PressPlateDown());

        // Activate the trap if possible 
        if(trap != null)
            trap.ActivateTrap();
    }

    private IEnumerator PressPlateDown()
    {
        float startHeight = this.transform.position.y;
        float target = this.transform.position.y - downAmount;

        float lerp = 0;
        while(lerp <= 1.0f)
        {
            // Interpolate between start height and new height 
            this.transform.position = new Vector3(this.transform.position.x, Mathf.Lerp(startHeight, target, curve.Evaluate(lerp)), this.transform.position.z);

            lerp += Time.deltaTime * pressDownSpeed;
            yield return null;
        }
    }
}
