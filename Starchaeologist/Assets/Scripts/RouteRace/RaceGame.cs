using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceGame : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject finishLine;
    [SerializeField] TMP_Text displayTime;
    [SerializeField] TMP_Text collisions;

    public float minuteCount;
    public float secondCount;
    public float milliCount;


    void Start()
    {
        
    }

    
    void Update()
    {
        //Timer to count down to the millisecond
        //To be implemented with the final text
        milliCount += Time.deltaTime * 10;
        if(milliCount >= 10)
        {
            milliCount = 0;
            secondCount++;
        }
        
    }

    public void PlayerCrossed()
    {
        displayTime.SetText("{0} Seconds!", secondCount);

        VehicleController temp = player.GetComponent<VehicleController>();
        collisions.SetText("{0}", temp.timesCollided);
    }
}
