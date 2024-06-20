using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NEA_testing
{
    public class Maze
    {
        private static Random r = new Random();
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
            ConsoleColor borderColour = ConsoleColor.DarkMagenta;
            ConsoleColor wallColour = ConsoleColor.Black;
            // Top of the border
            Console.ForegroundColor = borderColour;
            for (int i = 0; i < Xsize * 2 + 1; i++)
            {
                Console.Write("██");
            }
            Console.ForegroundColor = wallColour;
            for (int y = 0; y < Ysize; y++)
            {
                // Writes the left-hand side of the border
                Console.ForegroundColor = borderColour;
                Console.Write("\n██");
                Console.ForegroundColor = wallColour;
                // Writes each node and then a blank space if there is an edge with the node to the right of it
                for (int x = 0; x < Xsize; x++)
                {
                    int nodeNum = y * Xsize + x;
                    // Writing node
                    if (nodeNum == currentNode)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("██");
                        Console.ForegroundColor = wallColour;
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    // To the right of the node
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
                Console.ForegroundColor = borderColour;
                if (y == Ysize - 1)
                {
                    Console.Write("\n██");
                }
                else
                {
                    Console.Write("██\n██");
                }
                Console.ForegroundColor = wallColour;
                // If it's the last line, it writes the bottom of the border
                if (y == Ysize - 1)
                {
                    Console.ForegroundColor = borderColour;
                    for (int i = 0; i < Xsize * 2; i++)
                    {
                        Console.Write("██");
                    }
                    Console.ForegroundColor = wallColour;
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
                        // The right hand border of the maze
                        if (x == Xsize - 1)
                        {
                            Console.ForegroundColor = borderColour;
                        }
                        Console.Write("██");
                        Console.ForegroundColor = wallColour;
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
                // Checks if it can place an edge to the left
                if (getLeft(nodeNum) != -1)
                {
                    adjList[nodeNum].Add(getLeft(nodeNum));
                }
                // Checks if it can place an edge above
                if (getYcoordinate(nodeNum) > 0)
                {
                    adjList[nodeNum].Add(getUp(nodeNum));
                }
                // Checks if it can place an edge to the right
                if (getRight(nodeNum) != -1)
                {
                    adjList[nodeNum].Add(getRight(nodeNum));
                }
                // Checks if it can place an edge below
                if (getYcoordinate(nodeNum) < Ysize - 1)
                {
                    adjList[nodeNum].Add(getDown(nodeNum));
                }
            }
            return adjList;
        }
        public void recursiveBacktrackingConstructor(int startNode)
        {
            List<bool> visited = new List<bool>();
            for (int i = 0; i < adjList.Count(); i++)
                visited.Add(false);
            recursiveBacktracking(startNode, ref visited);
            return;
        }
        public void recursiveBacktracking(int startNode, ref List<bool> visited)
        {
            List<int> nodeEdges = randomize(adjList[startNode]);

            visited[startNode] = true;

            // Console.SetCursorPosition(0, 0);
            foreach (int i in nodeEdges.ToList())
            {
                if (!visited[i])
                {
                    removeEdge(startNode, i);
                    // displayGraph(startNode);
                    recursiveBacktracking(i, ref visited);
                }
            }
            return;
        }
        public bool removeEdge(int node1, int node2)
        {
            if (!adjList.ContainsKey(node1) || !adjList.ContainsKey(node2))
            {
                Console.WriteLine($"NotInDict");
                return false;
            }
            if (adjList[node1].Contains(node2))
            {
                adjList[node1].Remove(node2);
            }
            else
            {
                Console.WriteLine($"NotInList. {node1}, {node2}");
                return false;
            }
            if (adjList[node2].Contains(node1))
            {
                adjList[node2].Remove(node1);
            }
            else
            {
                Console.WriteLine($"NotInList. {node1}, {node2}");
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
        public int getXcoordinate(int node)
        {
            return node % Xsize;
        }
        public int getYcoordinate(int node)
        {
            return node / Xsize;
        }
        public int getLeft(int node)
        {
            if (getXcoordinate(node) != 0)
            {
                return node - 1;
            }
            return -1;
        }
        public int getRight(int node)
        {
            if (getXcoordinate(node) < Xsize - 1)
            {
                return node + 1;
            }
            return -1;
        }
        public int getUp(int node)
        {
            if (getYcoordinate(node) != 0)
            {
                return node - Xsize;
            }
            return -1;
        }
        public int getDown(int node)
        {
            if (getYcoordinate(node) != Ysize - 1)
            {
                return node + Xsize;
            }
            return -1;
        }
    }
}