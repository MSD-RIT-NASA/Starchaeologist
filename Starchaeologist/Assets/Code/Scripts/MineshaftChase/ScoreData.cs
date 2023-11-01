//NASA x RIT author: Noah Flanders

//This script interacts with the text files that store the game score data
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Globalization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ScoreData : MonoBehaviour
{
    public GameObject scoreCanvas;
    public GameObject leaderBoard;
    public GameObject playerDataCanvas;
    [SerializeField] private KeyInput keyboardCanvas;
    public TMP_Text score;
    public TMP_InputField playerName;
    public TMP_InputField date;
    public TMP_Text playerDataBox;
    [SerializeField] private TMP_Text rank;
    public List<TMP_Text> leaderboardEntries;
    [SerializeField]
    private TMP_Text balanceScore;
    private int balanceScoreint;

    private ScoreEntries scoreEntries;

    private bool receivedBalanceScore;

    private string currentScene;

    [SerializeField] private TMP_Text storeStatusText;
    [SerializeField] private string storedMessage;
    [SerializeField] private string emptyNameMessage;
    [SerializeField] private string invalidDateMessage;

    private bool hasPopulated;
    private bool scoreCanvasActive;

    [SerializeField]
    private TMP_InputField playerSearchName;

    [SerializeField]
    private UdpSocket server;


    // Start is called before the first frame update
    void Start()
    {
        hasPopulated = false;
        scoreCanvasActive = true;
        receivedBalanceScore = false;
        balanceScoreint = 0;

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            // Store all score files in the Scores folder
            if (!Directory.Exists("Scores"))
                Directory.CreateDirectory("Scores");
            currentScene = "Scores/" + SceneManager.GetActiveScene().name + "Scores";

            playerName.text = keyboardCanvas.NameEdit;
            keyboardCanvas.DateEdit = DateTime.Now.ToShortDateString();
            date.text = keyboardCanvas.DateEdit;
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerSearchName.text = keyboardCanvas.SearchNameEdit;
        if (receivedBalanceScore == true)
        {
            DisplayBalanceScore(balanceScoreint);
            receivedBalanceScore = false;
        }
        //balanceScore.text = 123.ToString();
    }


    /// <summary>
    /// Turns on the canvas showing the score at the end of the level
    /// </summary>
    /// <param name="isActive"></param>
    public void SetScoreCanvasActive(bool isActive)
    {
        //while (balanceScore.text == "0")
        //{

        ////}
        //waitCanvas.SetActive(false);
        scoreCanvas.SetActive(isActive);
        storeStatusText.text = "";
        if (isActive)
        {
            scoreCanvasActive = true;
            keyboardCanvas.ScoreCanvasActive = scoreCanvasActive;
        }
    }

    //Leaderboard Canvas
    public void SetLBCanvasActive(bool isActive)
    {
        leaderBoard.SetActive(isActive);
    }

    //Individual Player Data Canvas
    public void SetPlayerCanvas(bool isActive)
    {
        playerDataCanvas.SetActive(isActive);
        if (isActive)
        {
            scoreCanvasActive = false;
            keyboardCanvas.ScoreCanvasActive = scoreCanvasActive;
        }
    }


    /// <summary>
    /// Writes new score data to an external file to store for the leaderboard
    /// </summary>
    public void StorePlayerData()
    {
        // Do not store empty or invalid data
        string inputName = playerName.text.Trim();
        string inputDateText = date.text.Trim();
        if (inputName.Length == 0)
        {
            storeStatusText.text = emptyNameMessage;
            return;
        }
        if (!DateTime.TryParse(inputDateText, out DateTime inputDate))
        {
            storeStatusText.text = invalidDateMessage;
            return;
        }

        // Ensure that the player list has been populated since we are updating the JSON of existing scores
        PopulatePlayers();

        // Add a new score entry to the player list. If this is the player's second score on this date, override the existing one
        PlayerData newPlayerData = new PlayerData(inputName, inputDate.ToShortDateString(), float.Parse(score.text), rank.text);
        int existingIndex = scoreEntries.players.FindIndex(p =>
            p.PlayerName == newPlayerData.PlayerName
            && p.Date == newPlayerData.Date);
        if (existingIndex != -1)
        {
            // Only override lower scores
            if (newPlayerData.Score >= scoreEntries.players[existingIndex].Score)
                scoreEntries.players[existingIndex] = newPlayerData;
        }
        else
            scoreEntries.players.Add(newPlayerData);
        scoreEntries.lastWrite = DateTime.Now;

        // Reserialize the scores into a JSON file
        SerializeScores();

        storeStatusText.text = storedMessage;
        Debug.Log("Stored");
    }

    private void SerializeScores()
    {
        string fileName = currentScene + ".json";
        File.WriteAllText(fileName, JsonUtility.ToJson(scoreEntries, true));
    }


    /// <summary>
    /// Reads score data from the external file to populate the list of PlayerData
    /// Should run on start to store the data from the external file into variables
    /// </summary>
    public void PopulatePlayers()
    {
        string fileName = currentScene + ".json";
        string json = File.Exists(fileName) ? File.ReadAllText(fileName) : "{}";
        scoreEntries = JsonUtility.FromJson<ScoreEntries>(json);
        hasPopulated = true;
    }


    /// <summary>
    /// Take PlayerData from the players list and display a leaderboard based on the data
    /// </summary>
    public void DisplayLeaderboard()
    {
        //If the list hasn't been populated, read the data from the score file
        if (!hasPopulated)
        {
            PopulatePlayers();
        }

        List<PlayerData> leaders = SortPlayers(9);

        //Display the contents of leaders on the leaderboard canvas
        for (int i = 0; i < leaders.Count; i++)
        {
            leaderboardEntries[i].text = (i + 1) + " - " + leaders[i].PlayerName + ": " + leaders[i].Score + " " + leaders[i].Date;
        }
    }


    /// <summary>
    /// Reads score data from the external file regarding a single user determined by their name/username
    /// </summary>
    /// <param name="name"></param>
    public void ShowPlayerData(TMP_InputField name)
    {
        if (!hasPopulated)
            PopulatePlayers();

        List<PlayerData> singlePlayerData = new List<PlayerData>();

        //Finds all data with the player name that matches the one being searched
        for (int i = 0; i < scoreEntries.players.Count; i++)
        {
            if (scoreEntries.players[i].PlayerName == name.text)
            {
                singlePlayerData.Add(scoreEntries.players[i]);
            }
        }

        string dataText = "";

        //Display contents of the single player's game data
        for (int i = 0; i < singlePlayerData.Count; i++)
        {
            dataText += singlePlayerData[i].Date + " - " + singlePlayerData[i].PlayerName + ": " + singlePlayerData[i].Score + "\n";
        }
        Debug.Log(dataText);

        playerDataBox.text = dataText;
    }

    /// <summary>
    /// Puts the player data in order by score
    /// </summary>
    /// <param name="maxCount">Maximum number of players to include</param>
    /// <returns>List of the top N score entries by score</returns>
    private List<PlayerData> SortPlayers(int maxCount)
    {
        List<PlayerData> sorted = new List<PlayerData>(scoreEntries.players);
        sorted.Sort((a, b) => b.Score.CompareTo(a.Score));
        if (sorted.Count > maxCount)
            sorted.RemoveRange(maxCount, sorted.Count - maxCount);
        return sorted;
    }

    public void ShowKeyboard()
    {
        keyboardCanvas.gameObject.SetActive(true);
        keyboardCanvas.ScoreCanvasActive = scoreCanvasActive;
    }

    public void EditingName()
    {
        keyboardCanvas.EditingName = true;
    }

    public void EditingDate()
    {
        keyboardCanvas.EditingName = false;
    }

    public void SetBalanceScore(string balScore)
    {
        balanceScoreint = int.Parse(balScore);
        receivedBalanceScore = true;
    }
    public void DisplayBalanceScore(int balScore)
    {
        Debug.Log("displaying bScore");
        balanceScore.SetText("" + balScore);
        balanceScore.ForceMeshUpdate();
        DetermineRank(balScore);

        Debug.Log("Displayed bScore");
    }

    public void DetermineRank(int score)
    {
        Debug.Log("Determining Rank...");
        if (score < 40)
        {
            rank.text = "D";
        }
        else if (score >= 40 && score < 60)
        {
            rank.text = "C";
        }
        else if (score >= 60 && score < 80)
        {
            rank.text = "B";
        }
        else if (score >= 80 && score < 95)
        {
            rank.text = "A";
        }
        else if (score >= 95)
        {
            rank.text = "S";
        }

        scoreCanvas.SetActive(false);
        scoreCanvas.SetActive(true);
        Debug.Log("Rank should be displaying...");
    }

    public void SetCurrentScene(TMP_Text option)
    {
        currentScene = option.text;
    }
}
