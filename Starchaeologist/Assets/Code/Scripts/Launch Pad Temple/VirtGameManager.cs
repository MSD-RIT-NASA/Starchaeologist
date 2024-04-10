using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purpose of this is to act as a general game manager
/// that can be used to quickly make a game manager. 
/// </summary>
public abstract class VirtGameManager : MonoBehaviour
{
    // Information all game managers should have 
    [SerializeField] protected Transform playerHead;
    [SerializeField] protected Transform playerHandLeft; 
    [SerializeField] protected Transform playerHandRight;  
    [SerializeField] protected Transform playerAnkleLeft;
    [SerializeField] protected Transform playerAnkleRight;

    protected bool gameOver; 

    /// <summary>
    /// Setup this game manager with player data and other setup. This
    /// is called 
    /// </summary>
    public abstract void Init(
        Transform playerHead, 
        Transform playerHandLeft, 
        Transform playerHandRight, 
        Transform playerAnkleLeft, 
        Transform playerAnkleRight);

    /// <summary>
    /// Run this game manager's logic and allow it to manage itself 
    /// </summary>
    public abstract void StateMachine();

    /// <summary>
    /// Render this game's gizmos and debug information. 
    /// Must be called in Unity's DrawGizmos functions 
    /// </summary>
    public abstract void GizmosVisuals();

    /// <summary>
    /// Writes to DSL to store data 
    /// </summary>
    /// <param name="name"></param>
    public abstract void WriteDataToFile(string name, DataStoreLoad dsl);

    /// <summary>
    /// Reset game manager's variables for a new round 
    /// </summary>
    public abstract void ResetGame();

    /// <summary>
    /// Cleanup all the assets that were created within the world 
    /// </summary>
    public abstract void CleanupGame();

    /// <summary>
    /// Returns whether or not this game is over 
    /// </summary>
    /// <returns></returns>
    public abstract bool IsGameDone();
}
