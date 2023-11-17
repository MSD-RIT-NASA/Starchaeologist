using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLog : Trap
{
    // Used to determine which way the trap will swing in
    // the animation. Since we do not know what axis the log
    // will be set up this variable has a very vague name 
    // purely for swapping which side to swing to.
    private bool sideSwap = true;
    public override void ActivateTrap()
    {
        // Begin log animation 
    }

    // Following Functions are for the animation event 

    /// <summary>
    /// Display an area to avoid the player's head 
    /// </summary>
    public void OnWarn()
    {

    }

    /// <summary>
    /// End the warning 
    /// </summary>
    public void OnLeave()
    {

    }
    
}
