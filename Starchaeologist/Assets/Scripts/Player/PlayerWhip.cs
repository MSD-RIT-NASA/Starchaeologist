using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWhip : MonoBehaviour
{
    //Things needed:
    //Current pos
    //Previous pso
    //From those, get current estimated velocity; (pos - prev) / Time.deltaTime
    //store that calculated velocity in a queue with a set size, so we have multiple
    //  velocities to average between across the past few frames
    //Use averaged velocity to determine where the player's hand is going, and make the whip go there
    //Remember to account for division by 0, like if Time.timeScale becomes 0 due to a pause or something
}
