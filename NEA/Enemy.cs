using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;

namespace NEA
{
    public abstract class Enemy : IVisible
    {
        private static Random r;
        private int Position;
        private int Xpos;
        private int Ypos;
        public Enemy(Maze maze, Random ran)
        {
            r = ran;
            spawn(maze);
        }
        public int getPosition()
        {
            return Position;
        }
        public int getXpos()
        {
            return Xpos;
        }
        public int getYpos()
        {
            return Ypos;
        }
        public bool spawn(Maze maze)
        {
            Position = r.Next(0, maze.getXsize() * maze.getYsize() - 1);
            Xpos = maze.getXcoordinate(Position);
            Ypos = maze.getYcoordinate(Position);
            return true;
        }
        public string getType()
        {
            return "Enemy";
        }
        public void move(Maze maze, int playerPos)
        {
            int[] distances = new int[maze.getXsize() * maze.getYsize()];
            int[] previous = new int[maze.getXsize() * maze.getYsize()];
            List<int> unvisitedNodes = new List<int>();
            for (int i = 0; i < maze.getXsize() * maze.getYsize(); i++)
            {
                distances[i] = 1000;
                previous[i] = -1;
                unvisitedNodes.Add(i);
            }
            distances[Position] = 0;
            unvisitedNodes.Remove(0);
            while (unvisitedNodes.Count > 0)
            {
                // Getting node with shortest distance:
                // Has to be in unvisitedNodes, have lowest corresponding distance
                int nodeVal = 1001;
                int node = -1;
                foreach (int i in unvisitedNodes)
                {
                    if (distances[i] < nodeVal)
                    {
                        nodeVal = distances[i];
                        node = i;
                    }
                }
                unvisitedNodes.Remove(node);
                Console.Write($"node: {node} value: {nodeVal} ");
                // Creates a list of all the possible moves the enemy could do
                List<int> possibleMoves = new List<int>();
                Dir[] directions = { Dir.up, Dir.right, Dir.down, Dir.left };
                List<int> possibleDirections = new List<int>();
                foreach (Dir d in directions)
                {
                    try
                    {
                        possibleDirections.Add(maze.getDirection(node, d));
                    }
                    catch (NotInListException) { }
                }
                foreach (int i in possibleDirections)
                {
                    if (unvisitedNodes.Contains(i) && !maze.getAdjList()[node].Contains(i))
                    {
                        possibleMoves.Add(i);
                    }
                }

                // Temp is the same for each one as the distance is the same each time.

                foreach (int i in possibleMoves)
                {
                    int thisPath = distances[node] + 1;
                    if (thisPath < distances[i])
                    {
                        distances[i] = thisPath;
                        // 'i' is the node that it's talking about, the actual value is the node prior to it.
                        previous[i] = node;
                        Console.Write($"previous {i}: {previous[i]} ");
                    }
                }
            }

            // Backtracking until it finds what it's next move should be.
            bool isFound = false;
            int temp = playerPos;
            while (!isFound)
            {
                if (previous[temp] == Position)
                {
                    Position = temp;
                    isFound = true;
                }
                temp = previous[temp];
            }
            

            /*List<int> randomDirections = maze.randomize(new List<int> { 0, 1, 2, 3 });
            List<int> possibleDirections = new List<int>();
            bool isValidDirection = false;
            foreach (int i in randomDirections)
            {
                try
                {
                    int tempPos = maze.getDirection(Position, directions[i]);
                    if (!maze.getEdges(tempPos).Contains(Position))
                    {
                        possibleDirections.Add(tempPos);
                    }
                    isValidDirection = true;
                }
                catch (NotInListException) { }
            }
            if (isValidDirection)
            {
                Position = possibleDirections[0];
            }*/
        }
    }
}
