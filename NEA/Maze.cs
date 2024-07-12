using System;
using System.Collections.Generic;
using System.Linq;

namespace NEA
{
    public class Maze
    {
        private static Random r;
        private readonly int Xsize;
        private readonly int Ysize;
        private int endPoint = -1;
        Dir[] directions = { Dir.up, Dir.right, Dir.down, Dir.left };
        private readonly Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();
        public Maze(int sizeIn, Random ran)
        {
            r = ran;
            Xsize = sizeIn * 2;
            Ysize = sizeIn;
        }
        public void displayGraph(List<IVisible> objects)
        {
            ConsoleColor borderColour = ConsoleColor.Black;
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
                    bool isObject = false;
                    foreach (IVisible obj in objects)
                    {
                        if (obj.getPosition() == nodeNum)
                        {
                            if (obj.getType() == "Player")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                isObject = true;
                            }
                            else if (obj.getType() == "Enemy")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                isObject = true;
                            }
                            else if (obj.getType() == "Power-up")
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                isObject = true;
                            }
                        }
                    }
                    if (nodeNum == endPoint)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        isObject = true;
                    }
                    if (isObject)
                    {
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
                Console.Write("██\n██");
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
                        // The wall diagonally between nodes
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
                foreach (Dir d in directions)
                {
                    try
                    {
                        adjList[nodeNum].Add(getDirection(nodeNum, d));
                    }
                    catch (NotInListException) { }
                }
            }
            return adjList;
        }
        public void generateMaze(int startNode, List<IVisible> objects)
        {
            // Creating and filling up visited with falses
            List<bool> visited = new List<bool>();
            for (int i = 0; i < adjList.Count(); i++)
                visited.Add(false);
            // Running recursive backtracking
            recursiveBacktracking(startNode, ref visited, objects);
            for (int i = 0; i < Xsize * Ysize; i++)
            {
                // Checks for dead ends, breaks the ends.

                // Use this one to have some dead ends still
                // int node = getRandom(Xsize * Ysize - 1);

                // Use this one for all dead ends to be removed
                int node = i;
                if (adjList[node].Count() == 3)
                {
                    foreach (Dir d in directions)
                    {
                        try
                        {
                            if (!adjList[node].Contains(getDirection(node, d)))
                            {
                                removeEdge(node, getDirection(node, directions[((int)d + 2) % 4]));
                            }
                        }
                        catch (NotInListException) { }
                    }
                }
            }
            makeEndPoint(startNode);
            return;
        }
        public void recursiveBacktracking(int startNode, ref List<bool> visited, List<IVisible> objects)
        {
            List<int> nodeEdges = randomize(adjList[startNode]);
            visited[startNode] = true;
            foreach (int i in nodeEdges.ToList())
            {
                if (!visited[i])
                {
                    removeEdge(startNode, i);
                    recursiveBacktracking(i, ref visited, objects);
                }
            }
            return;
        }
        public bool removeEdge(int node1, int node2)
        {
            if (!adjList.ContainsKey(node1) || !adjList.ContainsKey(node2))
            {
                // Console.WriteLine($"NotInDict");
                return false;
            }
            if (adjList[node1].Contains(node2))
            {
                adjList[node1].Remove(node2);
            }
            else
            {
                // Console.WriteLine($"NotInList. {node1}, {node2}");
                return false;
            }
            if (adjList[node2].Contains(node1))
            {
                adjList[node2].Remove(node1);
            }
            else
            {
                // Console.WriteLine($"NotInList. {node1}, {node2}");
                return false;
            }
            return true;

        }
        public bool addEdge(int node1, int node2)
        {
            if (!adjList.ContainsKey(node1) || !adjList.ContainsKey(node2))
            {
                Console.WriteLine($"NotInDict");
                return false;
            }
            if (!adjList[node1].Contains(node2))
            {
                adjList[node1].Add(node2);
            }
            else
            {
                Console.WriteLine($"AlreadyInList. {node1}, {node2}");
                return false;
            }
            if (!adjList[node2].Contains(node1))
            {
                adjList[node2].Add(node1);
            }
            else
            {
                Console.WriteLine($"AlreadyInList. {node1}, {node2}");
                return false;
            }
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
                (ints[ran], ints[i]) = (ints[i], ints[ran]);
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
        public int getDirection(int node, Dir d)
        {
            switch (d)
            {
                case Dir.left:
                    return getXcoordinate(node) != 0 ? node - 1 : throw new NotInListException();
                case Dir.right:
                    return getXcoordinate(node) < Xsize - 1 ? node + 1 : throw new NotInListException();
                case Dir.up:
                    return getYcoordinate(node) != 0 ? node - Xsize : throw new NotInListException();
                case Dir.down:
                    return getYcoordinate(node) != Ysize - 1 ? node + Xsize : throw new NotInListException();
            }
            throw new NotInListException();
        }
        public int getRandom(int maxNum)
        {
            return r.Next(maxNum + 1);
        }
        public int getEndPoint()
        {
            return endPoint;
        }
        public void makeEndPoint(int startPoint)
        {
            bool validEndPoint = false;
            endPoint = 0;
            while (!validEndPoint)
            {
                endPoint = getRandom(Xsize * Ysize);
                // Checks if the x and y coordinate separatley are at least a third of the grid away from the start point
                if (Math.Abs(getXcoordinate(endPoint) - getXcoordinate(startPoint)) >= Xsize / 3 && Math.Abs(getYcoordinate(endPoint) - getYcoordinate(startPoint)) >= Ysize / 3)
                {
                    validEndPoint = true;
                }
            }
        }
    }
}