using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * TODO
 *      - Reset barriers after each tile has been achieved 
 */

public class TempleGameManager : MonoBehaviour
{
    [Header("Overal Game Management")]
    [SerializeField] TempleGameStates gameState = TempleGameStates.CHOOSE_GAME_MODE;
    [SerializeField] Transform playerHead;
    [SerializeField] Transform playerLeft;
    [SerializeField] Transform playerRight;
    [Space]
    [SerializeField] GameObject instructionCanvas;

    [Header("Four Squares")]
    [SerializeField] FourSquareStates fourSquareState = FourSquareStates.GENERATE_PATTERNS;
    [Tooltip("How many squares will the player need to travel to before the round ends")]
    [SerializeField] int patternSize;
    [SerializeField] List<Tile> tiles;
    [SerializeField] Vector3 tileSize;
    [Tooltip("Used to instantiate barriers")]
    [SerializeField] GameObject barrierObj;
    [SerializeField] List<Cube_Transform> barriers;
    [SerializeField] float pointsGainSuccess;
    [SerializeField] float pointsLossCollision;

    [Space]
    [SerializeField] int currentDifficulty; 
    [SerializeField] List<DifficultySettings> FS_Difficulties;

    //[Header("Pose Copy")]
    //[SerializeField]

    private void Start()
    {
        // Setup Foursquares 
        barrierTransforms = new List<Transform>();
        for (int i = 0; i < barriers.Count; i++)
        {
            barrierTransforms.Add(Instantiate(barrierObj, this.transform.position, Quaternion.identity).transform);
            barrierObj.SetActive(false);
        }
        UpdateBarrierTransforms();
    }

    void Update()
    {
        TempleSM();
    }

    #region GAME_MANAGER

    public void SetGameMode(int gameMode)
    {
        gameState = (TempleGameStates)gameMode;

        // Any intitialization 
        switch (gameState)
        {
            case TempleGameStates.CHOOSE_GAME_MODE:
                break;
            case TempleGameStates.POSE_MATCH:
                break;
            case TempleGameStates.FOUR_SQUARE:
                InitializeFourSquare();
                break;
            case TempleGameStates.DISPLAY_SCORE:
                break;
        }
    }

    /// <summary>
    /// State machine that runs the temple game modes 
    /// </summary>
    void TempleSM() 
    {
        switch (gameState)
        {
            case TempleGameStates.CHOOSE_GAME_MODE:
                ChooseGameMode();
                break;
            case TempleGameStates.POSE_MATCH:

                break;
            case TempleGameStates.FOUR_SQUARE:
                FourSquareSM();
                break;
            case TempleGameStates.DISPLAY_SCORE:
                DisplayScore();
                break;
        }

        instructionCanvas.SetActive(gameState == TempleGameStates.CHOOSE_GAME_MODE);
    }

    /// <summary>
    /// Lets the user choose what game to play 
    /// </summary>
    void ChooseGameMode()
    {
        // Temporary 
        //gameState = TempleGameStates.FOUR_SQUARE;

        // Game mode is decided by button press 
    }

    void DisplayScore()
    {
        gameState = TempleGameStates.CHOOSE_GAME_MODE;
    }

    /// <summary>
    /// Changes the visuals around the player to show the score of the
    /// desired gamemode
    /// </summary>
    /// <param name="game"></param>
    void BeginDisplayScore(GameModes game) { } 
   

    enum GameModes
    {
        POSE_MATCH,
        FOUR_SQUARE
    }

    #endregion

    #region FOUR_SQUARES


    // Arr of indexes each refer to differnt square 
    private int[] pattern;
    private int indexInPattern; // Index in pattern arr
    // Selected tile 
    private int currentTile;

    // Recorded data 
    private float score = 0.0f;
    private int collisions = 0;

    // The generated transforms 
    private List<Transform> barrierTransforms;

