using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    private string playerName;
    private string date;
    private float score;
    private string rank;

    public PlayerData(string name, string date, float score, string rank)
    {
        this.playerName = name;
        this.date = date;
        this.score = score;
        this.rank = rank;
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

    public string Rank
    {
        get { return rank; }
    }
}
