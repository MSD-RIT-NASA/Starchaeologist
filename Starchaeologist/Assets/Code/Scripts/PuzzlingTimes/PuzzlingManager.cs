using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

public class PuzzlingManager : MonoBehaviour
{
    [SerializeField] Transform head;

    [Header("Plate Generation Settings")]
    /// How will the plates be generated on load-in
    [SerializeField] GenerationType generationType;

    [SerializeField] Vector2 offset;
    /// How many plates wide will the area be 
    [SerializeField] int xCells;
    /// How many plates far will the area be 
    [SerializeField] int yCells;

    /// Size of a cell 
    [SerializeField] Vector2 cellSize;

    [SerializeField] Vector3 endPlatePos;

    [Header("Plates")]
    // Non-trapped plate 
    [SerializeField] GameObject defaultPlate;
    // The types of plates that can be spawned 
    [SerializeField] List<GameObject> trappedPlatePool;
    

    [Header("Treasure Generation Settings")]
    [SerializeField] List<GameObject> treasurePool;

    [Header("Traps")]
    [SerializeField] List<Trap> traps;

    [Header("Gizmos")]
    [SerializeField] Color gizmosColor;

    // Whether or not a trap is already active 
    private bool trapIsActive;
    private List<PuzzlePlate> plates;

    private Dictionary<PuzzlePlate, Trap> plateToTrap;
    private PuzzlePlate end;

    private enum GenerationType
    {
        Random,
        ReadFile,
        NoTraps,
        AllRandomTraps
    }

    // What plate was the player on during 
    // the last frame 
    private PuzzlePlate currentPlate;
    private bool takenFirstStep = false;
    private List<PuzzlePlate> firstRow;

    private PuzzlingTimeState gameState = PuzzlingTimeState.Load;
    private enum PuzzlingTimeState
    { 
        Load,
        Play,
        End
    }


    public void ActivateTrap()
    {
        // Don't activate more than one or multiple times 
        if (trapIsActive)
            return;
    }

    public PuzzlePlate FindPlateFromPos(Vector3 pos)
    {
        // Get the x and y index of cell based on 
        // simple grid equation 
        int x = Mathf.FloorToInt((pos.x - offset.x + (cellSize.x / 2.0f)) / (cellSize.x));
        int y = Mathf.FloorToInt((pos.z - offset.y + (cellSize.y / 2.0f)) / (cellSize.y));

        print(new Vector2(x, y));

        // Check if in range 
        if (x < 0 || x >= xCells || y < 0 || y >= yCells)
            return null;

        // Each row has cellSize.x amount of cells
        // and then add remaining x index amount 
        return plates[y * xCells + x];
    }

    void Start()
    {
        plates = new List<PuzzlePlate>();
        firstRow = new List<PuzzlePlate> ();
        plateToTrap = new Dictionary<PuzzlePlate, Trap>();
    }

    void Update()
    {
        switch(gameState)
        {
            case PuzzlingTimeState.Load:
                GeneratePlates();
                break;
            case PuzzlingTimeState.Play:
                PlayState();
                break;
            case PuzzlingTimeState.End: // TODO 
                break;

        }

        
    }

    private void PlayState()
    {

        PuzzlePlate next = FindPlateFromPos(head.transform.position);

        if (currentPlate != next) // Whether on a new plate or not 
        {
            if (next == null && takenFirstStep) // On final plate which doesn't actually exist in grid
            {
                print("On End");

                // End game 
                currentPlate.SetWalkStatus(false);
                SetNeighborsWalkable(currentPlate, false);

                currentPlate = null;

                gameState = PuzzlingTimeState.End;
                return;
            }

            else if (next.Index / xCells == yCells - 1)
            {
                // Set final to walkable 
                end.SetWalkStatus(true);
            }
            

            if (!takenFirstStep)
            {
                // Reset first row 
                for (int i = 0; i < firstRow.Count; ++i)
                {
                    takenFirstStep = true;
                    firstRow[i].SetWalkStatus(false);
                }
            }

            // Set this next plate as unsteppable 
            next.SetWalkStatus(false);
            // Activates trap & animates plate going down 
            next.ActivateTrap();

            // TODO - Optimize to only change difference 
            if (currentPlate != null)
                SetNeighborsWalkable(currentPlate, false);

            if (next != null)
                SetNeighborsWalkable(next, true);

            currentPlate = next;
        }
    }

