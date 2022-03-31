using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    //public AudioSource buttonClick;
    //public AudioClip click;

    [SerializeField] private InputActionReference pauseActionRef;

    private void OnEnable()
    {
        pauseActionRef.action.performed += OnPauseAction;
    }
    private void OnDisable()
    {
        pauseActionRef.action.performed -= OnPauseAction;
    }

    private void OnPauseAction(InputAction.CallbackContext ctx)
    {
        if (GameIsPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void MainMenu()
    {
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
