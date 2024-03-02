using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileIO : MonoBehaviour
{
    [SerializeField] TestObject testingJSONObj;

    // Start is called before the first frame update
    void Start()
    {
        WriteObjectDataToFile("MyTest.json", new JSONHelperObject("Name", "Level"));
    }

    /// <summary>
    /// Writes to a file within the scores folder. Do NOT include
    /// \\ in the beginning for the file path and assume it is already 
    /// included.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    private void WriteObjectDataToFile(string path, JSONHelperObject obj)
    {
        string basePath = Directory.GetCurrentDirectory() + "\\scores\\";
        string finalPath = basePath + path;

        File.WriteAllText(finalPath, JsonUtility.ToJson(obj, true));
    }

    /// <summary>
    /// Appends to a file within the scores folder. Do NOT include
    /// \\ in the beginning for the file path and assume it is already 
    /// included.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    private void AppendObjectDataToFile(string path, JSONHelperObject obj)
    {
        string basePath = Directory.GetCurrentDirectory() + "\\scores\\";
        string finalPath = basePath + path;

        using (StreamWriter sw = File.AppendText(finalPath))
        {
            sw.WriteLine(JsonUtility.ToJson(obj, true));
        }
    }

    [System.Serializable]
    public class JSONHelperObject
    {
        public string name;
        public string level;
        public int floatCount;
        public string[] floatNames;
        public float[] floatValues;
        public int intCount;
        public int[] intNames;
        public int[] intValues;
        public int stringCount;
        public int stringNames;
        public int stringValues;

        public JSONHelperObject(string name, string level)
        {
            this.name = name;
            this.level = level;
            //this.dataObj = dataObj;
            floatNames = new string[] {"TestA", "TestB"};
            floatValues = new float[] {0.5f, 1.0f};
        }
    }

    [System.Serializable]
    public class JSONFloatHelperObject
    {
        public string name;
        public float value;
    }

    [System.Serializable]
    public class JSONIntHelperObject
    {
        public string name;
        public int value;
    }

    [System.Serializable]
    public class JSONStringHelperObject
    {
        public string name;
        public string value;
    }



    [System.Serializable]
    private class TestObject
    {
        public string name = "Yuh";
        public int value = 15;
        public int value2 = 25;
        public InternalObj inside;
    }

    [System.Serializable]
    private class InternalObj
    {
        public string title = "This is inside";
        public float miniValue = 0.356124125325f;
    }
}
