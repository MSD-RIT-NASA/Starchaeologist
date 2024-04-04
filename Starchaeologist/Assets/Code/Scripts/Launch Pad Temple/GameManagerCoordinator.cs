using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerCoordinator : MonoBehaviour
{
    [Header("Games")]
    [SerializeField] List<VirtGameManager> gameManagers;
    [SerializeField] int currentGame;

    [Header("UI")]
    [SerializeField] GameObject instructionCanvas;

    [Header("Player")]
    [SerializeField] Transform playerHead;
    [SerializeField] Transform playerHandLeft;
    [SerializeField] Transform playerHandRight;
    [SerializeField] Transform playerAnkleLeft;
    [SerializeField] Transform playerAnkleRight;

    private CoordinatorState state;
    private VirtGameManager game;

    private void Update()
    {
        game = gameManagers[currentGame];


        StateMachine();
    }

    public void StateMachine()
    {
        switch (state)
        {
            case CoordinatorState.CHOOSE_GAME_MODE:
                ChooseGameMode();
                break;
            case CoordinatorState.CURRENT_GAME_INIT:
                GameInit();
                break;
            case CoordinatorState.CURRENT_GAME_RUN:
                GameRun();
                break;
            case CoordinatorState.CURRENT_GAME_RESET:
                GameReset();
                break;
            case CoordinatorState.CURRENT_GAME_CLEANUP:
                GameCleaup();
                break;
        }
    }

    /// <summary>
    /// Opens up UI that allows the player to select a game. This can be done 
    /// while game assets are still loaded in 
    /// </summary>
    void ChooseGameMode()
    {
        // Is there already game assets active? 

        // Determine if there is a game that needs 
        // to be cleaned up. 

        // If so then reset game 

        // Otherwise just go to 
    }

    /// <summary>
    /// Initialize a game from its start 
    /// </summary>
    void GameInit()
    {
        // Game will manage if assets are already created 
        game.Init(
            playerHead, 
            playerHandLeft, 
            playerHandRight, 
            playerAnkleLeft, 
            playerAnkleRight);
    }

    /// <summary>
    /// Run the game's state machine and determine when it is
    /// finished 
    /// </summary>
    void GameRun()
    {
        game.StateMachine();

        if(game.IsGameDone())
        {
            state = CoordinatorState.CURRENT_GAME_RESET;
        }
    }

    /// <summary>
    /// Reset the game's variables and states. Does NOT remove 
    /// assets from scene 
    /// </summary>
    void GameReset()
    {
        gameManagers[currentGame].ResetGame();

        state = CoordinatorState.CHOOSE_GAME_MODE;
    }

    /// <summary>
    /// Cleans up the game's assets from the scene 
    /// </summary>
    void GameCleaup()
    {
        // Call game's cleanup function 

        state = CoordinatorState.CHOOSE_GAME_MODE;
    }


    private enum CoordinatorState
    {
        CHOOSE_GAME_MODE,       // Choose game from list 
        CURRENT_GAME_INIT,      // Initialize choosen game 
        CURRENT_GAME_RUN,       // Run game manager's statemachine 
        CURRENT_GAME_RESET,     // Reset the the game to be played again 
        CURRENT_GAME_CLEANUP   // Cleanup game assets and return to choice 
    }
}
