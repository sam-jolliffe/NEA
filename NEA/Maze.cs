using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace NEA
{
    internal class Maze
    {
        private Random r = new Random();
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
        public void displayGraph(int currentNode)
        {
            // Top of the border
            for (int i = 0; i < Xsize * 2 + 1; i++)
            {
                Console.Write("██");
            }
            for (int y = 0; y < Ysize; y++)
            {
                Console.Write("\n██");
                // Writes each node and then a blank space if there is an edge with the node to the right of it
                for (int x = 0; x < Xsize; x++)
                {
                    int nodeNum = y * Xsize + x;
                    // Writing node
                    if (nodeNum == currentNode)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("██");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (adjList[nodeNum].Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("  ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("  ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    // To the right of the node
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (x != Xsize - 1)
                    {
                        if (adjList[nodeNum].Contains(nodeNum + 1))
                        {
                            Console.Write("██");
                        }
                        else
                        {
                            Console.Write("  ");
                        }
                    }
                }
                Console.Write("██\n██");
                // If it's the last line, it writes the bottom of the border
                if (y == Ysize - 1)
                {
                    for (int i = 0; i < Xsize * 2; i++)
                    {
                        Console.Write("██");
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
                            Console.Write("██");
                        }
                        else
                        {
                            Console.Write("  ");
                        }
                        // For the space diagonally between nodes, to have an open space:
                        // The top left node and bottom left node must share an edge AND
                        // The top right node and bottom right node must share an edge OR
                        // The top left node and the top right node share an edge AND
                        // The bottom left node and the bottom right node share an edge
                        // If not true, a corner wall is placed there.
                        if (x != Xsize - 1)
                        {
                            if ((!adjList[nodeNum].Contains(nodeNum + Xsize) && !adjList[nodeNum + 1].Contains(nodeNum + Xsize + 1)) || (!adjList[nodeNum].Contains(nodeNum + 1) && !adjList[nodeNum + Xsize].Contains(nodeNum + Xsize + 1)))
                            {
                                Console.Write("██");
                            }
                            else
                            {
                                // Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("██");
                                // Console.ForegroundColor = ConsoleColor.Gray;
                            }
                        }
                        else
                        {
                            Console.Write("██");
                        }
                    }
                }
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
        public void recursiveBacktrackingConstructor(int startNode)
        {
            Console.WriteLine("Generating maze, please wait...");
            List<bool> visited = new List<bool>();
            for (int i = 0; i < adjList.Count(); i++)
                visited.Add(false);
            recursiveBacktracking(startNode, visited);
            return;
        }
        public List<bool> recursiveBacktracking(int startNode, List<bool> visited)
        {
            List<int> nodeEdge = adjList[startNode];
            List<int> nodeEdges = randomize(nodeEdge);

            visited[startNode] = true;

            Console.SetCursorPosition(0, 0);
            foreach (int i in nodeEdges.ToList())
            {
                if (!visited[i])
                {
                    removeEdge(startNode, i);
                    displayGraph(startNode);
                    visited = recursiveBacktracking(i, visited);
                }
            }
            return visited;
        }
        public bool removeEdge(int node1, int node2)
        {
            if (!adjList.ContainsKey(node1) || !adjList.ContainsKey(node2))
            {
                Console.WriteLine("NotInDict");
                return false;
            }
            if (adjList[node1].Contains(node2))
            {
                adjList[node1].Remove(node2);
            }
            else
            {
                Console.WriteLine("NotInList");
                return false;
            }
            if (adjList[node2].Contains(node1))
            {
                adjList[node2].Remove(node1);
            }
            else
            {
                Console.WriteLine("NotInList");
                return false;
            }
            return true;

        }
        public bool addEdge(int node1, int node2)
        {
            if (adjList.ContainsKey(node1) || adjList.ContainsKey(node2))
                return false;
            if (!adjList[node1].Contains(node2))
            {
                adjList[node1].Add(node2);
            }
            else
                return false;

            if (!adjList[node2].Contains(node1))
            {
                adjList[node2].Add(node1);
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
            // Based on the Fisher-Yates Shuffling algorithm. Article cited in references.
            for (int i = ints.Count - 1; i > 0; i--)
            {
                // i = size of list left
                int ran = r.Next(0, i + 1);
                int temp = ints[i];
                ints[i] = ints[ran];
                ints[ran] = temp;
            }
            return ints;
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
