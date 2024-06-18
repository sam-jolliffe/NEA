using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Program
    {
        public static Dictionary<int, List<int>> createGraph(int size)
        {
            // Make an adjacency list so that each node shares an edge with all adjacent nodes
            Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();
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
        public static void displayGraph(Dictionary<int, List<int>> adjList, int size)
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
        public static Dictionary<int, List<int>> recursiveBacktracking(Dictionary<int, List<int>> adjList, int size, int startNode)
        {
            Stack<int> stack = new Stack<int>();
            List<int> copyOfNode = adjList[startNode];
            return adjList;
        }
        public static List<int> randomize(List<int> ints)
        {
            Random r = new Random();
            List<int> retrunList = new List<int>();
            for (int i = ints.Count; i > 0; i--)
            {
                // i = size of list left
                r.Next(0, i + 1);
            }
            return ints;
        }
        static void Main(string[] args)
        {
            int size = 3;
            Dictionary<int, List<int>> adjList = createGraph(size);
            adjList[4] = new List<int>();
            displayGraph(adjList, size);
            List<int> contains = adjList[5];
            Console.ReadKey();
        }
    }
}
