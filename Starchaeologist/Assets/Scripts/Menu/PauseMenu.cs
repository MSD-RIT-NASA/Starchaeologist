using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{

    //singleton
    public static PauseMenu singleton;


    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] GameObject playerReference;

    private void Start()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
    }

    private void UpdateMenuPosition()
    {
        GameObject tempPlayer = new GameObject();
        tempPlayer.transform.position = playerReference.transform.position;
        Quaternion tempRotation = playerReference.transform.rotation;
        tempRotation.x = 0;
        tempRotation.z = 0;
        tempPlayer.transform.rotation = tempRotation;
        Destroy(tempPlayer);
        Vector3 spawnPos = playerReference.transform.position + (tempPlayer.transform.forward * 1.5f) + new Vector3(0,1,0);
        pauseMenuUI.transform.position = spawnPos;
    }


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        UpdateMenuPosition();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void MainMenu()
    {
        pauseMenuUI.SetActive(false);
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void Restart()
    {

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("RiverRide"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("PuzzlingTimes"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Resume();
        }

    }

    public void ChangeLevel()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("RiverRide"))
        {
            SceneManager.LoadScene(SceneManager.GetSceneByName("PuzzlingTimes").buildIndex);
        }

        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("PuzzlingTimes"))
        {
            SceneManager.LoadScene(SceneManager.GetSceneByName("RiverRide").buildIndex);
        }
        

    }

    public void QuitGame()
    {
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
