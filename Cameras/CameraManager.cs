using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    RoverSpawner rs;

    Camera[] allCameras;

    // void Start()
    // {
    //     allCameras = Camera.allCameras;
    //     rs = FindFirstObjectByType<RoverSpawner>();
    //     if (rs == null)
    //     {
    //         Debug.LogError("RoverSpawner not found. Please ensure it is present in the scene.");
    //         return;
    //     }

    //     print(allCameras.Length + " cameras found in the scene.");
    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
    
    // IEnumerator waitForRovers(RoverSpawner rs)
    // {
    //     if (rs == null)
    //     {
    //         Debug.LogError("RoverSpawner not found. Please ensure it is present in the scene.");
    //         yield break;
    //     }

    //     while (rs.roverCount < rs.numRovers)
    //     {
    //         yield return null;
    //     }

    //     // All rovers spawned, do something here
    //     Debug.Log("All rovers have been spawned.");
    // }
}
