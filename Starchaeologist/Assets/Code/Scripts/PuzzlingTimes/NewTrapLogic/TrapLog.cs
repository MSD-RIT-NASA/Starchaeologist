using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLog : Trap
{
    [SerializeField] Vector3 hitBoxPos;
    [SerializeField] Vector3 hitBoxSize;

    public override void ActivateTrap()
    {

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position + hitBoxPos, hitBoxSize);
    }
}
