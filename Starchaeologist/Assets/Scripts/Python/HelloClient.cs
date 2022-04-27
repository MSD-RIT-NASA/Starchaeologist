using UnityEngine;

//See: https://github.com/off99555/Unity3D-Python-Communication

public class HelloClient : MonoBehaviour
{
    private HelloRequester _helloRequester;

    private void Start()
    {
        _helloRequester = new HelloRequester();
        _helloRequester.StartThread();
    }

    private void OnDestroy()
    {
        _helloRequester.Stop();
    }
}