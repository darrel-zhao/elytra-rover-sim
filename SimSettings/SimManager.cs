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
    public int inactiveRovers { get; set; } = 0;
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

        roverManager.OnRoversInitialized += HandleRoversInitialized;
        roverManager.AssignPathsandStart(settings.assignments, settings.numberOfRovers);
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
        
        if (settings != null && inactiveRovers == settings.numberOfRovers)
        {
            print($"Simulation Complete! Here are the results: ");
            print($"Total Trash Collected: {totalTrashCollected}");
            int totalDetected = 0;
            // Calculate efficacy of each rover
            for (int i = 0; i < roverManager.numRovers; i++)
            {
                var rover = roverManager.transform.GetChild(i).GetComponent<RoverDriver>().rover;
                var detected = roverManager.transform.GetChild(i).GetComponentInChildren<TrashFinder>().detectedCount;
                totalDetected += detected;

                print($"Rover {rover.id}: {rover.trashCollected / (float)detected * 100}% efficacy ({rover.trashCollected}/{detected} collected)");
            }
            print($"Fleet Efficacy: {totalTrashCollected / (float)totalDetected * 100}% ({totalTrashCollected}/{totalDetected} collected)");
            inactiveRovers = 0;
        }
    }
}
