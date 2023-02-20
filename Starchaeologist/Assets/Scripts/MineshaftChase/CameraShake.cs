using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class CameraShake : MonoBehaviour
{
    //private float maxShake = 1f;
    //private float shakeSpeed = 0.3f;
    //private bool touchMax = false;
    //private bool touchMin = false;

    [SerializeField]
    private Animator anim;

    //public Shaker shaker;

    //public ShakePreset shakePre;

    public void OnTriggerEnter(Collider trigger)
    {
        //If the player passes through the TNT Zone
        if(trigger.gameObject.tag == "TntZone")
        {
            //shaker.Shake(shakePre);

            //anim.SetBool("cameraShouldShake", true);

            //Debug.Log("Start shaking");
            ////Start shaking camera
            //while (Mathf.Abs(shakeSpeed) > 0f) 
            //{
            //    this.transform.localPosition = new Vector3(this.transform.localPosition.x + shakeSpeed, this.transform.localPosition.y, this.transform.localPosition.z);

            //    if(this.transform.localPosition.x >= maxShake)
            //    {
            //        touchMax = true;
            //        shakeSpeed *= -1;
            //    }
            //    if(this.transform.localPosition.x <= -maxShake)
            //    {
            //        touchMin = true;
            //        shakeSpeed *= -1;
            //    }

            //    if(touchMax && touchMin)
            //    {
            //        shakeSpeed *= 0.5f;
            //        maxShake *= 0.75f;

            //        touchMax = false;
            //        touchMin = false;
            //    }
            //    if(shakeSpeed <= 0.02f)
            //    {
            //        shakeSpeed = 0f;
            //    }
            //}
        }
    }
}
