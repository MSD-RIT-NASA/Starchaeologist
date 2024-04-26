using System.Collections;
using System.Collections.Generic;
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


    public void LoadScene(int sceneId)
    {
        print("Loading scene");
        SceneManager.LoadScene(sceneId);
    }
}
