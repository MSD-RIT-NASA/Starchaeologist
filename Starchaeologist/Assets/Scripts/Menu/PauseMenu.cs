using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] private InputActionReference pauseActionRef;
    [SerializeField] GameObject playerReference;

    private void OnEnable()
    {
        Debug.Log($"PauseMenu on {name}: pauseActionRef = {pauseActionRef}");
        pauseActionRef.action.performed += OnPauseAction;
    }
    private void OnDisable()
    {
        pauseActionRef.action.performed -= OnPauseAction;
    }

    private void OnPauseAction(InputAction.CallbackContext ctx)
    {
        Debug.Log("pause action performed");
        if (GameIsPaused)
            Resume();
        else
            Pause();
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
        Vector3 spawnPos = playerReference.transform.position + (tempPlayer.transform.forward * 1.5f);
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
