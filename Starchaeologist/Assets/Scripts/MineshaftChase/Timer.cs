//NASA x RIT author: Noah Flanders

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] 
    private float timeRemaining;

    private float time;

    public float TimeRemaining
    {
        get { return timeRemaining; }
    }
    public float GetTime
    {
        get { return time; }
    }

    private void Start()
    {
        time = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.deltaTime;

        //The timer continues until it reaches 0
        if(timeRemaining > 0)
        {
            timeRemaining -= time;
        }
    }
}
