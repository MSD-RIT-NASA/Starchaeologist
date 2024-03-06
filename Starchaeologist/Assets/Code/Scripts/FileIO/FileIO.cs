using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Analytics;
using static FileIO;
using System.Linq;

/// <summary>
/// For generalized player level loading and storing. Not
/// for any level or player in particuluar. Also holds 
/// data structures to make the process more convient.
/// 
/// When storing data it expects it in the proper data
/// structures beforehand. 
/// </summary>
public class FileIO
{
    [SerializeField] PlayerData playerSample;
    [SerializeField] PlayerData playerSample2;

    /// <summary>
    /// Load a player's data if possible 
    /// </summary>
    /// <param name="playerName"></param>
    /// <returns></returns>
    public PlayerData LoadData(string playerName)
    {
        string basePath = Directory.GetCurrentDirectory() + "\\scores\\";
        string finalPath = basePath + playerName + ".json";

        if (!File.Exists(finalPath))
            return null;

        string jsonString = File.ReadAllText(finalPath);
        return JsonUtility.FromJson<PlayerData>(jsonString);  
    }


    /// <summary>
    /// Store a players data using the data's player name
    /// as a json directory
    /// </summary>
    /// <param name="data"></param>
    public void StoreData(PlayerData data)
    {
        string basePath = Directory.GetCurrentDirectory() + "\\scores\\";
        string finalPath = basePath + data.playerName + ".json";

        if (!File.Exists(finalPath))
            File.CreateText(finalPath);

        File.WriteAllText(finalPath, JsonUtility.ToJson(data, true));
    }



    #region FileIOStructs

    /// <summary>
    /// Represents a whole json file connected to a single player 
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public LevelData[] levelDatas;

        /// <summary>
        /// Get the level data by passing in its name. If level 
        /// does not exists returns null 
        /// </summary>
        /// <param name="levelName"></param>
        /// <returns></returns>
        public LevelData GetLevelData(string levelName)
        {
            foreach(LevelData levelData in levelDatas) 
            {
                if (levelData.levelName == levelName) 
                    return levelData;
            }

            return null;
        }

        /// <summary>
        /// Store a level data into this player's JSON. If level name
        /// does not exist adds new data to the player's JSON. 
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="data"></param>
        public void SetlevelData(string levelName, FileIO.LevelData data)
        {
            if(levelDatas == null)
                levelDatas = new LevelData[0];

            for (int i = 0; i < levelDatas.Length; i++)
            {
                // Have we found the right level? 
                if (levelDatas[i].levelName == levelName)
                {
                    levelDatas[i] = data;
                }
            }

            // Add data as a new entry 
            FileIO.LevelData[] nextData = new FileIO.LevelData[levelDatas.Length + 1];
            for (int i = 0; i < levelDatas.Length; i++)
            {
                nextData[i] = levelDatas[i];
            }
            nextData[levelDatas.Length] = data;

            levelDatas = nextData;
        }
    }

    /// <summary>
    /// Holds the raw data tied to each particuluar level. Each 
    /// array holds structs of a type and their particuluar 
    /// name. 
    /// </summary>
    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public JSONIntHelper[]  intValues;
        public JSONFloatHelper[] floatValues;
        public JSONStringHelper[] stringValues;

        public int GetInt(string variableName)
        {
            foreach (JSONIntHelper intData in intValues)
            {
                if (intData.name == variableName)
                    return intData.value;
            }

            Debug.Assert(false, "Invalid variable name: " + variableName);

            return -1;
        }

        public float GetFloat(string variableName)
        {
            foreach (JSONFloatHelper floatData in floatValues)
            {
                if (floatData.name == variableName)
                    return floatData.value;
            }

            Debug.Assert(false, "Invalid variable name: " + variableName);

            return -1.0f;
        }

        public string GetString(string variableName)
        {
            foreach (JSONStringHelper stringData in stringValues)
            {
                if (stringData.name == variableName)
                    return stringData.value;
            }

            Debug.Assert(false, "Invalid variable name: " + variableName);

            return "";
        }
    }


    [System.Serializable]
    public class JSONIntHelper
    {
        public string name;
        public int value;
    }

    [System.Serializable]
    public class JSONFloatHelper
    {
        public string name;
        public float value;
    }
    

    [System.Serializable]
    public class JSONStringHelper
    {
        public string name;
        public string value;
    }

    #endregion

}
