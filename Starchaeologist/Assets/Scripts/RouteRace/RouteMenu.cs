using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RouteMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn1 = GameObject.Find("Quit").GetComponent<Button>();
        btn1.onClick.AddListener(LoadMenu);

        Button btn2 = GameObject.Find("PlayAgain").GetComponent<Button>();
        btn2.onClick.AddListener(ReloadLevel);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene("RouteRace");
    }
}
