using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerCoordinator : MonoBehaviour
{
    // NOTE: This coordinator has the ability to hold multiple game
    //       modes and switch to them if necessary. This ability 
    //       should only be used if there are multiple gamemodes
    //       within a single scene. 
    //
    //      This was needed previously but now it might not be 
    //      needed at all. I do recommend continuing to use it
    //      though because it requries game managers to follow 
    //      a stricter function setup. 

    [SerializeField] CoordinatorState state;

    [Header("Games")]
    [SerializeField] List<VirtGameManager> gameManagers;
    [SerializeField] int currentGame = -1;
    [SerializeField] bool drawCurrentGizmos;

    [Header("UI")]
    [SerializeField] GameObject instructionCanvas;

    [Header("Player")]
    [SerializeField] string playerName; // TODO: Have player input their name id 
    [SerializeField] Transform playerHead;
    [SerializeField] Transform playerHandLeft;
    [SerializeField] Transform playerHandRight;
    [SerializeField] Transform playerAnkleLeft;
    [SerializeField] Transform playerAnkleRight;

    private VirtGameManager game;
    private DataStoreLoad dsl;

    private void Start()
    {
        instructionCanvas.SetActive(true);
    }

    private void Update()
    {
        game = DoesGameExist() ? gameManagers[currentGame] : null;

        StateMachine();
    }

    private void StateMachine()
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
    private void ChooseGameMode()
    {
        print(DoesGameExist());
        // Only continues once the current game index 
        // has been set to a valid value 
        if(currentGame == 0 /*DoesGameExist()*/)
        {
            instructionCanvas.SetActive(false);
            state = CoordinatorState.CURRENT_GAME_INIT;
        }
    }

    /// <summary>
    /// Initialize a game from its start 
    /// </summary>
    private void GameInit()
    {
        // Game will manage if assets are already created 
        game.Init(
            playerHead, 
            playerHandLeft, 
            playerHandRight, 
            playerAnkleLeft, 
            playerAnkleRight);

        state = CoordinatorState.CURRENT_GAME_RUN;
    }

    /// <summary>
    /// Run the game's state machine and determine when it is
    /// finished 
    /// </summary>
    private void GameRun()
    {
        game.StateMachine();

        if(game.IsGameDone())
        {
            game.WriteDataToFile(playerName, dsl);
            state = CoordinatorState.CURRENT_GAME_RESET;
        }
    }

    /// <summary>
    /// Reset the game's variables and states. Does NOT remove 
    /// assets from scene 
    /// </summary>
    private void GameReset()
    {
        gameManagers[currentGame].ResetGame();

        state = CoordinatorState.CURRENT_GAME_CLEANUP;
    }

    /// <summary>
    /// Cleans up the game's assets from the scene 
    /// </summary>
    private void GameCleaup()
    {
        // NOTE: We currently have this in a seperate function than
        //       the reset function because we don't want to cleanup 
        //       the world if the player wants to play the same game
        //       again. 

        // Call game's cleanup function 
        game.CleanupGame();

        state = CoordinatorState.CHOOSE_GAME_MODE;


        currentGame = -1; // Reset to default 
    }

    /// <summary>
    /// Checks if the game index is within range 
    /// </summary>
    /// <returns></returns>
    private bool DoesGameExist()
    {
        return currentGame >= 0 && currentGame < gameManagers.Count;
    }

    /// <summary>
    /// Used by the UI to let the player set a gamemode 
    /// </summary>
    /// <param name="gameMode"></param>
    public void SetGameMode(int gameMode)
    {
        currentGame = gameMode;
    }

    private enum CoordinatorState
    {
        CHOOSE_GAME_MODE,       // Choose game from list 
        CURRENT_GAME_INIT,      // Initialize choosen game 
        CURRENT_GAME_RUN,       // Run game manager's statemachine 
        CURRENT_GAME_RESET,     // Reset the the game to be played again 
        CURRENT_GAME_CLEANUP    // Cleanup game assets and return to choice 
    }

    private void OnDrawGizmos()
    {
        if (drawCurrentGizmos && DoesGameExist())
        {
            // Don't use "game" here because we use
            // in the editor 
            gameManagers[currentGame].GizmosVisuals();
        }
    }
}
