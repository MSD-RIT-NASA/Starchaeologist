using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Send_Me_Back : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene(0);
    }

}
