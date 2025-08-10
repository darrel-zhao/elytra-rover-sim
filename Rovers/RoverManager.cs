using System;
using System.Collections.Generic;
using System.Linq;
using Sim.Rover;
using UnityEngine;

public class RoverManager : MonoBehaviour
{
    [SerializeField] GridMapGenerator map;

    [Header("Rover Prefab")]
    public GameObject roverPrefab;

    [Header("Number of Rovers")]
    public int numRovers;

    // Rover should spawn on the side of the road, not middle
    float spawnXOffset = 1.5f;
    float spawnZOffset = 2.3f; // remove later

    // Housekeeping variables 
    public int roverCount = 0;
    List<Camera> roverCameras;

    // Event handling
    public event Action OnRoversInitialized;

    public void AssignPathsandStart(List<(int start, int end)> assignments)
    {
        roverCameras = new List<Camera>();
        for (int i = 0; i < assignments.Count; i++)
        {
            // Create rover instance
            var (s, e) = assignments[i];
            var data = new Rover(i, s, e, map.graph);

            // Have rover compute its path
            if (!data.ComputePath(s, e))
            {
                Debug.LogError($"No path for rover {i} from {s} to {e}.");
                return;
            }


            // Copy path queue and figure out first destination (second in queue)
            print("Initialized Rover heading towards node " + data.path.ElementAt(1));
            int next = data.path.ElementAt(1);

            // Instantiate rover
            GameObject roverGO;
            SpawnRover(s, next, out roverGO);
            var ctrl = roverGO.GetComponent<RoverDriver>();
            if (ctrl == null) { Debug.LogError($"RoverDriver component missing on {roverGO.name}."); }

            ctrl.Init(map, data);
        }

        OnRoversInitialized?.Invoke();
    }

    void SpawnRover(int node, int next, out GameObject rover)
    {
        Vector3 pos = map.NodeToWorld(node);
        Vector3 second = map.NodeToWorld(next);

        // calculate rover heading
        Vector3 startHeading = second - pos;

        // calculate rover positioning
        if (isRight(Vector3.forward, startHeading))
        {
            print("right");
            pos.x += spawnZOffset;
            pos.z -= spawnXOffset;
        }
        else if (isLeft(Vector3.forward, startHeading))
        {
            print("left");
            pos.x -= spawnZOffset;
            pos.z += spawnXOffset;
        }
        else
        {
            print("none");
            pos.x += spawnXOffset;
            pos.z += spawnZOffset;
        }

        roverCount++;
        rover = Instantiate(roverPrefab, pos, Quaternion.identity, transform);
        rover.transform.forward = startHeading;
        rover.name = $"Rover {roverCount}";

        // remove shadows
        var renderers = rover.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderers)
        {
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
        }

        roverCameras.Add(rover.GetComponentInChildren<Camera>());
    }

    bool isRight(Vector3 from, Vector3 to)
    {
        float signedAngle = Vector3.SignedAngle(from, to, Vector3.up);
        return signedAngle > 20f;
    }

    bool isLeft(Vector3 from, Vector3 to)
    {
        float signedAngle = Vector3.SignedAngle(from, to, Vector3.up);
        return signedAngle < -20f;
    }
}
