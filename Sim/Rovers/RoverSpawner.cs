using UnityEngine;

public class RoverSpawner : MonoBehaviour
{
    [Header("Rover Prefab")]
    public GameObject roverPrefab;

    [Header("Start Position")]
    public int startNode = 0;

    // Rover should spawn at this height above the ground
    float spawnYOffset = 0.01f;

    int roverCount = 0;

    void Start()
    {
        var mapGen = FindFirstObjectByType<GridMapGenerator>();

        if (mapGen == null)
        {
            Debug.LogError("No road map provided. Please add one before spawning rovers.");
            return;
        }

        // get (x, z) coordinates of the start node and then use offset to make wheels touch ground
        Vector3 pos = mapGen.NodeToWorld(startNode);
        pos.y += spawnYOffset; // offset to make sure rover is above ground

        roverCount++;
        Instantiate(roverPrefab, pos, Quaternion.identity, transform)
            .name = $"Rover {roverCount}";
    }

}