using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class ScoreData : MonoBehaviour
{
    public Canvas scoreCanvas;
    public TextMeshPro score;
    public TMP_InputField name;

    private StreamReader reader;
    private StreamWriter writer;

    // Start is called before the first frame update
    void Start()
    {
        scoreCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetScoreCanvasActive(bool isActive)
    {
        scoreCanvas.enabled = isActive;
    }
}
