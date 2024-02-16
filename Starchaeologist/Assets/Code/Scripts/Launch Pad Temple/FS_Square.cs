using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_Square : MonoBehaviour
{
    [SerializeField] Renderer rend;
    [SerializeField] Color activeColor;
    [SerializeField] Color defaultColor;

    private void Start()
    {
        //rend.material.SetColor("_Color", Color.red);
    }

    public void SetTargetVisual(bool active)
    {
        rend.material.SetColor("_Color", active ? Color.red : Color.green);
        print("Changing selection color");
    }
}
