using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class MiniSceneLoader : MonoBehaviour
{

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard.rKey.isPressed)
        {
            LoadScene(0);
        }
    }

    /// <summary>
    /// Load a desired scene based on its build index 
    /// </summary>
    /// <param name="sceneId"></param>
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    /// <summary>
    /// Reload the current scene 
    /// </summary>
    public void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
