using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private string name;
    private string date;
    private float score;

    public PlayerData(string name, string date, float score)
    {
        this.name = name;
        this.date = date;
        this.score = score;
    }


    public float Score
    {
        get { return score; }
    }
}
