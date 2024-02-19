using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScoreEntries
{
    public DateTime lastWrite;
    public List<PlayerData> players;
}