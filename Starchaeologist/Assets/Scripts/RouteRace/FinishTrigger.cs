using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] GameObject finshLine;
    [SerializeField] RaceGame raceScr;
    [SerializeField] GameObject display;

    void OnTriggerEnter()
    {
        raceScr.PlayerCrossed();
    }
}
