using QuickGraph;

namespace Sim.World
{
    public class World
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public UndirectedGraph<int, TaggedEdge<int, double>> _graph { get; set; }

        public World(int height, int width)
        {
            Height = height;
            Width = width;
            _graph = new UndirectedGraph<int, TaggedEdge<int, double>>(false);
        }

        public UndirectedGraph<int, TaggedEdge<int, double>> generateGridWorld()
        {
            for (int i = 0; i < Height * Width; i++)
            {
                _graph.AddVertex(i);
            }
            
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int currentNode = i * Width + j;
                    _graph.AddVertex(currentNode);

                    // Add edges to the right and down neighbors
                    if (j < Width - 1) // Right neighbor
                    {
                        int rightNeighbor = currentNode + 1;
                        _graph.AddEdge(new TaggedEdge<int, double>(currentNode, rightNeighbor, 1.0));
                    }
                    if (i < Height - 1) // Down neighbor
                    {
                        int downNeighbor = currentNode + Width;
                        _graph.AddEdge(new TaggedEdge<int, double>(currentNode, downNeighbor, 1.0));
                    }
                }
            }

            return _graph;
        }


    }
}