using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TrashSpawner : MonoBehaviour
{
    [Header("Trash Spawner Settings")]
    //public GameObject trashPrefab; // Prefab for the trash object
    public int numberOfTrashItems = 10; // Number of trash items to spawn

    [Header("Debug Mode")]
    public bool debugMode = false; // Enable debug mode to visualize trash spawning

    float roadWidth = 0.4f; // Width of the road area where trash can spawn
    float[] axisOffsets = new float[] { -1.6f, 1.6f }; // Possible offsets for trash spawning

    // Event handling
    public event Action OnTrashSpawned;

    /// <summary>
    /// Spawns trash items throughout the map
    /// </summary>
    public void SpawnTrash()
    {
        // Load all trash prefabs labeled "Trash" using Addressables
        GameObject[] trashArray = getAllTrashPrefabs();
        
        if (trashArray == null || trashArray.Length == 0)
        {
            Debug.LogError("No trash prefabs found. Please add some prefabs to Addressables with the label 'Trash'.");
            return;
        }

        // Get the map generator instance
        var map = FindFirstObjectByType<GridMapGenerator>();

        // Error checking
        if (map == null)
        {
            Debug.LogError("No road map provided. Please add one before spawning trash.");
            return;
        }

        // loop to iterate through the number of trash items to spawn
        for (int i = 0; i < numberOfTrashItems; i++)
        {
            // randomly select an edge (road) to spawn trash, then randomly select an adjacent node from that beginning node
            int fromNode, toNode;
            selectRandomRoad(out fromNode, out toNode, ref map);

            // Convert the selected nodes to world positions
            Vector3 fromPos, toPos, orthogonalDirection;
            convertToWorldPositions(out fromPos, out toPos, out orthogonalDirection, fromNode, toNode, ref map);

            // check debugMode: if active, draws lines to visualize the spawn positions
            if (debugMode)
                debugSpawnTrash(fromPos, toPos, orthogonalDirection);

            // Calculate the random spawn position along the road + assign
            Vector3 spawnPosition = calcSpawnPosition(fromPos, toPos, orthogonalDirection);

            // Finally, spawn the trash item
            initiateSpawnedTrash(i, spawnPosition, ref trashArray);
        }

        OnTrashSpawned?.Invoke();
    }

    GameObject[] getAllTrashPrefabs()
    {
        List<GameObject> trashList = new List<GameObject>();
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>("Trash", null);

        handle.WaitForCompletion();
        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result.Count == 0)
        {
            Debug.LogError("No trash prefabs found with the 'Trash' label. Please add some prefabs to Addressables.");
            return null;
        }

        trashList.AddRange(handle.Result);
        return trashList.ToArray();
    }

    void selectRandomRoad(out int fromNode, out int toNode, ref GridMapGenerator map)
    {
        fromNode = UnityEngine.Random.Range(0, map.numNodes);
        List<int> adjacentVertices = new List<int>();

        // Find all edges adjacent to the selected node
        foreach (var edge in map.graph.AdjacentEdges(fromNode))
        {
            // if the edge’s Source is our node, take the Target, otherwise take Source
            int neighbor = edge.Source == fromNode
                            ? edge.Target
                            : edge.Source;
            adjacentVertices.Add(neighbor);
        }

        if (adjacentVertices.Count == 0)
        {
            Debug.LogWarning($"No adjacent vertices found for node {fromNode}.");
        }

        int randomEdgeIndex = UnityEngine.Random.Range(0, adjacentVertices.Count);
        toNode = adjacentVertices[randomEdgeIndex];
    }

    void convertToWorldPositions(out Vector3 fromPos, out Vector3 toPos, out Vector3 orthogonalDirection, int fromNode, int toNode, ref GridMapGenerator map)
    {
            fromPos = map.NodeToWorld(fromNode);
            toPos = map.NodeToWorld(toNode);
            Vector3 roadDirection = (toPos - fromPos).normalized;
            fromPos += roadDirection * 2f; // offset from the start of the road
            toPos -= roadDirection * 2f; // offset from the end of the road

            // Find orthogonal heading to the road
            orthogonalDirection = new Vector3(-roadDirection.z, 0, roadDirection.x);
    }

    Vector3 calcSpawnPosition(Vector3 fromPos, Vector3 toPos, Vector3 orthogonalDirection)
    {
        float axisOffset = axisOffsets[UnityEngine.Random.Range(0, axisOffsets.Length)];
        Vector3 pos = Vector3.Lerp(fromPos, toPos, UnityEngine.Random.Range(0f, 1f));
        pos += orthogonalDirection * axisOffset;

        // Randomly offset the spawn position on either side of the road
        Vector3 randomOffset = orthogonalDirection * UnityEngine.Random.Range(-roadWidth / 2f, roadWidth / 2f);
        pos += randomOffset;

        return pos;
    }

    void initiateSpawnedTrash(int i, Vector3 spawnPosition, ref GameObject[] trashArray)
    {
        int pickTrashIndex = UnityEngine.Random.Range(0, trashArray.Length);
        GameObject trashPrefab = trashArray[pickTrashIndex];
        var trashGO = Instantiate(trashPrefab, spawnPosition, Quaternion.identity, transform);

        // randomly rotate the trash item
        trashGO.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
        trashGO.name = $"TrashItem_{i}";
    }

    void debugSpawnTrash(Vector3 fromPos, Vector3 toPos, Vector3 orthogonalDirection)
    {
        // highlight the orthogonal direction for debugging
        Debug.DrawLine(fromPos + orthogonalDirection * axisOffsets[1], toPos + orthogonalDirection * axisOffsets[1], Color.red, 20f);
        Debug.DrawLine(fromPos - orthogonalDirection * axisOffsets[1], toPos - orthogonalDirection * axisOffsets[1], Color.red, 20f);
        Debug.DrawLine(fromPos + orthogonalDirection * (axisOffsets[1] + roadWidth / 2f), toPos + orthogonalDirection * (axisOffsets[1] + roadWidth / 2f), Color.green, 20f);
        Debug.DrawLine(fromPos + orthogonalDirection * (axisOffsets[1] - roadWidth / 2f), toPos + orthogonalDirection * (axisOffsets[1] - roadWidth / 2f), Color.green, 20f);
        Debug.DrawLine(fromPos - orthogonalDirection * (axisOffsets[1] + roadWidth / 2f), toPos - orthogonalDirection * (axisOffsets[1] + roadWidth / 2f), Color.green, 20f);
        Debug.DrawLine(fromPos - orthogonalDirection * (axisOffsets[1] - roadWidth / 2f), toPos - orthogonalDirection * (axisOffsets[1] - roadWidth / 2f), Color.green, 20f);
    }
}
