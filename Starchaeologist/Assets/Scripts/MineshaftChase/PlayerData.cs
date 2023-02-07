using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    private string playerName;
    private string date;
    private float score;

    public PlayerData(string name, string date, float score)
    {
        this.playerName = name;
        this.date = date;
        this.score = score;
    }


    public float Score
    {
        get { return score; }
    }

    public string PlayerName
    {
        get { return playerName; }
    }

    public string Date
    {
        get { return date; }
    }
}
