using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GridMapGenerator gridMapGenerator;
    [SerializeField] RoverSpawner roverSpawner;
    [SerializeField] TrashSpawner trashSpawner;
    void Awake()
    {
        if (gridMapGenerator == null)
        {
            Debug.LogError("GridMapGenerator is not assigned in the GameManager.");
            return;
        }

        if (roverSpawner == null)
        {
            Debug.LogError("RoverSpawner is not assigned in the GameManager.");
            return;
        }

        gridMapGenerator.OnMapInitialized += OnMapInitialized;
        gridMapGenerator.InitializeMap();
    }

    void OnMapInitialized()
    {
        gridMapGenerator.OnMapInitialized -= OnMapInitialized; // Unsubscribe to prevent multiple calls
        Debug.Log("Map initialized successfully.");

        roverSpawner.OnRoversInitialized += OnRoversInitialized;
        roverSpawner.InitializeRovers();
    }

    void OnRoversInitialized()
    {
        roverSpawner.OnRoversInitialized -= OnRoversInitialized; // Unsubscribe to prevent multiple calls
        Debug.Log("Rovers initialized successfully.");

        trashSpawner.OnTrashSpawned += OnTrashSpawned;
        trashSpawner.SpawnTrash();
    }

    void OnTrashSpawned()
    {
        trashSpawner.OnTrashSpawned -= OnTrashSpawned; // Unsubscribe to prevent multiple calls
        Debug.Log("Trash spawned successfully.");
    }
}