    /// <summary>
    /// Sets up this game 
    /// </summary>
    void InitializeFourSquare()
    {
        fourSquareState = FourSquareStates.GENERATE_PATTERNS;
        UpdateBarrierTransforms();

        foreach (Transform t in barrierTransforms)
        {
            t.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Manages the states of the Four Square game-mode 
    /// </summary>
    void FourSquareSM() 
    { 
        switch (fourSquareState)
        {
            case FourSquareStates.GENERATE_PATTERNS:
                GeneratePatterns();
                break;
            case FourSquareStates.DISPLAY_PATTERN:
                DisplayPattern();
                break;
            case FourSquareStates.PLAY:
                PlayState();
                break;
            case FourSquareStates.END_GAME:
                FourSquareEndGame();
                break;
        }
    }

    /// <summary>
    /// Generate patterns state
    /// </summary>
    private void GeneratePatterns()
    {
        pattern = GeneratePattern();
        indexInPattern = 0;
        currentTile = pattern[indexInPattern];

        fourSquareState = FourSquareStates.DISPLAY_PATTERN;
    }


    /// <summary>
    /// Creates an array of indexes that each represent one of the squares that the player must step to. 
    /// There cannot be the same two indexes in a row. 
    /// </summary>
    /// <returns></returns>
    private int[] GeneratePattern()
    {
        int[] pattern = new int[patternSize];
        int previous = -1;

        for (int i = 0; i < patternSize; i++)
        {
            int current;
            
            // Make sure not tile that player is currently standing on 
            if (i == 0)
            {
                int playerCurrentTile = -1;

                // Find current tile player head is in 
                for (int t = 0; t < tiles.Count; t++)
                {
                    print("in tiles search " + InTile(playerHead, t));
                    if (InTile(playerHead, t))
                    {
                        playerCurrentTile = t;
                        break;
                    }
                }


                // Don't continue if not in any tile 
                if(playerCurrentTile != -1)
                {

                    // Continue until current is not the tile 
                    // the player is in 
                    do
                    {
                        // 0 to 4 each represents a square index 
                        current = UnityEngine.Random.Range(0, 4);

                    } while (current == playerCurrentTile);

                    // Set values 
                    pattern[i] = current;
                    previous = current;

                    continue;
                }
                
            }


            // Make sure pattern does not repeat 
            do
            {
                // 0 to 4 each represents a square index 
                current = UnityEngine.Random.Range(0, 4);

            } while (current == previous);

            // Set values 
            pattern[i] = current;
            previous = current;

        }

        return pattern;
    }

    /// <summary>
    /// Shows the current square that the player must travel to. 
    /// </summary>
    private void DisplayPattern() 
    {
        // Send pattern to console 
        string patternStr = "";
        for (int i = 0; i < patternSize; i++)
        {
            patternStr += pattern[i].ToString();
        }
        print("Current pattern: " + patternStr);

        // Reset 
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].SetSelection(i == currentTile ? FS_Square.FSSquareStates.TARGET : FS_Square.FSSquareStates.NOT_TARGET);
        }

        fourSquareState = FourSquareStates.PLAY;
    }

    /// <summary>
    /// Dictates when the player has reached the desired square 
    /// </summary>
    private void PlayState()
    {
        // TODO: Figure out why we need to check if the round is complete twice...


        // Playstate Management 
        if (IsRoundComplete())
        {
            fourSquareState = FourSquareStates.END_GAME;
            return;
        }

        if (PlayerInTarget())
        {
            score += pointsGainSuccess;
            indexInPattern++;

            if (IsRoundComplete())
            {
                fourSquareState = FourSquareStates.END_GAME;
                return;
            }

            currentTile = pattern[indexInPattern];

            fourSquareState = FourSquareStates.DISPLAY_PATTERN;
        }



    }

    /// <summary>
    /// Changes how the game is visualized 
    /// </summary>
    private void UpdateBarrierTransforms()
    {
        // Barrier Height 
        DifficultySettings settings = FS_Difficulties[currentDifficulty];
        for (int i = 0; i < barriers.Count; i++)
        {
            float height = UnityEngine.Random.Range(settings.heightMin, settings.heightMax);

            Cube_Transform transform = barriers[i];
            barrierTransforms[i].position = this.transform.position + transform.position + Vector3.up * height / 2.0f;
            barrierTransforms[i].localScale = new Vector3(transform.scale.x, settings.heightMin, transform.scale.y);
        }
    }

