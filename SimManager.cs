using UnityEngine;
using UnityEngine.InputSystem;

public class SimManager : MonoBehaviour
{
    [SerializeField] GridMapGenerator gridMapGenerator;
    [SerializeField] RoverSpawner roverSpawner;
    [SerializeField] TrashSpawner trashSpawner;
    [SerializeField] CameraManager cameraManager;

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

        gridMapGenerator.OnMapInitialized += HandleMapInitialized;
        gridMapGenerator.InitializeMap();
    }

    void HandleMapInitialized()
    {
        gridMapGenerator.OnMapInitialized -= HandleMapInitialized; // Unsubscribe to prevent multiple calls
        Debug.Log("Map initialized successfully.");

        roverSpawner.OnRoversInitialized += HandleRoversInitialized;
        roverSpawner.InitializeRovers();
    }

    void HandleRoversInitialized()
    {
        roverSpawner.OnRoversInitialized -= HandleRoversInitialized; // Unsubscribe to prevent multiple calls
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
