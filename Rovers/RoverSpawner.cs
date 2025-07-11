using UnityEngine;


public class RoverSpawner : MonoBehaviour
{
    [Header("Rover Prefab")]
    public GameObject roverPrefab;

    [Header("Number of Rovers")]
    public int numRovers = 1;

    // Rover should spawn at this height above the ground
    float spawnYOffset = 0.01f;

    // Rover should spawn on the side of the road, not middle
    float spawnXOffset = 1.5f;
    float spawnZOffset = 2.3f; // remove later

    // Housekeeping variables 
    int roverCount = 0;
    int totalNodes;

    void Start()
    {
        var mapGen = FindFirstObjectByType<GridMapGenerator>();

        if (mapGen == null)
        {
            Debug.LogError("No road map provided. Please add one before spawning rovers.");
            return;
        }

        totalNodes = mapGen.width * mapGen.height;

        // Randomly generate 3 rovers at randomly selected start nodes
        for (int i = 0; i < numRovers; i++)
        {
            SpawnRover(i);
        }
    }

    void SpawnRover(int node)
    {
        Vector3 pos = FindFirstObjectByType<GridMapGenerator>().NodeToWorld(node);
        pos.x += spawnXOffset; // offset to make sure rover is not in the middle of the road
        pos.y += spawnYOffset; // offset to make sure rover is above ground
        pos.z += spawnZOffset; // remove later

        roverCount++;
        Instantiate(roverPrefab, pos, Quaternion.identity, transform)
            .name = $"Rover {roverCount}";
    }

}