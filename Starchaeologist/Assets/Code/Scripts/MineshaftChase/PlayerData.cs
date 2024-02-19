//NASA x RIT author: Noah Flanders

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to take in score data from the text file it is stored in and turn it
//into a more usable form for displaying it in the leaderboard
[Serializable]
public struct PlayerData
{
    public string playerName;
    public string date;
    public float score;
    public string rank;

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
