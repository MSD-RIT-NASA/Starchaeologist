/*
Created by Youssef Elashry to allow two-way communication between Python3 and Unity to send and receive strings

Feel free to use this in your individual or commercial projects BUT make sure to reference me as: Two-way communication between Python 3 and Unity (C#) - Y. T. Elashry
It would be appreciated if you send me how you have used this in your projects (e.g. Machine Learning) at youssef.elashry@gmail.com

Use at your own risk
Use under the Apache License 2.0

Modified by: 
Youssef Elashry 12/2020 (replaced obsolete functions and improved further - works with Python as well)
Based on older work by Sandra Fang 2016 - Unity3D to MATLAB UDP communication - [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
*/

// NASA x RIT author: Angela Hudak

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;


public class UdpSocket : MonoBehaviour
{
        //Game Mode
    /*
     * Main Menu = 0
     * River Ride = 1
     * Puzzling Times = 2
     * Minecart = 3
     */

    public int gameMode = 0;

    //thread
    bool threadRunning = false;
    Thread communicateThread;

    //score
    bool isCalibrated = false;
    private bool calibrateRig = false;
    float getBalanceScore = -1f;

    //killswitch
    bool killIt = false;

    // Game data
    bool gameOver = false;
    private bool gameStart = false;
    bool gamePaused = false;
    bool txPlatformMovement = false;
    //string gameProfiles = null;

    // TESTING
    public bool test = false;
    float fltTest = -1f;

    // ROTATION BOARD SENSOR
    private float boardRotation;

    [HideInInspector] public bool isTxStarted = false;

    [SerializeField] string IP = "127.0.0.1"; // local host
    [SerializeField] int rxPort = 8000; // port to receive data from Python on
    [SerializeField] int txPort = 8001; // port to send data to Python on

    // Create necessary UdpClient objects
    UdpClient client;
    IPEndPoint remoteEndPoint;
    Thread receiveThread; // Receiving Thread
    
    [SerializeField]
    private MineGame mineLevel;

    [SerializeField]
    private TMP_Text balanceScoreDisplay;

    static public int Score;

    [SerializeField]
    private ScoreData scoreMgr;
    
    public bool CalibrateRig
    {
        get { return calibrateRig; }
        set { calibrateRig = value; }
    }

    public bool GameStart
    {
        get { return gameStart; }
        set { gameStart = value; }
    }
    public bool GameOver
    {
        get { return gameOver; }
        set { gameOver = value; }
    }
    public float BoardRotation
    {
        get { return boardRotation; }
    }
        public float BalanceScore
    {
        get { return getBalanceScore; }
    }

    ////////////////////////////////////////////////////////////////////////////////

