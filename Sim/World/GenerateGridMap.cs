using UnityEngine;
using Sim.World;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

[ExecuteAlways]

public class GridMapGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 10;
    public int height = 10;

    [Header("Grid Prefab")]
    public GameObject intersectionPrefab;
    public GameObject roadPrefab;

    float scale = 30f; // scale for the grid
    void Start()
    {
        BuildMap();
    }

    #if UNITY_EDITOR
        void OnValidate()
        {
            if (!Application.isPlaying)
            {
                BuildMap();
            }
        }
    #endif

    void BuildMap()
    {
        // clear old map
        ClearMap();

        // generate graph
        var world = new World(height, width);
        var graph = world.generateGridWorld();

        // 2) create intersections 
        foreach (int node in graph.Vertices)
        {
            Vector3 intersection = NodeToWorld(node);
            Instantiate(intersectionPrefab, intersection, Quaternion.identity, transform)
                .name = $"Intersection {node}";
        }

        // 3) create roads
        foreach (var edge in graph.Edges)
            SpawnRoad(edge.Source, edge.Target);
    }

    // ***HELPER FUNCTIONS*** //

    /// <summary>
    /// Converts a node index into a world position.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="dist"></param>
    /// <returns>a 3D position (Vector3)</returns>
    public Vector3 NodeToWorld(int node)
    {
        float i = (node / width) * scale; // calculate row position
        float j = (node % width) * scale; // calculate column position
        return new Vector3(j, 0f, i);
    }

/// <summary>
/// Spawns a road between two nodes in the grid. Handles the rotation and scaling of the road prefab.
/// </summary>
/// <param name="src"></param>
/// <param name="dst"></param>
/// <param name="scale"></param>
    void SpawnRoad(int src, int dst)
    {
        Vector3 a = NodeToWorld(src);
        Vector3 b = NodeToWorld(dst);

        var road = Instantiate(roadPrefab, (a + b) * 0.5f, Quaternion.identity, transform);
        road.name = $"Road {src} to {dst}";
        road.transform.rotation =
            Quaternion.LookRotation(b - a)
            * Quaternion.Euler(0, 90, 0); // rotate to face the direction of the road

        float length = Vector3.Distance(a, b) - 4f;
        road.transform.localScale = new Vector3(
            length * 0.1f,
            road.transform.localScale.y,
            road.transform.localScale.z
        );

        // Render material using MaterialPropertyBlock
        var renderer = road.GetComponent<Renderer>();
        var block = new MaterialPropertyBlock();

        // Get the current property block
        renderer.GetPropertyBlock(block);

        // Edit it, then set it back onto the renderer
        block.SetVector("_BaseColorMap_ST", new Vector4(length / 4, 1f, 0f, 0f));
        renderer.SetPropertyBlock(block);
    }

/// <summary>
/// Clears the current map by destroying all child objects of this GameObject.
/// This is useful for regenerating the map without creating duplicates.
/// </summary>
    void ClearMap()
    {
        // iterate backwards to avoid indexing issues when removing children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            print("destroying child: " + i + "\n");
            var c = transform.GetChild(i).gameObject;
            #if UNITY_EDITOR
                DestroyImmediate(c);
            #else
                Destroy(c);
            #endif
        }
    }
}
