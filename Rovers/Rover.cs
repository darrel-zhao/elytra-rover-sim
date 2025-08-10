using QuickGraph;
using UnityEngine;
using System.Collections.Generic;

namespace Sim.Rover
{
    public class Rover
    {
        public int id{ get; private set; }
        public int startNode{ get; private set; }
        public int goalNode{ get; private set; }
        public int trashCollected{ get; private set; }
        public Queue<int> path{ get; private set; }
        public UndirectedGraph<int, TaggedEdge<int, double>> graph;

        public Rover(int id, int startNode, int goalNode, UndirectedGraph<int, TaggedEdge<int, double>> graph)
        {
            this.id = id;
            this.startNode = startNode;
            this.goalNode = goalNode;
            trashCollected = 0;
            path = new Queue<int>();
            this.graph = graph;
        }

        public bool ComputePath(int startNode, int goalNode)
        {
            if (graph == null || !graph.ContainsVertex(startNode) || !graph.ContainsVertex(goalNode))
            {
                Debug.LogError("Graph not assigned or invalid nodes");
                return false;
            }

            Dictionary<int, int> cameFrom = new Dictionary<int, int>();
            Queue<int> frontier = new Queue<int>();
            frontier.Enqueue(startNode);
            cameFrom[startNode] = -1;

            while (frontier.Count > 0)
            {
                int current = frontier.Dequeue();

                if (current == goalNode)
                    break;

                foreach (var edge in graph.AdjacentEdges(current))
                {
                    int neighbor = edge.GetOtherVertex(current);

                    if (!cameFrom.ContainsKey(neighbor))
                    {
                        frontier.Enqueue(neighbor);
                        cameFrom[neighbor] = current;
                    }
                }
            }

            if (!cameFrom.ContainsKey(goalNode))
            {
                Debug.LogError("No path found");
                return false;
            }

            // Reconstruct path
            Stack<int> reversePath = new Stack<int>();
            int node = goalNode;
            while (node != -1)
            {
                reversePath.Push(node);
                node = cameFrom[node];
            }

            path.Clear();
            while (reversePath.Count > 0)
                path.Enqueue(reversePath.Pop());

            Debug.Log($"Path computed: {string.Join(" -> ", path)}");

            return path != null && path.Count > 0;
        }


    }
}