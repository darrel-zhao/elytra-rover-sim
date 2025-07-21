using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    RoverSpawner rs;

    int activeCameraIndex = 0; // Index of the currently active camera

    Camera[] allCameras;

    // Event handling
    public event Action OnCamerasUpdated;

    public Camera[] getCameras()
    {
        rs = FindFirstObjectByType<RoverSpawner>();
        allCameras = new Camera[rs.roverCount + 1]; // +1 for the main camera
        allCameras[0] = Camera.main; // Assign the main camera to the first index

        if (rs == null)
        {
            Debug.LogError("RoverSpawner not found in the scene.");
            return null;
        }

        // Get and add all rover cameras
        Camera[] roverCams = rs.GetComponentsInChildren<Camera>();

        print("roverCams: " + roverCams.Length);

        for (int i = 1; i < allCameras.Length; i++)
        {
            allCameras[i] = roverCams[i - 1]; // Assign rover cameras to the subsequent indices
            allCameras[i].gameObject.SetActive(false);
        }

        if (allCameras.Length == 0)
        {
            Debug.LogWarning("No cameras found in RoverSpawner.");
        }
        else
        {
            Debug.Log($"{allCameras.Length} cameras found in RoverSpawner.");
        }

        // Set only the first camera (main camera) as active
        allCameras[activeCameraIndex].gameObject.SetActive(true);

        for (int i = 1; i < allCameras.Length; i++)
        {
            if (allCameras[i] == null)
            {
                Debug.LogWarning($"Camera at index {i} is null.");
                continue;
            }
            allCameras[i].gameObject.SetActive(false); // Deactivate all rover cameras initially
        }

        // Notify subscribers that cameras have been updated
        OnCamerasUpdated?.Invoke();
        return allCameras;
    }

    public void SwitchNextCamera()
    {
        allCameras[activeCameraIndex].gameObject.SetActive(false); // Deactivate the current camera
        activeCameraIndex = (activeCameraIndex + 1) % allCameras.Length; // Move to the next camera
        allCameras[activeCameraIndex].gameObject.SetActive(true); // Activate the new camera
    }
}
