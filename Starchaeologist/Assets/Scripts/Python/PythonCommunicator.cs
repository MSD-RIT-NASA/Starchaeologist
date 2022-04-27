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

/*Description

    This script is attached to the Game manager and handles communication with the 
    external python server which communicates to the motion floor and back to this.
    This will create a thread ~every frame which sends a message and tries to recieve
    a message. For the most part this will be in the form of a rotation which will be
    sent back to the relative platforms via the game manager script. There is also the 
    intent to stop the game when the kill switch is hit, receive balance score data, 
    and quit game signals. The killswitch/pause still needs full implementation.
    This was put together with the help from a public GIT repository here:
    https://github.com/off99555/Unity3D-Python-Communication
 
 */

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

    //stop the thread if the script is destroyed
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

    //this method will be threaded and handles sending, receiving, and reading messages with the python server
    void Communicate()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5678");

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
                while(!gotMessage)
                {
                    gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
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

    //if the message isn't one of the one word commands, assume it's score or rotation and split it up
    void SplitMessage(string message)
    {
        //split the string into two floats
        string[] splitMessage = message.Split(' ');

        /*TODO
         -the final balance score will probably end up being multiple variables. read them accordingly
         */
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
