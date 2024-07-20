using System;
using System.Collections.Generic;
using System.Linq;

namespace NEA
{
    public class Maze
    {
        private readonly bool testing = false;
        private static Random r;
        private readonly int Xsize;
        private readonly int Ysize;
        private int endPoint = -1;
        private int keyPosition;
        private List<int> treasureRoomNodes = new List<int>();
        private List<int> allRoomsNodes = new List<int>();
        private readonly Dir[] directions = { Dir.up, Dir.right, Dir.down, Dir.left };
        private readonly Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();
        public Maze(int sizeIn, Random ran)
        {
            r = ran;
            Xsize = sizeIn * 2;
            Ysize = sizeIn;
        }
        public void displayGraph(List<IVisible> objects)
        {
            // ██ signifies a wall
            // '  ' signifies a coridoor
            int playerPos = 0;
            foreach (IVisible obj in objects)
            {
                if (obj.getType() == "Player")
                {
                    playerPos = obj.getPosition();
                }
            }
            List<int> visibleNodes = new List<int> { playerPos };
            if (!testing)
            {
                // visibleNodes = depthFirst(playerPos, 0, visibleNodes);
                visibleNodes = getVisibleNodes(playerPos);
            }
            else
            {
                for (int i = 0; i < Xsize * Ysize; i++)
                {
                    visibleNodes.Add(i);
                }
            }
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
                // Writes each node 
                for (int x = 0; x < Xsize; x++)
                {
                    int nodeNum = y * Xsize + x;
                    // Writing node
                    bool isObject = false;
                    if (visibleNodes.Contains(nodeNum))
                    {
                        foreach (IVisible obj in objects)
                        {
                            if (obj.getPosition() == nodeNum)
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                                isObject = true;
                                switch (obj.getType())
                                {
                                    case "Power-up":
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write("°°");
                                        break;
                                    case "Enemy":
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Write("╬╬");
                                        break;
                                    case "Key":
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Console.Write("██");
                                        break;
                                    case "Player":
                                        playerPos = obj.getPosition();
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        Console.Write("██");
                                        break;
                                }
                            }
                        }
                        if (nodeNum == endPoint)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("██");
                            isObject = true;
                        }
                        if (isObject)
                        {
                            Console.ForegroundColor = wallColour;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("██");
                        }
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("  ");
                    }
                    Console.BackgroundColor = ConsoleColor.White;
                    // To the right of the node
                    if (x != Xsize - 1)
                    {
                        if (!visibleNodes.Contains(nodeNum) || !visibleNodes.Contains(nodeNum + 1))
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        if (adjList[nodeNum].Contains(nodeNum + 1))
                        {
                            Console.Write("██");
                        }
                        else
                        {
                            Console.Write("  ");
                        }
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                }
                // Writes the right then left hand of the border
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
                    // Writes the vertical layer between nodes, has a coridoor if the nodes above and below it share a conection.
                    for (int x = 0; x < Xsize; x++)
                    {
                        // Makes itself the node above where it will write the piece
                        int nodeNum = y * Xsize + x;
                        // If this node shares an edge with the node below it, a coridoor is added.
                        if (!adjList[nodeNum].Contains(nodeNum + Xsize) && visibleNodes.Contains(nodeNum) && visibleNodes.Contains(nodeNum + Xsize))
                        {
                            Console.Write("  ");
                        }
                        else
                        {
                            Console.Write("██");
                        }
                        Console.BackgroundColor = ConsoleColor.White;
                        // The wall diagonally between nodes
                        if (allRoomsNodes.Contains(nodeNum) && allRoomsNodes.Contains(getDirection(nodeNum, Dir.right)) && allRoomsNodes.Contains(getDirection(nodeNum, Dir.down)) && allRoomsNodes.Contains(getDirection(getDirection(nodeNum, Dir.right), Dir.down)) && visibleNodes.Contains(nodeNum))
                        {
                            Console.Write("  ");
                        }
                        else Console.Write("██");
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
            recursiveBacktracking(startNode, ref visited);
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
            makeRooms(ref objects);
            makeEndPoint(startNode);

        }
        public void recursiveBacktracking(int startNode, ref List<bool> visited)
        {
            List<int> nodeEdges = randomize(adjList[startNode]);
            visited[startNode] = true;
            foreach (int i in nodeEdges.ToList())
            {
                if (!visited[i])
                {
                    removeEdge(startNode, i);
                    recursiveBacktracking(i, ref visited);
                }
            }
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
        public List<int> depthFirst(int node, int count, List<int> visited)
        {
            count++;
            if (count >= 15) return visited;
            visited.Add(node);
            List<int> adjacents = new List<int>();
            foreach (Dir d in directions)
            {
                try
                {
                    adjacents.Add(getDirection(node, d));
                }
                catch (NotInListException) { }
            }
            foreach (int i in adjacents)
            {
                if (!adjList[node].Contains(i))
                {
                    visited = depthFirst(i, count, visited);
                }
            }
            return visited;
        }
        public void makeRooms(ref List<IVisible> objects)
        {
            // Treasure room:
            int Node = 0;
            bool isValid = false;
            // Node cannot be on one of the borders, as i'm going to dig out a 3x3 around it.
            while (!isValid)
            {
                Node = r.Next(0, Xsize * Ysize);
                //      Top row                               Bottom row                        Left column                       Right column
                if (!(getYcoordinate(Node) <= 1 || getYcoordinate(Node) >= Ysize - 2 || getXcoordinate(Node) <= 1 || getXcoordinate(Node) >= Xsize - 2))
                {
                    isValid = true;
                }
            }
            removeEdge(getDirection(getDirection(Node, Dir.left), Dir.up), getDirection(Node, Dir.up)); // Top left, top middle
            removeEdge(getDirection(Node, Dir.up), getDirection(getDirection(Node, Dir.right), Dir.up)); // Top middle, top right
            removeEdge(getDirection(getDirection(Node, Dir.left), Dir.up), getDirection(Node, Dir.left)); // Top left, middle left
            removeEdge(getDirection(Node, Dir.up), Node); // Top middle, middle
            removeEdge(getDirection(getDirection(Node, Dir.right), Dir.up), getDirection(Node, Dir.right)); // Top right, middle right
            removeEdge(getDirection(Node, Dir.left), Node); // Middle left, middle
            removeEdge(Node, getDirection(Node, Dir.right)); // Middle, middle right
            removeEdge(getDirection(Node, Dir.left), getDirection(getDirection(Node, Dir.left), Dir.down)); // Middle left, bottom left
            removeEdge(Node, getDirection(Node, Dir.down)); // Middle, bottom middle
            removeEdge(getDirection(Node, Dir.right), getDirection(getDirection(Node, Dir.right), Dir.down)); // Middle right, bottom right
            removeEdge(getDirection(getDirection(Node, Dir.left), Dir.down), getDirection(Node, Dir.down)); // Bottom left, bottom middle
            removeEdge(getDirection(Node, Dir.down), getDirection(getDirection(Node, Dir.right), Dir.down)); // Bottom middle, bottom right
            treasureRoomNodes = new List<int>
            {
                Node,
                getDirection(getDirection(Node, Dir.up), Dir.left),
                getDirection(Node, Dir.up),
                getDirection(getDirection(Node, Dir.right), Dir.up),
                getDirection(Node, Dir.left),
                getDirection(Node, Dir.right),
                getDirection(getDirection(Node, Dir.left), Dir.down),
                getDirection(Node, Dir.down),
                getDirection(getDirection(Node, Dir.right), Dir.down)
            };
            allRoomsNodes = new List<int>(treasureRoomNodes);

            // Key room:
            isValid = false;
            while (!isValid)
            {
                Node = r.Next(0, Xsize * Ysize);
                //      Top row                               Bottom row                        Left column                       Right column
                if (!(getYcoordinate(Node) <= 1 || getYcoordinate(Node) >= Ysize - 2 || getXcoordinate(Node) <= 1 || getXcoordinate(Node) >= Xsize - 2) && !allRoomsNodes.Contains(Node))
                {
                    isValid = true;
                }
            }
            removeEdge(getDirection(getDirection(Node, Dir.left), Dir.up), getDirection(Node, Dir.up)); // Top left, top middle
            removeEdge(getDirection(Node, Dir.up), getDirection(getDirection(Node, Dir.right), Dir.up)); // Top middle, top right
            removeEdge(getDirection(getDirection(Node, Dir.left), Dir.up), getDirection(Node, Dir.left)); // Top left, middle left
            removeEdge(getDirection(Node, Dir.up), Node); // Top middle, middle
            removeEdge(getDirection(getDirection(Node, Dir.right), Dir.up), getDirection(Node, Dir.right)); // Top right, middle right
            removeEdge(getDirection(Node, Dir.left), Node); // Middle left, middle
            removeEdge(Node, getDirection(Node, Dir.right)); // Middle, middle right
            removeEdge(getDirection(Node, Dir.left), getDirection(getDirection(Node, Dir.left), Dir.down)); // Middle left, bottom left
            removeEdge(Node, getDirection(Node, Dir.down)); // Middle, bottom middle
            removeEdge(getDirection(Node, Dir.right), getDirection(getDirection(Node, Dir.right), Dir.down)); // Middle right, bottom right
            removeEdge(getDirection(getDirection(Node, Dir.left), Dir.down), getDirection(Node, Dir.down)); // Bottom left, bottom middle
            removeEdge(getDirection(Node, Dir.down), getDirection(getDirection(Node, Dir.right), Dir.down)); // Bottom middle, bottom right
            allRoomsNodes.AddRange(new List<int>
            {
                Node,
                getDirection(getDirection(Node, Dir.up), Dir.left),
                getDirection(Node, Dir.up),
                getDirection(getDirection(Node, Dir.right), Dir.up),
                getDirection(Node, Dir.left),
                getDirection(Node, Dir.right),
                getDirection(getDirection(Node, Dir.left), Dir.down),
                getDirection(Node, Dir.down),
                getDirection(getDirection(Node, Dir.right), Dir.down)
            });
            keyPosition = Node;
        }
        public List<int> getTreasureRoomNodes()
        {
            return treasureRoomNodes;
        }
        public List<int> getAllRoomsNodes()
        {
            return allRoomsNodes;
        }
        public int getKeyPosition()
        {
            return keyPosition;
        }
        public List<int> getVisibleNodes(int Node)
        {
            int radius = 10;
            List<int> nodes = new List<int>();
            for (int i = 0; i < Xsize * Ysize; i++)
            {
                if (Math.Pow(Math.Abs(getXcoordinate(Node) - getXcoordinate(i)), 2) + Math.Pow(Math.Abs(getYcoordinate(Node) - getYcoordinate(i)), 2) <= Math.Pow(radius, 2))
                {
                    nodes.Add(i);
                }
            }
            return nodes;
        }
    }
}