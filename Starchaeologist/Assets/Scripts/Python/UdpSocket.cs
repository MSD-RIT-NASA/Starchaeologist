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

    //rotation

    // int getMovement
    // int sendMovement


    //score
    bool isCalibrated = false;
    public bool calibrateRig = false;
    float getBalanceScore = -1f;

    //killswitch
    bool killIt = false;

    //Quit game
    bool gameOver = false;
    private bool gameStart = false;
    bool gamePaused = false;
    bool txPlatformMovement = false;
    //string gameProfiles = null;

    [HideInInspector] public bool isTxStarted = false;

    [SerializeField] string IP = "127.0.0.1"; // local host
    [SerializeField] int rxPort = 8000; // port to receive data from Python on
    [SerializeField] int txPort = 8001; // port to send data to Python on

    // Create necessary UdpClient objects
    UdpClient client;
    IPEndPoint remoteEndPoint;
    Thread receiveThread; // Receiving Thread

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

        //figure out what to do with the message
        switch (input)
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
                    //comeBack = true;
                }
                break;
            case "quit":
                Debug.Log("received 'quit'");
                threadRunning = false;
                //NetMQConfig.Cleanup();
                Application.Quit();
                return;
            case "calibratedRigsuccess":
                // when the message calibrated is received then 
                Debug.Log("Sensors calibrated");
                isCalibrated = true;
                // then allow user to step on platform
                //game start AFTER isCalibrated is true
                break;
            case "calibratedRigFailed":
                Debug.Log("Sensors not calibrated");
                isCalibrated = false;
                break;
            
            case "balanceScore":
                Debug.Log("Collecting balance score");
                // somehow collect the balance score?
                //getBalanceScore = 
                break;

            case "boardMove":
                Debug.Log("board moved!");
                //collect board data
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

    //this method will be threaded and handles sending messages with the python server
    void Communicate()
    {


            if (threadRunning)
            {
                //send messages


                if(gameStart) // the game starts
                {
                    // for levels 1 and 2 this must be called AFTER confirmation of calibration
                    //
                    // send over gameProfile data so MATLAB data is correctly labeled
                    // start collecting balance data 
                    
                }

                else if(gameOver) // the game ends
                {
                    Debug.Log("Game Over");
                    // stop acctuators
                    // get the balance score
                    // close the server,,,,, i think
                    // end the game


                    Application.Quit();

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

                    return;
                }

                // else if(gameMode)
                // {
                //     //send over the game mode
                //     return;
                // }

                

                // else//send the desired rotation
                // {
                //     //Debug.Log("Sending Rotation");
                //     string giveRotation = "rotation " + desiredRotation.x + " " + desiredRotation.y;
                //     client.SendFrame(giveRotation);
                // }
              
            
                
            }
            threadRunning = false;
        
            
        

    }


}