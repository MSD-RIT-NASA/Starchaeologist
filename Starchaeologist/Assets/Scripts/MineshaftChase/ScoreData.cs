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
    private StreamReader reader;
    private StreamWriter writer;

    private List<PlayerData> players;
    private List<PlayerData> leaders;
    private List<PlayerData> singlePlayerData;

    private bool receivedBalanceScore;

    private string currentScene;

    [SerializeField]
    private GameObject storedMessage;

    private bool hasPopulated;
    private bool scoreCanvasActive;

    [SerializeField]
    private TMP_InputField playerSearchName;

    [SerializeField]
    private GameObject waitCanvas;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<PlayerData>();
        leaders = new List<PlayerData>();
        singlePlayerData = new List<PlayerData>();
        hasPopulated = false;
        scoreCanvasActive = true;
        receivedBalanceScore = false;
        balanceScoreint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            currentScene = SceneManager.GetActiveScene().name + "Scores";
        }
        if (SceneManager.GetActiveScene().name != "MainMenu") 
        {
            playerName.text = keyboardCanvas.NameEdit;
            date.text = keyboardCanvas.DateEdit;
        }
        playerSearchName.text = keyboardCanvas.SearchNameEdit;
        if(receivedBalanceScore == true)
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
        storedMessage.SetActive(false);
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
        string fileName = currentScene + ".txt";
        writer = new StreamWriter(fileName, true);

        writer.WriteLine("Player: " + playerName.text);
        writer.WriteLine("Date: " + date.text);
        writer.WriteLine("Score: " + score.text);
        writer.WriteLine("Rank: " + rank.text);

        writer.Close();

        storedMessage.SetActive(true);
        Debug.Log("Stored");
    }


    /// <summary>
    /// Reads score data from the external file to populate the list of PlayerData
    /// Should run on start to store the data from the external file into variables
    /// </summary>
    public void PopulatePlayers()
    {
        string fileName = currentScene + ".txt";
        reader = new StreamReader(fileName);

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

            newLine = reader.ReadLine();
            data = newLine.Split(' ');
            string pRank = data[1];

            //Creates new instance of PlayerData object
            players.Add(new PlayerData(pName, pDate, pScoreNum, pRank));

            newLine = reader.ReadLine();
        }

        reader.Close();
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

        leaders.Clear();

        SortPlayers();//Orders the list of players

        if (players.Count >= 9)//Just take the top 9 scores
        {
            for (int i = 0; i < 9; i++)
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
    public void ShowPlayerData(TMP_InputField name)
    {
        if (players.Count == 0)
        {
            PopulatePlayers();
        }

        singlePlayerData.Clear();

        //Finds all data with the player name that matches the one being searched
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

    /// <summary>
    /// Puts the player data in order by score
    /// </summary>
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
        if(score < 40)
        {
            rank.text = "D";
        }
        else if(score >= 40 && score < 60)
        {
            rank.text = "C";
        }
        else if(score >= 60 && score < 80)
        {
            rank.text = "B";
        }
        else if(score >= 80 && score < 95)
        {
            rank.text = "A";
        }
        else if(score >= 95)
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
