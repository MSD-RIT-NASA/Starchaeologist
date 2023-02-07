using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Globalization;

public class ScoreData : MonoBehaviour
{
    public GameObject scoreCanvas;
    public GameObject leaderBoard;
    public GameObject playerDataCanvas;
    public TMP_Text score;
    public TMP_Text playerName;
    public TMP_Text date;
    public TMP_Text playerDataBox;
    public List<TMP_Text> leaderboardEntries;

    private StreamReader reader;
    private StreamWriter writer;

    private List<PlayerData> players;
    private List<PlayerData> leaders;
    private List<PlayerData> singlePlayerData;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<PlayerData>();
        leaders = new List<PlayerData>();
        singlePlayerData = new List<PlayerData>();

        //PopulatePlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Turns on the canvas showing the score at the end of the level
    /// </summary>
    /// <param name="isActive"></param>
    public void SetScoreCanvasActive(bool isActive)
    {
        scoreCanvas.SetActive(isActive);
    }

    public void SetLBCanvasActive(bool isActive)
    {
        leaderBoard.SetActive(isActive);
    }

    public void SetPlayerCanvas(bool isActive)
    {
        playerDataCanvas.SetActive(isActive);
    }


    /// <summary>
    /// Writes new score data to an external file to store for the leaderboard
    /// </summary>
    public void StorePlayerData()
    {
        writer = new StreamWriter("ScoreData.txt", true);

        //float pScoreNum = float.Parse(score.text, CultureInfo.InvariantCulture.NumberFormat);
        //players.Add(new PlayerData(playerName.text, date.text, pScoreNum));

        writer.WriteLine("Player: " + playerName.text);
        writer.WriteLine("Date: " + date.text);
        writer.WriteLine("Score: " + score.text);

        writer.Close();
    }


    /// <summary>
    /// Reads score data from the external file to populate the list of PlayerData
    /// Should run on start to store the data from the external file into variables
    /// </summary>
    public void PopulatePlayers()
    {
        reader = new StreamReader("ScoreData.txt");

        string newLine = reader.ReadLine();
        while(newLine != null)
        {
            string[] data = newLine.Split(' ');
            string pName = data[1];

            newLine = reader.ReadLine();
            data = newLine.Split(' ');
            string pDate = data[1];

            newLine = reader.ReadLine();
            data = newLine.Split(' ');
            string pScore = data[1];
            float pScoreNum = float.Parse(pScore, CultureInfo.InvariantCulture.NumberFormat);

            players.Add(new PlayerData(pName, pDate, pScoreNum));

            newLine = reader.ReadLine();
        }

        reader.Close();
    }


    /// <summary>
    /// Take PlayerData from the players list and display a leaderboard based on the data
    /// </summary>
    public void DisplayLeaderboard()
    {
        if (players.Count == 0)
        {
            PopulatePlayers();
        }

        leaders.Clear();

        SortPlayers();

        if (players.Count >= 10)//Just take the top 10 scores
        {
            for (int i = 0; i < 10; i++)
            {
                leaders.Add(players[i]);
            }
        }
        else//If there are less than 10 scores, just take whatever is there
        {
            for (int i = 0; i < players.Count; i++)
            {
                leaders.Add(players[i]);
            }
        }

        //Display the contents of leaders on the leaderboard canvas
        for(int i = 0; i < leaders.Count; i++)
        {
            leaderboardEntries[i].text = (i + 1) + " - " + leaders[i].PlayerName + ": " + leaders[i].Score + " " + leaders[i].Date;
        }
    }


    /// <summary>
    /// Reads score data from the external file regarding a single user determined by their name/username
    /// </summary>
    /// <param name="name"></param>
    public void ShowPlayerData(TMP_Text name)
    {
        if (players.Count == 0)
        {
            PopulatePlayers();
        }

        singlePlayerData.Clear();

        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].PlayerName == name.text)
            {
                singlePlayerData.Add(players[i]);
            }
        }

        string dataText = "";

        //Display contents of the single player's game data
        for(int i = 0; i < singlePlayerData.Count; i++)
        {
            dataText += singlePlayerData[i].Date + " - " + singlePlayerData[i].PlayerName + ": " + singlePlayerData[i].Score + "\n";
        }
        Debug.Log(dataText);

        playerDataBox.text = dataText;
    }


    private void SortPlayers()
    {
        int topScorerIndex = 0;

        //Sort players list from greatest to least score
        for (int o = 0; o < players.Count; o++)
        {
            topScorerIndex = o;
            for (int i = o; i < players.Count; i++)
            {
                if (players[i].Score > players[topScorerIndex].Score)
                {
                    topScorerIndex = i;
                }
            }
            PlayerData tempPlayer = players[o];
            players[o] = players[topScorerIndex];
            players[topScorerIndex] = tempPlayer;
        }
    }
}
