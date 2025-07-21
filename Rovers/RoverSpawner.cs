using System;
using System.Collections.Generic;
using UnityEngine;

public class RoverSpawner : MonoBehaviour
{
    [Header("Rover Prefab")]
    public GameObject roverPrefab;

    [Header("Number of Rovers")]
    public int numRovers = 1;

    // Rover should spawn on the side of the road, not middle
    float spawnXOffset = 1.5f;
    float spawnZOffset = 2.3f; // remove later

    // Housekeeping variables 
    public int roverCount = 0;
    // List<Camera> roverCameras;

    // Event handling
    public event Action OnRoversInitialized;

    public void InitializeRovers()
    {
        var mapGen = FindFirstObjectByType<GridMapGenerator>();

        if (mapGen == null)
        {
            Debug.LogError("No road map provided. Please add one before spawning rovers.");
            return;
        }

        // Randomly generate 3 rovers at randomly selected start nodes
        for (int i = 0; i < numRovers; i++)
        {
            SpawnRover(i);
        }

        OnRoversInitialized?.Invoke();
    }

    void SpawnRover(int node)
    {
        Vector3 pos = FindFirstObjectByType<GridMapGenerator>().NodeToWorld(node);
        pos.x += spawnXOffset; // offset to make sure rover is not in the middle of the road
        pos.z += spawnZOffset; // remove later

        roverCount++;
        var rover = Instantiate(roverPrefab, pos, Quaternion.identity, transform);
        rover.name = $"Rover {roverCount}";
    }

}