    /// <summary>
    /// Final state of this game which transitions from four square to display score 
    /// </summary>
    private void FourSquareEndGame()
    {
        // TODO: Write data to text file 
        print("Final Score: " + score);
        print("Total collision: " + collisions);

        // Reset to default visual 
        foreach(Tile tile in tiles)
        {
            tile.tileObj.SetTargetVisual(FS_Square.FSSquareStates.NOT_IN_PLAY);
        }

        // Turn off barriers 
        foreach (Transform t in barrierTransforms)
        {
            t.gameObject.SetActive(false);
        }

        score = 0.0f;
        collisions = 0;
        currentTile = 0;
        indexInPattern = 0;

        gameState = TempleGameStates.DISPLAY_SCORE;
    }

    /// <summary>
    /// Detects whether the player is fully within the desired target. 
    /// This is done by checking if the feet, hands, and head occupy the desired area.
    /// </summary>
    /// <returns></returns>
    private bool PlayerInTarget()
    {
        int tile = pattern[indexInPattern];
        return InTile(playerHead, tile) && InTile(playerLeft, tile) && InTile(playerRight, tile);
    }

    /// <summary>
    /// Used to check if a transform is within range of a given tile index 
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tileIndex"></param>
    /// <returns></returns>
    private bool InTile(Transform transform, int tileIndex)
    {
        Vector3 tileCurr = this.transform.position + tiles[tileIndex].tilePos;
        Vector3 halfSize = tileSize / 2.0f;

        bool aboveMin =
                transform.position.x >= tileCurr.x - halfSize.x &&
                transform.position.z >= tileCurr.z - halfSize.z;

        bool belowMax =
            transform.position.x <= tileCurr.x + halfSize.x &&
            transform.position.z <= tileCurr.z + halfSize.z;

        return aboveMin && belowMax;
    }

    /// <summary>
    /// Returns true if the the entire pattern has been completed 
    /// </summary>
    /// <returns></returns>
    private bool IsRoundComplete()
    {
        return indexInPattern >= patternSize;
    }

    /// <summary>
    /// Call if the player has made collision with a barrier 
    /// </summary>
    public void TakeCollision()
    {
        score -= pointsLossCollision;
        collisions++;
    }

    enum FourSquareStates
    {
        GENERATE_PATTERNS,  // Setup for the game round 
        DISPLAY_PATTERN,    // Indicates the current square 
        PLAY,               // Grades player score and waits for their input 
        END_GAME            // Cleanup and send to display score 
    }

    [System.Serializable]
    private class Tile
    {
        [SerializeField] public Vector3 tilePos;
        [SerializeField] public FS_Square tileObj;


        /// <summary>
        /// Update the visual of this tile to represet whether it is 
        /// the current target tile or not 
        /// </summary>
        /// <param name="selection"></param>
        public void SetSelection(FS_Square.FSSquareStates state)
        {
            tileObj.SetTargetVisual(state);
        }

    }

    [System.Serializable]
    private class Cube_Transform
    {
        [SerializeField] public Vector3 position;
        [Tooltip("Only can decide x and z axis as height is done by difficulty")]
        [SerializeField] public Vector2 scale;
    }

    [System.Serializable] 
    private class DifficultySettings
    {
        [SerializeField] public string Name;
        [SerializeField] public float heightMax;
        [SerializeField] public float heightMin;

        [Tooltip("Each wall is random in height")]
        [SerializeField] public bool variableHeights;
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


    private void OnDrawGizmosSelected()
    {
        // Represent tiles areas 
        for (int i = 0; i < tiles.Count; i++)
        {
            if(i == currentTile)
            {
                Gizmos.color = Color.black;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            //Gizmos.DrawCube()
            Gizmos.DrawWireCube(this.transform.position + tiles[i].tilePos, tileSize);
        }

        Gizmos.color = Color.magenta;
        foreach (Cube_Transform transform in barriers)
        {
            DifficultySettings settings = FS_Difficulties[currentDifficulty];

            if(Application.isPlaying)
            {
                Gizmos.DrawWireCube(
                this.transform.position + transform.position + Vector3.up * settings.heightMin / 2.0f,
                new Vector3(transform.scale.x, settings.heightMin, transform.scale.y)
                );
            }
            else
            {
                Gizmos.DrawCube(
                this.transform.position + transform.position + Vector3.up * settings.heightMin / 2.0f,
                new Vector3(transform.scale.x, settings.heightMin, transform.scale.y)
                );
            }
        }
    }
}
