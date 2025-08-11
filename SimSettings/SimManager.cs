using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimManager : MonoBehaviour
{
    [SerializeField] GridMapGenerator gridMapGenerator;
    [SerializeField] RoverManager roverManager;
    [SerializeField] TrashSpawner trashSpawner;
    [SerializeField] CameraManager cameraManager;
    public int totalTrashCollected { get; set; }
    private SimSettings settings;

    public void SetSettings(SimSettings simSettings)
    {
        settings = simSettings;

        // Apply settings to respective components
        gridMapGenerator.width = settings.gridMapCols;
        gridMapGenerator.height = settings.gridMapRows;
        totalTrashCollected = 0;

        // roverSpawner.numRovers = settings.numberOfRovers;
        trashSpawner.numberOfTrashItems = settings.numTrashItems;

        Initialize();
    }

    void Initialize()
    {
        if (gridMapGenerator == null)
        {
            Debug.LogError("GridMapGenerator is not assigned in the GameManager.");
            return;
        }

        if (roverManager == null)
        {
            Debug.LogError("RoverSpawner is not assigned in the GameManager.");
            return;
        }

        gridMapGenerator.OnMapInitialized += HandleMapInitialized;
        gridMapGenerator.InitializeMap();
    }

    void HandleMapInitialized()
    {
        gridMapGenerator.OnMapInitialized -= HandleMapInitialized; // Unsubscribe to prevent multiple calls
        Debug.Log("Map initialized successfully.");

        // roverSpawner.OnRoversInitialized += HandleRoversInitialized;
        // roverSpawner.InitializeRovers();

        roverManager.OnRoversInitialized += HandleRoversInitialized;
        List<(int s, int e)> assignments = new List<(int s, int e)> { (0, 7), (4, 1), (3, 5), (2, 8) }; // hardcoded for debugging purposes
        roverManager.AssignPathsandStart(assignments);
    }

    void HandleRoversInitialized()
    {
        // roverSpawner.OnRoversInitialized -= HandleRoversInitialized; // Unsubscribe to prevent multiple calls
        roverManager.OnRoversInitialized -= HandleRoversInitialized;
        Debug.Log("Rovers initialized successfully.");

        trashSpawner.OnTrashSpawned += HandleTrashSpawned;
        trashSpawner.SpawnTrash();
    }

    void HandleTrashSpawned()
    {
        trashSpawner.OnTrashSpawned -= HandleTrashSpawned; // Unsubscribe to prevent multiple calls
        Debug.Log("Trash spawned successfully.");

        cameraManager.OnCamerasUpdated += HandleCamerasUpdated;
        cameraManager.getCameras();
    }

    void HandleCamerasUpdated()
    {
        cameraManager.OnCamerasUpdated -= HandleCamerasUpdated; // Unsubscribe to prevent multiple calls
        Debug.Log("Cameras updated successfully.");
    }

    void Update()
    {
        // Camera Switching: if "c" is pressed, switch to the next camera
        if (Keyboard.current.cKey.wasPressedThisFrame)
            cameraManager.SwitchNextCamera();
    }
}
