using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trap_Indicator : MonoBehaviour
{
    //Indicator should only be visible when active (i.e. when a trap is activated)
    private bool isActive;

    public Transform playerTransform;
    private Transform targetTransform;
    //Calculated by the difference of the player and target positions
    private Vector3 indicatingTarget;

    public Text indicator;

    //Calculated using indicatingTarget
    private float angleToRotate;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        indicatingTarget = Vector3.zero;
        angleToRotate = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        indicator.enabled = isActive;

        if (isActive)
        {
            /* Pseudo-code
                theta = angle between indicatingTarget and indicator.right
                indicator.rotate by theta degrees

                cos(theta) = x-axis/indicatingTarget
                cos(theta) = indicatingTarget.x/Math.sqrt(indicatingTarget.x^2 + indicatingTarget.y^2)
            */
            angleToRotate = Mathf.Acos(indicatingTarget.x / Mathf.Sqrt(Mathf.Pow(indicatingTarget.x, 2) + Mathf.Pow(indicatingTarget.y, 2)));
            indicator.transform.Rotate(new Vector3(0.0f, 0.0f, angleToRotate));
        }
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
