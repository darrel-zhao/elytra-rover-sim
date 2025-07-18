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
    public float roadWidth = 5f; // Width of the road area where trash can spawn

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
                adjacentEdges.Add(edge);

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

            // Find orthogonal heading to the road
            Vector3 roadDirection = (toPos - fromPos).normalized;
            Vector3 orthogonalDirection = new Vector3(-roadDirection.z, 0, roadDirection.x);

            Vector3 spawnPosition = Vector3.Lerp(fromPos, toPos, Random.Range(0f, 1f));

            Vector3 offset = orthogonalDirection * Random.Range(-roadWidth / 2f, roadWidth / 2f);
            spawnPosition += offset;

            // Instantiate the trash prefab at the calculated position
            int pickTrashIndex = Random.Range(0, trashArray.Length);
            GameObject trashPrefab = trashArray[pickTrashIndex];
            var trashGO = Instantiate(trashPrefab, spawnPosition, Quaternion.identity, transform);

            // randomly rotate the trash item
            trashGO.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            trashGO.name = $"TrashItem_{i}";
        }
    }
}
