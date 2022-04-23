using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreScript : MonoBehaviour
{
    public static scoreScript singleton;

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
    // Start is called before the first frame update
    void Start()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }

        Score = 0;
        txt= GameObject.Find("Score").GetComponentInChildren<Text>();
        txt.enabled = false;
        txtBlip= GameObject.Find("ScoreBlip").GetComponentInChildren<Text>();
        txtBlip.enabled = false;
        txtMenu= GameObject.Find("ScoreMenu").GetComponent<Text>();
        txtMenu.enabled = false;
        txtCalibration= GameObject.Find("CalibrationCanvas").GetComponent<Text>();
        txtCalibration.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {        
        if(txtBlip.enabled && (Time.time >= hideTime)&& !scoreMenu)
        {
            txtBlip.enabled = false;
        }
        if(scoreMenu){ //set to true when playerFoot collides with "finish" tag
            txtMenu.enabled = true;
        }
        
    }
    public void artifactScore()
    {
        Score += 100;
        txtVisual(100);
    }
    public void treasureScore()
    {
        Score += 20;
        txtVisual(20);
    }
    public void hitScore()
    {
        Score -= 10;
        txtVisual(10);
        //enable vignette
        vignetteOn();
        Invoke("vignetteOff", 3.0f); //set inactive after 3 seconds have passed
    }

    void vignetteOn()
    {
        vignette.SetActive(true);
    }

    void vignetteOff()
    {
        vignette.SetActive(false);
    }
    void txtVisual(int points)
    {
        if (points == 10)
        {
            txtBlip.text = "- "+points.ToString();
            txtBlip.color = Color.red;
        }
        else
        {
            txtBlip.text = "+ " + points.ToString();
            txtBlip.color = Color.black;
        }
        
        txtBlip.enabled = true;
        hideTime = Time.time + showTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHead")){

           if (gameObject.CompareTag("Obstacle")) 
            {
                hitScore();
            }
        }
        if (other.gameObject.CompareTag("PlayerBody")){

            if (gameObject.CompareTag("Treasure"))
            {
                treasureScore();
            }else if (gameObject.CompareTag("Artifact"))
            {
                artifactScore();
            }
        }
        //when the player first comes into contact with the end block, show the score screen
        if (other.gameObject.CompareTag("PlayerFoot")&&gameObject.CompareTag("Finish"))
        {
            txtCalibration.enabled=true;
            Invoke("ShowScoreMenu", 30f);
        }
        
    }

    void ShowScoreMenu()
    {
        txtCalibration.enabled=false;
        scoreMenu = true;
        for(int s=0;s<Score;s++){
            txt.text = s.ToString();//+communication player score
        }
    }
}
