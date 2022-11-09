using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Indicator : MonoBehaviour
{
    //Indicator should only be visible when active (i.e. when a trap is activated)
    private bool isActive;

    public Transform playerTransform;
    private Transform targetTransform;
    //Calculated by the difference of the player and target positions
    private Vector3 indicatingTarget;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        indicatingTarget = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Sets the target transform and calculate the indicating vector accordingly
    public void SetTarget(Transform targetTrans)
    {
        targetTransform = targetTrans;
        indicatingTarget = targetTransform.position - playerTransform.position;
    }

    //Sets the isActive bool to determine of the indicator should be displayed
    public void SetTrapActive(bool active)
    {
        isActive = active;
    }
}
