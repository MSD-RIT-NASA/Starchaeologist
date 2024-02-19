using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_Square : MonoBehaviour
{
    [SerializeField] Renderer rend;
    [SerializeField] Color targetColor;
    [SerializeField] Color nonTargetColor;
    [SerializeField] Color defaultColor;

    private void Start()
    {
        //rend.material.SetColor("_Color", Color.red);
    }

    public void SetTargetVisual(FSSquareStates state)
    {
        //rend.material.SetColor("_Color", active ? Color.red : Color.green);

        switch (state)
        {
            case FSSquareStates.TARGET:
                rend.material.SetColor("_Color", targetColor);
                break;
            case FSSquareStates.NOT_TARGET:
                rend.material.SetColor("_Color", nonTargetColor);
                break;
            case FSSquareStates.NOT_IN_PLAY:
                rend.material.SetColor("_Color", defaultColor);
                break;
        }


        print("Changing selection color");
    }

    public enum FSSquareStates
    {
        TARGET,
        NOT_TARGET,
        NOT_IN_PLAY 
    }
}
