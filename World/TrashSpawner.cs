using System.Collections.Generic;
using QuickGraph;
using UnityEditor;
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
    void Start()
    {
        SpawnTrash();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Spawns trash items throughout the map
    /// /// </summary>

    public void SpawnTrash()
    {
        // Load all trash prefabs labeled "Trash" using Addressables
        List<GameObject> trashList = new List<GameObject>();
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>("Trash", null);

        handle.WaitForCompletion();
        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result.Count == 0)
        {
            Debug.LogError("No trash prefabs found with the 'Trash' label. Please add some prefabs to Addressables.");
            return;
        }
        trashList.AddRange(handle.Result);
        GameObject[] trashArray = trashList.ToArray();

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
            int fromNode = Random.Range(0, map.numNodes);
            var adjacentEdges = new List<TaggedEdge<int, double>>();

            foreach (var edge in map.graph.AdjacentEdges(fromNode))
            {
                if (edge.Target != fromNode)
                    adjacentEdges.Add(edge);
            }

            if (adjacentEdges.Count == 0)
                {
                    Debug.LogWarning($"No adjacent edges found for node {fromNode}.");
                    continue;
                }
            int randomEdgeIndex = Random.Range(0, adjacentEdges.Count);
            int toNode = adjacentEdges[randomEdgeIndex].Target;

            // Randomly select a position on the road using lerp()
            Vector3 fromPos = map.NodeToWorld(fromNode);
            Vector3 toPos = map.NodeToWorld(toNode);
            Vector3 roadDirection = (toPos - fromPos).normalized;
            fromPos += roadDirection * 2f; // offset from the start of the road
            toPos -= roadDirection * 2f; // offset from the end of the road

            // Find orthogonal heading to the road
            Vector3 orthogonalDirection = new Vector3(-roadDirection.z, 0, roadDirection.x);

            // check debugMode
            if (debugMode)
                debugSpawnTrash(fromPos, toPos, orthogonalDirection);

            // Calculate a random position along the road, offset by the orthogonal direction
            // Concentrate trash on sides of the roads; first randomly pick left or right side, then apply offset
            float axisOffset = axisOffsets[Random.Range(0, axisOffsets.Length)];
            Vector3 spawnPosition = Vector3.Lerp(fromPos, toPos, Random.Range(0f, 1f));
            spawnPosition += orthogonalDirection * axisOffset;

            // Randomly offset the spawn position on either side of the road
            Vector3 randomOffset = orthogonalDirection * Random.Range(-roadWidth / 2f, roadWidth / 2f);
            spawnPosition += randomOffset;

            // Instantiate one of the trash prefabs at the calculated position
            int pickTrashIndex = Random.Range(0, trashArray.Length);
            GameObject trashPrefab = trashArray[pickTrashIndex];
            var trashGO = Instantiate(trashPrefab, spawnPosition, Quaternion.identity, transform);

            // randomly rotate the trash item
            trashGO.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            trashGO.name = $"TrashItem_{i}";
        }
    }

    public void debugSpawnTrash(Vector3 fromPos, Vector3 toPos, Vector3 orthogonalDirection)
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
