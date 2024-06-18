using System;
using System.Collections.Generic;
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
                    Console.Write("+");
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
            // The stack will hold the integer number of each node
            Stack<int> stack = new Stack<int>();
            // Parralel stack which will hold the edge that that node shares that wants removing
            Stack<int> connectedNodes = new Stack<int>();
            // nodeEdges will hold the nodes that the curent node connects to
            List<int> nodeEdges = adjList[startNode];
            // Visited will hold the boolean status of whether each node has been visited by the node's status in it's corresponing index.
            // For example, to see if node number 43 has been visited, look at visited[43]
            List<bool> visited = new List<bool>();
            // Filling visited with all false
            for (int i = 0; i < adjList.Count(); i++)
            {
                visited.Add(false);
            }
            visited[startNode] = true;
            // Randomising nodeEdges and adding all items to the stack
            nodeEdges = randomize(nodeEdges);
            foreach (int nodeNum in nodeEdges)
            {
                stack.Push(nodeNum);
                connectedNodes.Push(0);
            }
            // Actual recursive backtracking loop
            while (stack.Count() > 0)
            {
                // Takes the current node and it's connection from the top of the stack
                int currentNode = stack.Pop();
                int connectedNode = connectedNodes.Pop();
                // Retrieves and randomizes all connected nodes
                nodeEdges = adjList[currentNode];
                nodeEdges = randomize(nodeEdges);
                // For each edge, check if that node has already been visited.
                // If not, add it and the current node (it's connection) to the stack.
                foreach (int nodeNum in nodeEdges)
                {
                    if (!visited[nodeNum])
                    {
                        stack.Push(nodeNum);
                        connectedNodes.Push(currentNode);
                        visited[nodeNum] = true;
                    }
                }
                // Remove the edge between the current node and the last node.
                removeEdge(currentNode, connectedNode);
                // Console.Clear();
                // displayGraph();
                Thread.Sleep(1);
            }
            return adjList;
        }
        public bool removeEdge(int node1, int node2)
        {
            if (!adjList.ContainsKey(node1) || !adjList.ContainsKey(node2))
            {
                return false;
            }
            if (adjList[node1].Contains(node2))
            {
                adjList[node1].Remove(node2);
            }
            else
            {
                return false;
            }
            if (adjList[node2].Contains(node1))
            {
                adjList[node2].Remove(node1);
            }
            else
            {
                return false;
            }
            return true;
            
        }
        public bool addEdge(int node1, int node2)
        {
            if (!adjList.ContainsKey(node1) || !adjList.ContainsKey(node2))
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
            Random r = new Random();
            for (int i = ints.Count - 1; i > 1; i--)
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
