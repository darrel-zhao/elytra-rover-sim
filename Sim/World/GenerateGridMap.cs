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
        float scale = 5f; // scale for the grid

        // 2) create intersections 
        foreach (int node in graph.Vertices)
        {
            Vector3 intersection = NodeToWorld(node, scale);
            Instantiate(intersectionPrefab, intersection, Quaternion.identity, transform)
                .name = $"Intersection {node}";

            // print("built intersection: " + node + "\n");
        }

        // 3) create roads
        foreach (var edge in graph.Edges)
        {
            Vector3 a = NodeToWorld(edge.Source, scale);
            Vector3 b = NodeToWorld(edge.Target, scale);

            var road = Instantiate(roadPrefab, (a + b) * 0.5f, Quaternion.identity, transform);
            road.name = $"Road {edge.Source} to {edge.Target}";
            road.transform.LookAt(b);
            
            float length = Vector3.Distance(a, b);
            road.transform.localScale = new Vector3(
                road.transform.localScale.x,
                road.transform.localScale.y,
                length
            );

            // print("built intersection: " + edge.Source + " to " + edge.Target + "\n");
        }
    }

    Vector3 NodeToWorld(int node, float dist)
    {
        float i = (node / width) * dist; // calculate row position
        float j = (node % width) * dist; // calculate column position
        return new Vector3(j, 0f, i);
    }

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
