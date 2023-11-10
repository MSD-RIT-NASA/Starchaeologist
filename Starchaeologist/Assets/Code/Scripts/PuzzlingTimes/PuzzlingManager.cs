using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

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

    [Header("Plates")]
    // Non-trapped plate 
    [SerializeField] GameObject defaultPlate;
    // The types of plates that can be spawned 
    [SerializeField] List<GameObject> trappedPlatePool;

    [Header("Treasure Generation Settings")]
    [SerializeField] List<GameObject> treasurePool;

    [Header("Gizmos")]
    [SerializeField] Color gizmosColor;

    // Whether or not a trap is already active 
    private bool trapIsActive;

    private enum GenerationType
    { 
        Random,
        ReadFile,
        NoTraps,
        AllRandomTraps
    }


    public void ActivateTrap()
    {
        // Don't activate more than one 
        if (trapIsActive)
            return;
    }


    void Start()
    {
        GeneratePlates();
    }

    void Update()
    {

    }


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

            Instantiate(
                rand == 0 ? defaultPlate : trappedPlatePool[rand - 1],
                point,
                Quaternion.identity
                );
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

            Vector3 point = new Vector3(offset.x + cellSize.x * x, 0, offset.y + cellSize.y * y);
            Gizmos.DrawWireCube(point, new Vector3(cellSize.x, cellSize.y, 1));
            Gizmos.DrawSphere(point, 0.1f);
        }
    }

}
