using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreScript : MonoBehaviour
{
    private Text txt;
    private Text txtMenu;
    private float showTime = 1f;
    private float hideTime = 0f;
    public static int Score;
    public static bool scoreMenu=false;
    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        txt= GameObject.Find("Score").GetComponent<Text>();
        txt.enabled = false;
        txtMenu= GameObject.Find("ScoreMenu").GetComponent<Text>();
        txtMenu.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(txt.enabled && (Time.time >= hideTime)&& !scoreMenu)
        {
            txt.enabled = false;
        }
        if(scoreMenu){
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
    }
    void vignetteFade()
    {

    }
    void txtVisual(int points)
    {
        if (points == 10)
        {
            txt.text = "- "+points.ToString();
            txt.color = Color.red;
        }
        else
        {
            txt.text = "+ " + points.ToString();
            txt.color = Color.black;
        }
        
        txt.enabled = true;
        hideTime = Time.time + showTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHead")){

            if (gameObject.CompareTag("Treasure"))
            {
                treasureScore();
            }else if (gameObject.CompareTag("Artifact"))
            {
                artifactScore();
            }else if (gameObject.CompareTag("Obstacle")) 
            {
                hitScore();
            }
        }
        //when the player first comes into contact with the end block, show the score screen
        if (other.gameObject.CompareTag("PlayerBody")&&gameObject.CompareTag("Finish"))
        {
            scoreMenu = true;
            for(int s=0;s<Score;s++){
                txtMenu.text = "Game Score:\n"+s.ToString()+"\n\nPlayer Stats:\n";//+communication player score
            }
            
        }
        
    }
}
