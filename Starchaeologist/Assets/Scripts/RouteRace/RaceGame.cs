using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceGame : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject finishLine;
    [SerializeField] GameObject display;

    public float minuteCount;
    public float secondCount;
    public float milliCount;


    void Start()
    {
        
    }

    
    void Update()
    {
        secondCount += Time.deltaTime;
    }

    public void PlayerCrossed()
    {

    }
}
