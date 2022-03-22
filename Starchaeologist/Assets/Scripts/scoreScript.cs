using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreScript : MonoBehaviour
{
    private Text txt;
    private float showTime = 1f;
    private float hideTime = 0f;
    public static int Score;
    public static bool scoreMenu=false;
    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        txt= gameObject.GetComponent<Text>();
        txt.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(txt.enabled && (Time.time >= hideTime)&& !scoreMenu)
        {
            txt.enabled = false;
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
        if (other.gameObject.CompareTag("Treasure"))
        {
            treasureScore();
        }else if (other.gameObject.CompareTag("Artifact"))
        {
            artifactScore();
        }else if (other.gameObject.CompareTag("obstacle")) //possible name for tag
        {
            hitScore();
        }
        //when the player first comes into contact with the end block, show the score screen
        if (other.gameObject.CompareTag("PlayerBody"))
        {
            scoreMenu = true;
        }
    }
}
