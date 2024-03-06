using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FileIO;

/// <summary>
/// Used to store and load data using FileIo. Loading
/// data is still fairly generalized but is more 
/// human readable than the FileIO. Functions are 
/// explicitly made for each game mode in this 
/// script where data is turned into FIleIO data
/// structures. 
/// </summary>
public class DataStoreLoad : MonoBehaviour
{
    private FileIO fileIO;
    private FileIO.PlayerData currentData;

    // Gamemode names 
    const string FOURSQUARE_NAME = "FourSquare";
    const string TESTER_NAME = "Test";

    private void Awake()
    {
        // Initialize 
        fileIO = new FileIO();
        currentData = null;

        StoreFourSquare("The Watching Person", 150.0f, 5, 230.0f);
        StoreTest("The Watching Person", "This is just antoher value");
    }

    #region GAMEMODE_STORING

    /// <summary>
    /// Store the data recorded for the FourSquare gamemode 
    /// </summary>
    /// <param name="score"></param>
    /// <param name="collisions"></param>
    /// <param name="time"></param>
    public void StoreFourSquare(string playerName, float score, int collisions, float time)
    {
        // Create a new PlayerData to override previous data 
        FileIO.PlayerData data = new FileIO.PlayerData();
        data.playerName = playerName;


        // Score
        FileIO.JSONFloatHelper scoreStore = new FileIO.JSONFloatHelper();
        scoreStore.name = "score";
        scoreStore.value = score;

        // Collisions 
        FileIO.JSONIntHelper collisionsStore = new FileIO.JSONIntHelper();
        collisionsStore.name = "collisions";
        collisionsStore.value = collisions;

        // Time 
        FileIO.JSONFloatHelper timeStore = new FileIO.JSONFloatHelper();
        timeStore.name = "time";
        timeStore.value = time;


        // Form LevelData data structures and store them into 
        // our new level data 
        FileIO.LevelData levelData = new FileIO.LevelData();
        levelData.levelName = FOURSQUARE_NAME;

        JSONIntHelper[]    intHelpers       = new JSONIntHelper[]       { collisionsStore };
        JSONFloatHelper[]  floatHelpers     = new JSONFloatHelper[]     { scoreStore, timeStore };
        JSONStringHelper[] stringHelpers    = new JSONStringHelper[]    { };
        levelData.intValues     = intHelpers;
        levelData.floatValues   = floatHelpers;
        levelData.stringValues  = stringHelpers;

        // Data is already associated with a player 
        data.SetlevelData(FOURSQUARE_NAME, levelData);
        
        // Send data to FileIO 
        fileIO.StoreData(data);
    }

    public void StoreTest(string playerName, string value)
    {
        // Create a new PlayerData to override previous data 
        FileIO.PlayerData data = fileIO.LoadData(playerName);
        if (data == null)
            data = new FileIO.PlayerData();
            
        data.playerName = playerName;


        // Value
        FileIO.JSONStringHelper valueStore = new FileIO.JSONStringHelper();
        valueStore.name = "value";
        valueStore.value = value;



        // Form LevelData data structures and store them into 
        // our new level data 
        FileIO.LevelData levelData = new FileIO.LevelData();
        levelData.levelName = TESTER_NAME;

        JSONIntHelper[] intHelpers = new JSONIntHelper[]          { };
        JSONFloatHelper[] floatHelpers = new JSONFloatHelper[]    { };
        JSONStringHelper[] stringHelpers = new JSONStringHelper[] { valueStore };
        levelData.intValues = intHelpers;
        levelData.floatValues = floatHelpers;
        levelData.stringValues = stringHelpers;

        // Data is already associated with a player 
        data.SetlevelData(TESTER_NAME, levelData);

        // Send data to FileIO 
        fileIO.StoreData(data);
    }

    #endregion

    #region GETTERS

    /// <summary>
    /// Get a specific variable's value for a player in a specific level
    /// </summary>
    /// <returns></returns>
    public float GetFloat(string playerName, string levelName, string floatName)
    {
        if (currentData == null || currentData.playerName != playerName)
            currentData = fileIO.LoadData(playerName);

        LevelData level = currentData.GetLevelData(levelName);

        return level.GetFloat(floatName);
    }

    /// <summary>
    /// Get a specific variable's value for a player in a specific level
    /// </summary>
    /// <returns></returns>
    public int GetInt(string playerName, string levelName, string intName)
    {
        if (currentData == null || currentData.playerName != playerName)
            currentData = fileIO.LoadData(playerName);

        LevelData level = currentData.GetLevelData(levelName);

        return level.GetInt(intName);
    }

    /// <summary>
    /// Get a specific variable's value for a player in a specific level
    /// </summary>
    /// <returns></returns>
    public string GetString(string playerName, string levelName, string stringName)
    {
        if (currentData == null || currentData.playerName != playerName)
            currentData = fileIO.LoadData(playerName);

        LevelData level = currentData.GetLevelData(levelName);

        return level.GetString(stringName);
    }

    #endregion
}