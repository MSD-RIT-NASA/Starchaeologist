using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGameManager : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    #region GAME_MANAGER

    TempleGameStates gameState = TempleGameStates.CHOOSE_GAME_MODE;

    /// <summary>
    /// State machine that runs the temple game modes 
    /// </summary>
    void TempleSM() 
    {
        switch (gameState)
        {
            case TempleGameStates.CHOOSE_GAME_MODE:
                break;
            case TempleGameStates.POSE_MATCH:
                break;
            case TempleGameStates.FOUR_SQUARE:
                FourSquareSM();
                break;
            case TempleGameStates.DISPLAY_SCORE:
                break;
        }
    }

    /// <summary>
    /// Changes the visuals around the player to show the score of the
    /// desired gamemode
    /// </summary>
    /// <param name="game"></param>
    void DisplayScore(GameModes game) { } 

    enum TempleGameStates
    { 
        CHOOSE_GAME_MODE,
        POSE_MATCH,
        FOUR_SQUARE,
        DISPLAY_SCORE
    }

    enum GameModes
    {
        POSE_MATCH,
        FOUR_SQUARE
    }

    #endregion

    #region FOUR_SQUARES

    FourSquareStates fourSquareState = FourSquareStates.GENERATE_PATTERNS;

    // Arr of indexes each refer to differnt square 
    int[] pattern;

    /// <summary>
    /// Manages the states of the Four Square game-mode 
    /// </summary>
    void FourSquareSM() 
    { 
        switch (fourSquareState)
        {
            case FourSquareStates.GENERATE_PATTERNS:
                break;
            case FourSquareStates.DISPLAY_PATTERN:
                break;
            case FourSquareStates.PLAY:
                PlayState();
                break;
            case FourSquareStates.END_GAME:
                break;
        }
    }

    /// <summary>
    /// Creates an array of indexes that each represent one of the squares that the player must step to. 
    /// There cannot be the same two indexes in a row. 
    /// </summary>
    /// <returns></returns>
    int[] GeneratePattern()
    {
        return null;
    }

    /// <summary>
    /// Shows the current square that the player must travel to. 
    /// </summary>
    void DisplayPattern() 
    { 
    
    }

    /// <summary>
    /// Dictates when the player has reached the desired square 
    /// </summary>
    void PlayState()
    {

    }

    /// <summary>
    /// Detects whether the player is fully within the desired target. 
    /// This is done by checking if the feet, hands, and head occupy the desired area.
    /// </summary>
    /// <returns></returns>
    bool PlayerInTarget()
    {
        return false;
    }

    /// <summary>
    /// Returns true if the the entire pattern has been completed 
    /// </summary>
    /// <returns></returns>
    bool RoundComplete()
    {
        return false;
    }

    enum FourSquareStates
    {
        GENERATE_PATTERNS,  // Setup for the game round 
        DISPLAY_PATTERN,    // Indicates the current square 
        PLAY,               // Grades player score and waits for their input 
        END_GAME            // Cleanup and send to display score 
    }

    #endregion

    #region POSE_COPY

    /// <summary>
    /// Randomly chooses a pose from a list of poses that is not the same as the previous pose.
    /// </summary>
    /// <returns></returns>
    Pose SelectPose()
    {
        return null;
    }

    /// <summary>
    /// Animated and visualizes the pose that was chosen. 
    /// Indicates what the general pose the player should make.
    /// </summary>
    void PlaySelection()
    {

    }

    /// <summary>
    /// Visualizes how close the player is to matching the pose. 
    /// Changes the hands and feet of intstructor by having a green check, yellow minus, or red cross. 
    /// </summary>
    void DisplayPoseCloseness()
    {

    }

    /// <summary>
    /// Resets the display so that there is no more pose. A cleanup function
    /// </summary>
    void ResetPoseGame()
    {

    }

    [System.Serializable]
    private class Pose
    {
        [SerializeField] private Vector3 hand_L, hand_R, foot_L, foot_R;

        /// <summary>
        /// Passes in the current player’s limb positions and returns an array of enums that represent how close they are to matching the pose. 
        /// </summary>
        /// <returns>Array returned is of size 4 and has ratings in order from {hand_L, hand_R, foot_L, foot_R}</returns>
        MatchLevel[] GetPoseMatchLevels(Vector3 hand_L, Vector3 hand_R, Vector3 foot_L, Vector3 foot_R)
        {
            return null;
        }
    }

    enum MatchLevel
    { 
        NOT_MATCHED,
        CLOSE,
        MATCHED
    }


    #endregion

}
