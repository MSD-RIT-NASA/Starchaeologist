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
//    -Send: 'calibrate gameScore(float) gameMode(int)'
//    - Receive: 'calibrateStop balanceScore(float)'
//    Killswitch
//    - Receive 'kill'
//     - Receive 'live'
//    Quit Game
//    -Send 'quit'
//    -Receive 'quit'

public class PythonCommunicator : MonoBehaviour
{
    //Game Mode
    /*
     * Main Menu = 0
     * River Ride = 1
     * Puzzling Times = 2
     */
    public int gameMode = 0;

    //thread
    bool threadRunning = false;
    Thread communicateThread;

    //rotation
    public Vector2 desiredRotation = new Vector2(0, 0);
    public Vector2 realRotation = new Vector2(0, 0);

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
            StartThread();
        }

        if(comeBack)
        {
            /*TO DO*/
            //allow the game to be unpaused after it's been killed
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
                    quitGame = false;
                    Debug.Log("Quit Game");
                    threadRunning = false;
                    NetMQConfig.Cleanup();
                    //TO DO
                    //quit the game
                    Application.Quit();
                    return;
                }
                else if(transferScore)//send the score
                {
                    Debug.Log("Sending Score");
                    transferScore = false;
                    string giveScore = "calibrate " + gameScore + " " + gameMode;
                    client.SendFrame(giveScore);
                }
                else//send the desired rotation
                {
                    //Debug.Log("Sending Rotation");
                    string giveRotation = "rotation " + desiredRotation.x + " " + desiredRotation.y;
                    client.SendFrame(giveRotation);
                }


                //receive messages
                string message = null;
                bool gotMessage = false;
                gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful

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
                            Application.Quit();
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
        //split the string into two floats
        string[] splitMessage = message.Split(' ');

        if (splitMessage[0] == "calibrateStop")//get the score data back and do something with it
        {
            balanceScore = float.Parse(splitMessage[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }
        else if(splitMessage[0] == "rotation")//get the rotation back and set it
        {
            float xRotation = float.Parse(splitMessage[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            float zRotation = float.Parse(splitMessage[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            //The python sends negatives back as larger angles, this turns them back to negatives
            if (xRotation > 180f)
            {
                xRotation = xRotation - 360f;
            }
            if (zRotation > 180f)
            {
                zRotation = zRotation - 360f;
            }

            //set the rotation of the raft to the given rotation
            realRotation = new Vector2(xRotation, zRotation);

            Debug.Log("Received: " + "xRotation(" + xRotation + "), zRotation(" + zRotation + ")");
        }
    }
}
