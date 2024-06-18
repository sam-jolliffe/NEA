using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace NEA
{
    internal class Maze
    {
        public int size;
        Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();
        public Maze(int sizeIn)
        {
            size = sizeIn;
        }
        public void displayGraph()
        {
            // Top of the border
            for (int i = 0; i < size * 2 + 1; i++)
            {
                Console.Write("_");
            }
            for (int y = 0; y < size; y++)
            {
                Console.Write("\n|");
                // Writes each node and then a dash if there is an edge with the node to the right of it
                for (int x = 0; x < size; x++)
                {
                    int nodeNum = y * size + x;
                    Console.Write("o");
                    if (adjList[nodeNum].Contains(nodeNum + 1))
                    {
                        Console.Write("-");
                    }
                    else Console.Write(" ");
                }
                Console.Write("|\n|");
                // If it's the last line, it writes the bottom of the border
                if (y == size - 1)
                {
                    for (int i = 0; i < size * 2; i++)
                    {
                        Console.Write("_");
                    }
                }
                else
                {
                    // Writes the vertical layer between nodes, has a dash if the nodes above and below it share a conection.
                    for (int x = 0; x < size; x++)
                    {
                        int nodeNum = y * size + x;
                        if (adjList[nodeNum].Contains(nodeNum + size))
                        {
                            Console.Write("|");
                        }
                        else Console.Write(" ");
                        Console.Write(" ");
                    }
                }
                Console.Write("|");
            }
        }
        public Dictionary<int, List<int>> createGraph()
        {
            // Fills the dictionary so that each node has an associated adjacency list;
            for (int nodeNum = 0; nodeNum < size * size; nodeNum++)
            {
                adjList[nodeNum] = new List<int>();
            }
            // Adds connections with all adjacent nodes
            for (int nodeNum = 0; nodeNum < size * size; nodeNum++)
            {
                int y = nodeNum / size;
                int x = nodeNum - (y * size);
                // Checks if it can place an edge to the left
                if (x != 0)
                {
                    adjList[nodeNum].Add(nodeNum - 1);
                }
                // Checks if it can place a node above
                if (y != 0)
                {
                    adjList[nodeNum].Add(nodeNum - size);
                }
                // Checks if it can place a node to the right
                if (x != size - 1)
                {
                    adjList[nodeNum].Add(nodeNum + 1);
                }
                // Checks if it can place a node below
                if (y != size - 1)
                {
                    adjList[nodeNum].Add(nodeNum + size);
                }
            }
            return adjList;
        }
        public Dictionary<int, List<int>> recursiveBacktracking(int startNode)
        {
            Stack<int> stack = new Stack<int>();
            List<int> copyOfNode = adjList[startNode];
            return adjList;
        }
        public List<int> randomize(List<int> ints)
        {
            Random r = new Random();
            List<int> returnList = new List<int>();
            for (int i = ints.Count; i > 0; i--)
            {
                // i = size of list left
                r.Next(0, i + 1);
            }
            return returnList;
        }
    }
}
