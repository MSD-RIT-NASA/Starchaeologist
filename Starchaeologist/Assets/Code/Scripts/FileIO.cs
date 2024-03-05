using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Analytics;

/// <summary>
/// For generalized player level loading and storing. Not
/// for any level or player in particuluar. Also holds 
/// data structures to make the process more convient.
/// 
/// When storing data it expects it in the proper data
/// structures beforehand. 
/// </summary>
public class FileIO : MonoBehaviour
{
    [SerializeField] PlayerData playerSample;
    [SerializeField] PlayerData playerSample2;

    // Start is called before the first frame update
    void Start()
    {
        //WriteObjectDataToFile("MyTest.json", new JSONHelperObject("Name", "Level"));
        StoreData(playerSample);

        playerSample2 = LoadData(playerSample.playerName);
    }


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
