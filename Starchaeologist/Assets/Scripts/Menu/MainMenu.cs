using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource buttonClick;
    public AudioClip click;

    void Start()
    {
        buttonClick = GetComponent<AudioSource>(); 

        Button btn1 = GameObject.Find("RiverRide").GetComponent<Button>();
        btn1.onClick.AddListener(Level1);

        Button btn2 = GameObject.Find("PuzzlingTimes").GetComponent<Button>();
        btn2.onClick.AddListener(Level2);
    }

    public void Level1()
    {
        buttonClick.PlayOneShot(click);
        SceneManager.LoadScene(1);
    }

    public void Level2()
    {
        buttonClick.PlayOneShot(click);
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        buttonClick.PlayOneShot(click);
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == true)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else 
#endif

        Application.Quit();
        
    }
}
