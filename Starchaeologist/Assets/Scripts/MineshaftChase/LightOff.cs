using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOff : MonoBehaviour
{
    //when the player goes into the sensory deprevation section turn off  the light on the minecart
    //when the player leaves the section turn it back on.
    private void OnTriggerEnter(Collider other)
    {
        GameObject player = GameObject.Find("XR Rig 2").gameObject.transform.Find("Point Light").gameObject;
        if (player.activeInHierarchy)
        {
            player.SetActive(false);
            Destroy(this);
        }
        else
        {
            player.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
