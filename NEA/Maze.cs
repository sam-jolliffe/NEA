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
        public void DisplayGraph(List<IVisible> objects, int FOV)
        {
            int playerPos = 0;
            int CompassPos = -1;
            foreach (IVisible obj in objects)
            {
                if (obj.GetType() == "Player")
                {
                    playerPos = obj.GetPosition();
                }
            }
            foreach (IVisible obj in objects)
            {
                if (obj.GetName() == "Compass" && ((Compass)obj).GetIsVisible())
                {
                    CompassPos = ((Compass)obj).GetPosition(playerPos, endPoint, FOV);
                }
            }
            List<int> visibleNodes = new List<int> { playerPos };
            if (!testing)
            {
                visibleNodes = GetVisibleNodes(playerPos, FOV);
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
                    bool hasWritten = false;
                    int nodeNum = y * Xsize + x;
                    // Writing node
                    bool isObject = false;
                    if (CompassPos == nodeNum)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("██");
                    }
                    else if (visibleNodes.Contains(nodeNum))
                    {
                        if (nodeNum == endPoint)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("██");
                            isObject = true;
                            hasWritten = true;
                        }
                        foreach (IVisible obj in objects)
                        {
                            if ((obj.GetName() != "Compass" || !((Compass)obj).GetIsVisible()) && obj.GetPosition() == nodeNum && !hasWritten)
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                                isObject = true;
                                Console.ForegroundColor = obj.GetColour();
                                Console.Write(obj.GetSprite());
                                if (obj.GetType() == "Player")
                                {
                                    playerPos = obj.GetPosition();
                                }
                                hasWritten = true;
                            }
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
                        Console.Write("██");
                    }
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    // To the right of the node
                    if (x != Xsize - 1)
                    {
                        if (!adjList[nodeNum].Contains(nodeNum + 1) && visibleNodes.Contains(nodeNum) && visibleNodes.Contains(nodeNum + 1))
                        {
                            Console.Write("  ");
                        }
                        else
                        {
                            Console.Write("██");
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
                        if (allRoomsNodes.Contains(nodeNum) && allRoomsNodes.Contains(GetDirection(nodeNum, Dir.right)) && allRoomsNodes.Contains(GetDirection(nodeNum, Dir.down)) && allRoomsNodes.Contains(GetDirection(GetDirection(nodeNum, Dir.right), Dir.down)) && visibleNodes.Contains(nodeNum))
                        {
                            Console.Write("  ");
                        }
                        else Console.Write("██");
                        Console.ForegroundColor = wallColour;
                    }
                }
            }
        }
        public Dictionary<int, List<int>> CreateGraph()
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
                        adjList[nodeNum].Add(GetDirection(nodeNum, d));
                    }
                    catch (NotInListException) { }
                }
            }
            return adjList;
        }
        public void GenerateMaze(int startNode, List<IVisible> objects)
        {
            // Creating and filling up visited with falses
            List<bool> visited = new List<bool>();
            for (int i = 0; i < adjList.Count(); i++)
                visited.Add(false);
            // Running recursive backtracking
            RecursiveBacktracking(startNode, ref visited);
            BreakDeadEnds();
            MakeRooms(objects);
            MakeEndPoint(startNode);

        }
        public void RecursiveBacktracking(int startNode, ref List<bool> visited)
        {
            List<int> nodeEdges = Randomize(adjList[startNode]);
            visited[startNode] = true;
            foreach (int i in nodeEdges.ToList())
            {
                if (!visited[i])
                {
                    RemoveEdge(startNode, i);
                    RecursiveBacktracking(i, ref visited);
                }
            }
        }
        public bool RemoveEdge(int node1, int node2)
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
        public List<int> Randomize(List<int> ints)
        {
            // Based on the Fisher-Yates Shuffling algorithm. Article cited in references.
            for (int i = ints.Count - 1; i > 0; i--)
            {
                int ran = r.Next(0, i + 1);
                (ints[ran], ints[i]) = (ints[i], ints[ran]);
            }
            return ints;
        }
        public Dictionary<int, List<int>> GetAdjList() => adjList;
        public int GetXsize() => Xsize;
        public int GetYsize() => Ysize;
        public int GetXcoordinate(int node) => node % Xsize;
        public int GetYcoordinate(int node) => node / Xsize;
        public int GetDirection(int node, Dir d)
        {
            switch (d)
            {
                case Dir.left:
                    return GetXcoordinate(node) != 0 ? node - 1 : throw new NotInListException();
                case Dir.right:
                    return GetXcoordinate(node) < Xsize - 1 ? node + 1 : throw new NotInListException();
                case Dir.up:
                    return GetYcoordinate(node) != 0 ? node - Xsize : throw new NotInListException();
                case Dir.down:
                    return GetYcoordinate(node) != Ysize - 1 ? node + Xsize : throw new NotInListException();
            }
            throw new NotInListException();
        }
        public int GetRandom(int maxNum) => r.Next(maxNum + 1);
        public int GetEndPoint() => endPoint;
        public void MakeEndPoint(int startPoint)
        {
            bool validEndPoint = false;
            endPoint = 0;
            while (!validEndPoint)
            {
                endPoint = GetRandom(Xsize * Ysize);
                // Checks if the x and y coordinate separatley are at least a third of the grid away from the start point
                if (Math.Abs(GetXcoordinate(endPoint) - GetXcoordinate(startPoint)) >= Xsize / 3 && Math.Abs(GetYcoordinate(endPoint) - GetYcoordinate(startPoint)) >= Ysize / 3)
                {
                    validEndPoint = true;
                }
            }
        }
        public void MakeRooms(List<IVisible> objects)
        {
            // Treasure room:
            int Node = 0;
            bool isValid = false;
            // Node cannot be on one of the borders, as i'm going to dig out a 3x3 around it.
            List<int> objectPositions = new List<int>();
            foreach (IVisible obj in objects)
            {
                objectPositions.Add(obj.GetPosition());
            }
            while (!isValid)
            {
                Node = r.Next(0, Xsize * Ysize);
                //      Top row                               Bottom row                        Left column                       Right column
                if (!(GetYcoordinate(Node) <= 1 || GetYcoordinate(Node) >= Ysize - 2 || GetXcoordinate(Node) <= 1 || GetXcoordinate(Node) >= Xsize - 2) && !objectPositions.Contains(Node))
                {
                    isValid = true;
                }
            }
            RemoveEdge(GetDirection(GetDirection(Node, Dir.left), Dir.up), GetDirection(Node, Dir.up)); // Top left, top middle
            RemoveEdge(GetDirection(Node, Dir.up), GetDirection(GetDirection(Node, Dir.right), Dir.up)); // Top middle, top right
            RemoveEdge(GetDirection(GetDirection(Node, Dir.left), Dir.up), GetDirection(Node, Dir.left)); // Top left, middle left
            RemoveEdge(GetDirection(Node, Dir.up), Node); // Top middle, middle
            RemoveEdge(GetDirection(GetDirection(Node, Dir.right), Dir.up), GetDirection(Node, Dir.right)); // Top right, middle right
            RemoveEdge(GetDirection(Node, Dir.left), Node); // Middle left, middle
            RemoveEdge(Node, GetDirection(Node, Dir.right)); // Middle, middle right
            RemoveEdge(GetDirection(Node, Dir.left), GetDirection(GetDirection(Node, Dir.left), Dir.down)); // Middle left, bottom left
            RemoveEdge(Node, GetDirection(Node, Dir.down)); // Middle, bottom middle
            RemoveEdge(GetDirection(Node, Dir.right), GetDirection(GetDirection(Node, Dir.right), Dir.down)); // Middle right, bottom right
            RemoveEdge(GetDirection(GetDirection(Node, Dir.left), Dir.down), GetDirection(Node, Dir.down)); // Bottom left, bottom middle
            RemoveEdge(GetDirection(Node, Dir.down), GetDirection(GetDirection(Node, Dir.right), Dir.down)); // Bottom middle, bottom right
            treasureRoomNodes = new List<int>
            {
                Node,
                GetDirection(GetDirection(Node, Dir.up), Dir.left),
                GetDirection(Node, Dir.up),
                GetDirection(GetDirection(Node, Dir.right), Dir.up),
                GetDirection(Node, Dir.left),
                GetDirection(Node, Dir.right),
                GetDirection(GetDirection(Node, Dir.left), Dir.down),
                GetDirection(Node, Dir.down),
                GetDirection(GetDirection(Node, Dir.right), Dir.down)
            };
            allRoomsNodes = new List<int>(treasureRoomNodes);

            // Key room:
            isValid = false;
            while (!isValid)
            {
                Node = r.Next(0, Xsize * Ysize);
                //      Top row                               Bottom row                        Left column                       Right column
                if (!(GetYcoordinate(Node) <= 1 || GetYcoordinate(Node) >= Ysize - 2 || GetXcoordinate(Node) <= 1 || GetXcoordinate(Node) >= Xsize - 2) && !allRoomsNodes.Contains(Node) && !objectPositions.Contains(Node))
                {
                    isValid = true;
                }
            }
            RemoveEdge(GetDirection(GetDirection(Node, Dir.left), Dir.up), GetDirection(Node, Dir.up)); // Top left, top middle
            RemoveEdge(GetDirection(Node, Dir.up), GetDirection(GetDirection(Node, Dir.right), Dir.up)); // Top middle, top right
            RemoveEdge(GetDirection(GetDirection(Node, Dir.left), Dir.up), GetDirection(Node, Dir.left)); // Top left, middle left
            RemoveEdge(GetDirection(Node, Dir.up), Node); // Top middle, middle
            RemoveEdge(GetDirection(GetDirection(Node, Dir.right), Dir.up), GetDirection(Node, Dir.right)); // Top right, middle right
            RemoveEdge(GetDirection(Node, Dir.left), Node); // Middle left, middle
            RemoveEdge(Node, GetDirection(Node, Dir.right)); // Middle, middle right
            RemoveEdge(GetDirection(Node, Dir.left), GetDirection(GetDirection(Node, Dir.left), Dir.down)); // Middle left, bottom left
            RemoveEdge(Node, GetDirection(Node, Dir.down)); // Middle, bottom middle
            RemoveEdge(GetDirection(Node, Dir.right), GetDirection(GetDirection(Node, Dir.right), Dir.down)); // Middle right, bottom right
            RemoveEdge(GetDirection(GetDirection(Node, Dir.left), Dir.down), GetDirection(Node, Dir.down)); // Bottom left, bottom middle
            RemoveEdge(GetDirection(Node, Dir.down), GetDirection(GetDirection(Node, Dir.right), Dir.down)); // Bottom middle, bottom right
            allRoomsNodes.AddRange(new List<int>
            {
                Node,
                GetDirection(GetDirection(Node, Dir.up), Dir.left),
                GetDirection(Node, Dir.up),
                GetDirection(GetDirection(Node, Dir.right), Dir.up),
                GetDirection(Node, Dir.left),
                GetDirection(Node, Dir.right),
                GetDirection(GetDirection(Node, Dir.left), Dir.down),
                GetDirection(Node, Dir.down),
                GetDirection(GetDirection(Node, Dir.right), Dir.down)
            });
            keyPosition = Node;
        }
        public List<int> GetTreasureRoomNodes()
        {
            return treasureRoomNodes;
        }
        public int GetKeyPosition()
        {
            return keyPosition;
        }
        public List<int> GetVisibleNodes(int Node, int FOV)
        {
            List<int> nodes = new List<int>();
            for (int i = 0; i < Xsize * Ysize; i++)
            {
                if (Math.Pow(GetXcoordinate(Node) - GetXcoordinate(i), 2) + Math.Pow(GetYcoordinate(Node) - GetYcoordinate(i), 2) <= Math.Pow(FOV, 2))
                {
                    nodes.Add(i);
                }
            }
            return nodes;
        }
        private void BreakDeadEnds()
        {
            for (int i = 0; i < Xsize * Ysize; i++)
            {
                int node = i;
                if (adjList[node].Count() == 3)
                {
                    foreach (Dir d in directions)
                    {
                        try
                        {
                            if (!adjList[node].Contains(GetDirection(node, d)))
                            {
                                RemoveEdge(node, GetDirection(node, directions[((int)d + 2) % 4]));
                            }
                        }
                        catch (NotInListException) { }
                    }
                }
            }
        }
    }
}