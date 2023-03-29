using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionTest : MonoBehaviour
{
    private Text txt;
    private Text txtBlip;
    private float showTime = 1f;
    private float hideTime = 0f;
    private float calibrateOn = 30f;
    private float calibrateOff = 0f;
    public static int Score;
    public static bool scoreMenu = false;
   // public GameObject vignette;
    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        txt = GameObject.Find("Score").transform.GetChild(0).gameObject.GetComponent<Text>();
        txt.text = "Score: " + Score;
        //txt.enabled = false;
        txtBlip = GameObject.Find("Score").transform.GetChild(1).GetComponent<Text>();
        txtBlip.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (txtBlip.enabled && (Time.time >= hideTime))
        {
            txtBlip.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Obstacle"))
        {
            hitScore();
            Debug.Log("HIT");
        }

    }
    public void hitScore()
    {
        Score -= 2;
        txt.text = "Score: " + Score;
        txtVisual(2);
        Debug.Log(Score);
        //enable vignette
        //vignetteOn();
        //Invoke("vignetteOff", 3.0f); //set inactive after 3 seconds have passed
    }

    //void vignetteOn()
    //{
    //    vignette.SetActive(true);
    //}

    //void vignetteOff()
    //{
    //    vignette.SetActive(false);
    //}
    void txtVisual(int points)
    {
        if (points == 2)
        {
            txtBlip.text = "- " + points.ToString();
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
}