    public void SendData(string message) // Use to send data to Python
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    void Awake()
    {
        // Create remote endpoint (to Matlab) 
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), txPort);

        // Create local client
        client = new UdpClient(rxPort);

        // local endpoint define (where messages are received)
        // Create a new thread for reception of incoming messages
        threadRunning = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        communicateThread = new Thread(Communicate);
        communicateThread.IsBackground = true;
        communicateThread.Start();

        receiveThread.IsBackground = true;
        receiveThread.Start();


        // Initialize (seen in comments window)
        print("UDP Comms Initialised");

    }

    // Receive data, update packets received
    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                print(">> " + text);
                ProcessInput(text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    //this method will be threaded and handles reading messages with the python server
    private void ProcessInput(string input)
    {
        // PROCESS INPUT RECEIVED STRING HERE

        if (!isTxStarted) // First data arrived so tx started
        {
            isTxStarted = true;
        }
        //split the string into two floats
        string[] splitMessage = input.Split(' ');

        //figure out what to do with the message
        switch (splitMessage[0])
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
                }
                break;

            case "quit":
                Debug.Log("received 'quit'");
                threadRunning = false;
                Application.Quit(); 
                return;

            case "calibratedRigsuccess":
                // when the message calibrated is received then 
                Debug.Log("Sensors calibrated");
                isCalibrated = true;
                calibrateRig = false;
                CalibrateRig = false;
                // then allow user to step on platform
                // game start AFTER isCalibrated is true
                break;

            case "calibratedRigFailed":
                Debug.Log("Sensors not calibrated"); 
                isCalibrated = false;
                StopThread();
                break;
            
            case "balanceScore":
                Debug.Log("Collecting balance score");
                getBalanceScore = float.Parse(splitMessage[1]);
                Debug.Log(getBalanceScore.ToString());
                float gameScore = Score + getBalanceScore;
                balanceScoreDisplay.text = "" + gameScore;
                scoreMgr.DetermineRank(gameScore);//Determines a letter rank based on the score and displays it
                
                StopThread();
                break;

            case "boardMove":
                //Debug.Log("board moved!");
                //collect board data
                boardRotation = float.Parse(splitMessage[1]);
                //Debug.Log(boardRotation.ToString());

                //If multiple sensor values will be read in as parts of a single string,
                //split the messageat whitespace and assign sensorLRot and sensorRRot respectively
                break;

//////////////////////////////////////////////////////////////////////
////////////////////////////// TESTING ///////////////////////////////
//////////////////////////////////////////////////////////////////////

            case "testingPython":
                fltTest = float.Parse(splitMessage[1]);
                Debug.Log(fltTest.ToString());
                StopThread();
                break;

            case "longStringVerified":
                Debug.Log("Test long string working"); 
                StopThread();
                break;


            default:
                break;

        }
            
    }

    //Prevent crashes - close clients and threads properly!
    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if(!threadRunning)
        {
            threadRunning = true;
            StartThread();
        }

    }

    void StartThread()
    {
        threadRunning = true;
        communicateThread = new Thread(Communicate);
        communicateThread.Start();
    }

     void StopThread()
    {
        threadRunning = false;
        // block main thread, wait for _runnerThread to finish its job first, so we can be sure that 
        // _runnerThread will end before main thread end
        communicateThread.Join();
    }

    //this method will be threaded and handles sending messages with the python server
    // note: unity has a hard time sending multiple SendData(string) in one case. 
    //       Instead, send one long string and parse the string on server side
    //       Ex: "hello 1234 world 5678" or "gameMode 2 gameProfile Jerry test 123"
    public void Communicate()
    {
        if (threadRunning)
        {
            //send messages

                if(gameStart) // the game starts
            {
                // for levels 1 and 2 this must be called AFTER confirmation of calibration
                //if (isCalibrated == true){}
                Debug.Log("Game start");
                string msg = "gameStart";
                SendData(msg);
                // send game mode
                // string serverGameMode = gameMode.ToString();
                // SendData(serverGameMode);

                // send over gameProfile data so MATLAB data is correctly labeled
                // // string serverProfile = gameProfile.ToString();
                // // SendData(gameProfile);

                // start collecting balance data 
                SendData("collectBalanceData");

                string deadTime = "" + mineLevel.DeadTime;
                SendData(deadTime);

                gameStart = false;
                return;

            }

                else if(gameOver) // the game ends
                {
                    Debug.Log("Game Over");
                    // stop acctuators
                    // get the balance score
                    // close the server?
                    // end the game
                    string msg = "gameOver";
                    SendData(msg);
                    
                    gameOver = false;
                    return;
                }

            // when the game is paused, it must pause receiving / collecting sensor data, 
            // then recalibrate, then start collecting agin
            else if(gamePaused)
            {
                return; 
            }

            else if(txPlatformMovement) // Game 2 when sending different floor movements
            {

                return;
            } 


            else if(calibrateRig) 
            {
                Debug.Log("startCalibrating");
                string msg = "startCalibrating";
                SendData(msg);

                calibrateRig = false;
                CalibrateRig = false;
                return;
            }

//////////////////////////////////////////////////////////////////////
////////////////////////////// TESTING ///////////////////////////////
//////////////////////////////////////////////////////////////////////

            else if(test)
            {
                Debug.Log("sending message1 test");
                string msg = "testing";
                SendData(msg);

                Debug.Log("sending message2 test");
                string msg2 = "testing2 123";
                SendData(msg2);

                Debug.Log("sending message3 test");
                string msg3 = "hello 1234 world 5678";
                SendData(msg3);
            
                test = false;
                return;
            } 
                
        }
        threadRunning = false;   
    }

}