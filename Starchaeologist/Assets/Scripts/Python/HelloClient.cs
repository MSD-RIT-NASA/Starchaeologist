using UnityEngine;

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