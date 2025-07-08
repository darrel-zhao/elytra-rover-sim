using UnityEngine;


public class RoverSpawner : MonoBehaviour
{
    [Header("Rover Prefab")]
    public GameObject roverPrefab;

    [Header("Start Position")]
    public int startNode = 0;

    // Rover should spawn at this height above the ground
    float spawnYOffset = 0.01f;

    // Rover should spawn on the side of the road, not middle
    float spawnXOffset = 1.5f;
    int roverCount = 0;

    void Start()
    {
        var mapGen = FindFirstObjectByType<GridMapGenerator>();

        if (mapGen == null)
        {
            Debug.LogError("No road map provided. Please add one before spawning rovers.");
            return;
        }

        // Randomly generate 3 rovers at randomly selected start nodes
        // for (int i = 0; i < 3; i++)
        // {
        //     int randomNode = Random.Range(0, 100); // Assuming there are 100 nodes
        //     SpawnRover(randomNode);
        // }

        // get (x, z) coordinates of the start node and then use offset to make wheels touch ground
        Vector3 pos = mapGen.NodeToWorld(startNode);
        pos.x += spawnXOffset; // offset to make sure rover is not in the middle of the road
        pos.y += spawnYOffset; // offset to make sure rover is above ground

        roverCount++;
        Instantiate(roverPrefab, pos, Quaternion.identity, transform)
            .name = $"Rover {roverCount}";
    }

}