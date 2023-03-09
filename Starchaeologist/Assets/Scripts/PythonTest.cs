using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PythonTest : MonoBehaviour
{
    [SerializeField] TMP_Text pythonRcvdText = null;
    [SerializeField] TMP_Text sendToPythonText = null;

    string tempStr = "Sent from Python xxxx";
    int numToSendToPython = 0;
    UdpSocket udpSocket;

    public void QuitApp()
    {
        print("Quitting");
        Application.Quit();
    }

    public void UpdatePythonRcvdText(string str)
    {
        //This string should be parsed as rotation data and possibly filepaths for data images
        tempStr = str;
    }

    public void SendToPython()
    {
        udpSocket.SendData("Sent From Unity: " + numToSendToPython.ToString());
        numToSendToPython++;
        sendToPythonText.text = "Send Number: " + numToSendToPython.ToString();
    }

    private void Start()
    {
        udpSocket = FindObjectOfType<UdpSocket>();
        sendToPythonText.text = "Send Number: " + numToSendToPython.ToString();
    }

    void Update()
    {
        pythonRcvdText.text = tempStr;
    }
}
