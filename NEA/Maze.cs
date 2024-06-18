using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace NEA
{
    internal class Maze
    {
        private int Xsize;
        private int Ysize;
        Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();
        public Maze(int sizeIn)
        {
            Xsize = sizeIn * 2;
            Ysize = sizeIn;
        }
        public Maze(int XSize, int YSize)
        {
            Xsize = XSize;
            Ysize = YSize;
        }
        public void displayGraph()
        {
            // Top of the border
            for (int i = 0; i < Xsize * 2 + 1; i++)
            {
                Console.Write("_");
            }
            for (int y = 0; y < Ysize; y++)
            {
                Console.Write("\n|");
                // Writes each node and then a dash if there is an edge with the node to the right of it
                for (int x = 0; x < Xsize; x++)
                {
                    int nodeNum = y * Xsize + x;
                    Console.Write("o");
                    if (adjList[nodeNum].Contains(nodeNum + 1))
                    {
                        Console.Write("-");
                    }
                    else Console.Write(" ");
                }
                Console.Write("|\n|");
                // If it's the last line, it writes the bottom of the border
                if (y == Ysize - 1)
                {
                    for (int i = 0; i < Xsize * 2; i++)
                    {
                        Console.Write("_");
                    }
                }
                else
                {
                    // Writes the vertical layer between nodes, has a dash if the nodes above and below it share a conection.
                    for (int x = 0; x < Xsize; x++)
                    {
                        // Makes itself the node above where it will place the bar
                        int nodeNum = y * Xsize + x;
                        // If this node shares an edge with the node below it, a bar is added.
                        if (adjList[nodeNum].Contains(nodeNum + Xsize))
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
            for (int nodeNum = 0; nodeNum < Xsize * Ysize; nodeNum++)
            {
                adjList[nodeNum] = new List<int>();
            }
            // Adds connections with all adjacent nodes
            for (int nodeNum = 0; nodeNum < Xsize * Ysize; nodeNum++)
            {
                int y = nodeNum / Xsize;
                int x = nodeNum - (y * Xsize);
                // Checks if it can place an edge to the left
                if (x != 0)
                {
                    adjList[nodeNum].Add(nodeNum - 1);
                }
                // Checks if it can place an edge above
                if (y != 0)
                {
                    adjList[nodeNum].Add(nodeNum - Xsize);
                }
                // Checks if it can place an edge to the right
                if (x != Xsize - 1)
                {
                    adjList[nodeNum].Add(nodeNum + 1);
                }
                // Checks if it can place an edge below
                if (y != Ysize - 1)
                {
                    adjList[nodeNum].Add(nodeNum + Xsize);
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
        public bool removeEdge(int node1, int node2)
        {
            if (!adjList.ContainsKey(node1) || !adjList.ContainsKey(node2))
                return false;
            if (adjList[node1].Contains(node2))
            {
                adjList[node1].Remove(node2);
            }
            else
                return false;

            if (adjList[node2].Contains(node1))
            {
                adjList[node2].Remove(node1);
            }
            else 
                return false;
            return true;
            
        }
        public List<int> getEdges(int node)
        {
            return adjList[node];
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
        public Dictionary<int, List<int>> getAdjList()
        {
            return adjList;
        }
        public int getXsize()
        {
            return Xsize;
        }
        public int getYsize()
        {
            return Ysize;
        }
    }
}
