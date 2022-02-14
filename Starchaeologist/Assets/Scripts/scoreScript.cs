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
        
        if(txt.enabled && (Time.time >= hideTime))
        {
            txt.enabled = false;
        }
        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            artifactScore();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            treasureScore();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hitScore();
        }
        */
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
}
