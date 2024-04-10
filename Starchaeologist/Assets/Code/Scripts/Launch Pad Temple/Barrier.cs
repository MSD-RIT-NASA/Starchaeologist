using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] Renderer rend;
    [SerializeField] Color dangerColor;
    [SerializeField] Color nonDangerousColor;

    private TileTrialGM gm;
    private bool isDangerous;

    private void Start()
    {
        gm = GameObject.FindObjectOfType<TileTrialGM>();
        SetDanger(true);
    }

    /// <summary>
    /// Change whether or not this barrier can deduct points from the player
    /// and changes its visual based on that status 
    /// </summary>
    /// <param name="isDangerous"></param>
    public void SetDanger(bool isDangerous)
    {
        this.isDangerous = isDangerous;
        rend.material.SetColor("_Color", isDangerous ? dangerColor : nonDangerousColor);
    }


    private void OnTriggerEnter(Collider other)
    {
        print(other.tag);

        if (!isDangerous)
            return;

        if(other.tag == "PlayerHand")
        {
            gm.TakeCollision();
            SetDanger(false);
        }
    }
}
