using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

//PYTHON COMMUNICATION FORMAT
//    Rotation
//    -Send: 'rotation rotation1(float) rotation2(float)'
//    - Receive: 'rotation rotation1(float) rotation2(float)'
//    Score Calibration 
//    -Send: 'calibrate gameScore(float)'
//    - Receive: 'calibrateStop balanceScore(float)'
//    Killswitch
//    - Receive 'kill'
//     - Receive 'live'
//    Quit Game
//    -Send 'quit'
//    -Receive 'quit'

public class PythonCommunicator : MonoBehaviour
{
    //thread
    bool threadRunning = false;
    Thread communicateThread;

    //rotation
    public Vector2 desiredRotation = new Vector2(0, 0);
    Vector2 realRotation = new Vector2(0, 0);

    //score
    bool transferScore = false;
    float gameScore = -1f;
    float balanceScore = -1f;

    //killswitch
    bool killIt = false;
    bool comeBack = false;

    //Quit game
    bool quitGame = false;

    void OnDestroy()
    {
        Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(!threadRunning)
        {
            threadRunning = true;
            Communicate();
        }
    }

    void StartThread()
    {
        threadRunning = true;
        communicateThread = new Thread(Communicate);
        communicateThread.Start();
    }

    void Stop()
    {
        threadRunning = false;
        // block main thread, wait for _runnerThread to finish its job first, so we can be sure that 
        // _runnerThread will end before main thread end
        communicateThread.Join();
    }

    void Communicate()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");

            if (threadRunning)
            {
                //send messages
                if(quitGame)//the game is quitting
                {
                    Debug.Log("Quit Game");
                    threadRunning = false;
                    NetMQConfig.Cleanup();
                    //TO DO
                    //quit the game
                    return;
                }
                else if(transferScore)//send the score
                {
                    Debug.Log("Sending Score");
                    transferScore = false;
                }
                else//send the desired rotation
                {
                    Debug.Log("Sending Rotation");
                    string giveRotation = "rotation " + desiredRotation.x + " " + desiredRotation.y;
                    client.SendFrame(giveRotation);
                }


                //receive messages
                string message = null;
                bool gotMessage = false;
                while (threadRunning)
                {
                    gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                    if (gotMessage) break;
                }

                if (gotMessage)
                {
                    //figure out what to do with the message
                    switch (message)
                    {
                        case "kill":
                            Debug.Log("received 'kill'");
                            killIt = true;
                            break;
                        case "live":
                            Debug.Log("received 'live'");
                            if(killIt)
                            {
                                killIt = false;
                                comeBack = true;
                            }
                            break;
                        case "quit":
                            Debug.Log("received 'quit'");
                            threadRunning = false;
                            NetMQConfig.Cleanup();
                            return;
                        default://the message is either about score or rotation
                            SplitMessage(message);
                            break;
                    }
                }
                threadRunning = false;
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    void SplitMessage(string message)
    {

    }
}
