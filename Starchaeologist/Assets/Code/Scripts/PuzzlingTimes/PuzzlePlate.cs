using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlePlate : MonoBehaviour
{
    [SerializeField] Trap trap;
    [SerializeField] TeleportationArea tpArea;

    private bool hasBeenTriggered = false;
 
    public void SetWalkStatus(bool isWalkable)
    {
        tpArea.enabled = isWalkable;
    }

    public void ActivateTrap()
    {
        if (hasBeenTriggered)
            return;

        trap.ActivateTrap();
    }
}
