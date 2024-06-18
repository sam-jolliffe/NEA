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
            Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();
            // Make an adjacency list so that each node shares an edge with all adjacent nodes
            for (int nodeNum = 0; nodeNum < size * size; nodeNum++)
            {
                adjList[nodeNum] = new List<int>();
            }
            for (int nodeNum = 0; nodeNum < size * size; nodeNum++)
            {
                int y = nodeNum / size;
                int x = nodeNum - (y * size);
                Console.Write($" x: {nodeNum}, y: {y}");
                if (x != 0)
                {
                    adjList[nodeNum].Add(nodeNum - 1);
                    Console.Write("y");
                }
                if (y != 0)
                {
                    adjList[nodeNum].Add(nodeNum - size);
                    Console.Write("y");
                }
                if (x != size - 1)
                {
                    adjList[nodeNum].Add(nodeNum + 1);
                    Console.Write("y");
                }
                if (y != size - 1)
                {
                    adjList[nodeNum].Add(nodeNum + size);
                    Console.Write("y");
                }
                Console.Write("\n");
            }
            return adjList;
        }
        public static void displayGraph(Dictionary<int, List<int>> adjList, int size)
        {
            for (int i = 0; i < size * 2 + 1; i++)
            {
                Console.Write("_");
            }
            for (int y = 0; y < size; y++)
            {
                Console.Write("\n|");
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
                if (y == size - 1)
                {
                    for (int i = 0; i < size * 2; i++)
                    {
                        Console.Write("_");
                    }
                }
                else
                {
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
        static void Main(string[] args)
        {
            int size = 3;
            Dictionary<int, List<int>> adjList = createGraph(size);
            displayGraph(adjList, size);
            List<int> contains = adjList[5];
            Console.ReadKey();
        }
    }
}
