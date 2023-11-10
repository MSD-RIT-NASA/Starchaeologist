using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleAnim : MonoBehaviour
{
    [SerializeField] Transform target;

    public void SetTarget(Transform nextTarget)
    {
        target = nextTarget;
    }

    void Update()
    {
        // Simply tracks and orients 
        this.transform.right = target.position - this.transform.position;
    }

}
