using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleAnim : MonoBehaviour
{
    [SerializeField] Vector3 offsetToTarget;

    public void SetTarget(Vector3 nextTarget)
    {
        offsetToTarget = nextTarget;
    }

    void Update()
    {
        // Simply tracks and orients 
        this.transform.right = this.transform.position +offsetToTarget;
    }

}
