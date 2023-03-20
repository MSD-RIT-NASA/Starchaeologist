using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trap_Indicator : MonoBehaviour
{
    //Indicator should only be visible when active (i.e. when a trap is activated)
    private bool isActive;

    public Transform playerTransform;
    private Vector3 targetPosition;
    //Calculated by the difference of the player and target positions
    private Vector3 indicatingTarget;

    public GameObject indicator;

    //Calculated using indicatingTarget
    private float angleToRotateX;
    private float angleToRotateY;
    private float angleToRotateZ;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        indicatingTarget = Vector3.zero;
        angleToRotateX = 0.0f;
        angleToRotateY = 0.0f;
        angleToRotateZ = 0.0f;
    }
    

    // Update is called once per frame
    void Update()
    {
        indicator.SetActive(isActive);

        if (isActive)
        {
            /* Pseudo-code
                theta = angle between indicatingTarget and indicator.right
                indicator.rotate by theta degrees

                cos(theta) = x-axis/indicatingTarget
                cos(theta) = indicatingTarget.x/Math.sqrt(indicatingTarget.x^2 + indicatingTarget.y^2)
            */
            // angleToRotate = Mathf.Acos(indicatingTarget.x / Mathf.Sqrt(Mathf.Pow(indicatingTarget.x, 2) + Mathf.Pow(indicatingTarget.y, 2)));
            // indicator.transform.rotation = new Quaternion(0.0f, 0.0f, angleToRotate, 1f);
            // angleToRotateZ = Vector2.Angle(new Vector2(indicator.transform.right.x, indicator.transform.right.y), new Vector2(indicatingTarget.x, indicatingTarget.y));
            // angleToRotateY = Vector2.Angle(new Vector2(indicator.transform.right.x, indicator.transform.right.z), new Vector2(indicatingTarget.x, indicatingTarget.z));
            // angleToRotateX = Vector2.Angle(new Vector2(indicator.transform.right.y, indicator.transform.right.z), new Vector2(indicatingTarget.y, indicatingTarget.z));
            // indicator.transform.rotation = new Quaternion(angleToRotateX, angleToRotateY, angleToRotateZ, 1);//Changed to setting the rotation instead of adding it

            indicator.transform.LookAt(targetPosition);
        }
    }

    //Sets the target transform and calculate the indicating vector accordingly
    public void SetTarget(Vector3 targetPos)
    {
        targetPosition = targetPos;
        indicatingTarget = targetPos - indicator.transform.position;
        Debug.Log("TargetTransform has been set");
    }

    //Sets the isActive bool to determine of the indicator should be displayed
    public void SetTrapActive(bool active)
    {
        isActive = active;
        Debug.Log("SetTrapActive = " + isActive);
    }
}
