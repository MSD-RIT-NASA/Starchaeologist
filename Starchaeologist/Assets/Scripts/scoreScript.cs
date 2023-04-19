using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using TMPro;

public class scoreScript : MonoBehaviour
{
    public static scoreScript Instance{ get; private set;}

    private Text txt;
    private Text txtBlip;
    private Text txtMenu;
    private Text txtCalibration;

    
    private float showTime = 1f;
    private float hideTime = 0f;
    private float calibrateOn = 30f;
    private float calibrateOff = 0f;
    public static int Score;
    public static bool scoreMenu=false;
    public GameObject vignette;
    //public PythonCommunicator pythCom;
    // Start is called before the first frame update
    void Start()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Debug.Log("Instance Set");
            Instance = this;
        }
    }
    
    public void artifactScore()
    {
        Score += 100;
    }
    public void treasureScore()
    {
        Score += 2;
    }
    public void hitScore()
    {
        Score -= 2;
        Debug.Log("lower the score");
    }

    void vignetteOn()
    {
        vignette.SetActive(true);
    }

    void vignetteOff()
    {
        vignette.SetActive(false);
    }
}