    /// <summary>
    /// Set neighbors of given plate to a given walkable state 
    /// </summary>
    /// <param name="plate"></param>
    /// <param name="walkable"></param>
    private void SetNeighborsWalkable(PuzzlePlate plate, bool walkable)
    {
        // Given plate's index values
        int y = plate.Index / xCells;
        int x = plate.Index % xCells;


        // Go through each cell surronding the current one 
        for (int i = 0; i < 9; i++)
        {
            // Offset to surronding index  
            int nextY = y + (i / 3) - 1;
            int nextX = x + (i % 3) - 1;

            // Check for bounds 
            if (nextY < 0 || nextX < 0)
                continue;
            if (nextY >= yCells || nextX >= xCells)
                continue;

            print(nextX + ", " + nextY);
            int index = nextY * xCells + nextX;
            plates[index].SetWalkStatus(walkable);
        }
    }

    /// <summary>
    /// Initialize the generation of plates based on 
    /// given settings BEFORE runtime 
    /// </summary>
    private void GeneratePlates()
    {
        switch(generationType)
        {
            case GenerationType.Random:
                GenerateRandom();
                break;
            case GenerationType.ReadFile:
                GenerateFromFile();
                break;
            case GenerationType.NoTraps:
                GenerateNoTraps();
                break;
            case GenerationType.AllRandomTraps:
                GenerateAllRandomTraps();
                break;
        }

        end = Instantiate(
                defaultPlate,
                endPlatePos,
                Quaternion.identity
                ).GetComponent<PuzzlePlate>();

        // Finished loading 
        gameState = PuzzlingTimeState.Play;
    }

    /// <summary>
    /// Generate a grid of randomly placed traps 
    /// </summary>
    private void GenerateRandom()
    {
        // Must include default plate 
        int plateCount = trappedPlatePool.Count + 1;

        int totalSize = xCells * yCells;
        for (int i = 0; i < totalSize; i++)
        {
            int y = i / xCells;
            int x = i % xCells;

            Vector3 point = new Vector3(offset.x + cellSize.x * x, 0, offset.y + cellSize.y * y);

            int rand = UnityEngine.Random.Range(0, plateCount);

            PuzzlePlate temp = Instantiate(
                rand == 0 ? defaultPlate : trappedPlatePool[rand - 1],
                point,
                Quaternion.identity
                ).GetComponent<PuzzlePlate>();

            // Set the plate's index 
            temp.SetIndex(i);

            if (y == 0) // Set first row to walkable 
            {
                temp.SetWalkStatus(true);
                firstRow.Add(temp);
            }

            // Assumes obj has puzzleplate component 
            plates.Add(temp);

            
        }
    }

    /// <summary>
    /// Generate a grid based on a file 
    /// </summary>
    private void GenerateFromFile()
    {

    }

    /// <summary>
    /// Generates a grid of plates that have no traps 
    /// </summary>
    private void GenerateNoTraps()
    {

    }

    /// <summary>
    /// Generate a grid that contains only trapped plates 
    /// </summary>
    private void GenerateAllRandomTraps()
    {

    }

    void OnDrawGizmos()
    {
        if (!UnityEngine.Application.isPlaying)
            //return;

        // TODO Change color based on plate/trap type 
        Gizmos.color = gizmosColor;

        // Depict the plates
        int totalSize = xCells * yCells;    
        for(int i = 0; i < totalSize; i++)
        {
            int y = i / xCells;
            int x = i % xCells;

            if (y == 0)
                Gizmos.color = Color.red;
            else
            {
                Gizmos.color = gizmosColor;
            }

            Vector3 point = new Vector3(offset.x + cellSize.x * x, 0, offset.y + cellSize.y * y);
            Gizmos.DrawWireCube(point, new Vector3(cellSize.x, cellSize.y, 1));
            //Gizmos.DrawSphere(point, 0.1f);
            Handles.Label(point, x +", " + y);
        }

        Gizmos.DrawWireCube(endPlatePos, new Vector3(cellSize.x, cellSize.y, 1));
        Handles.Label(endPlatePos, "End");


        // Show trap hitboxes 
        for (int i = 0; i < traps.Count; i++)
        {
            Trap current = traps[i];

            for (int a = 0; a < current.PossiblePlates.Count; a++)
            {
                int index = (int)current.PossiblePlates[a].y * xCells + (int)current.PossiblePlates[a].x;
                // Range check 
                if (index < 0)
                    continue;
                if (index >= (xCells * yCells))
                    continue;

                Gizmos.color = Color.red;

                Vector3 point = new Vector3(
                    offset.x + cellSize.x * (int)current.PossiblePlates[a].x, 
                    0, 
                    offset.y + cellSize.y * (int)current.PossiblePlates[a].y
                    );

                Gizmos.DrawCube(point + current.HitBoxOffset, current.HitBoxSize);
            }
        }
    }
}
