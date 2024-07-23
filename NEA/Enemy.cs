using System;
using System.Collections.Generic;

namespace NEA
{
    public abstract class Enemy : IVisible
    {
        private static Random r;
        private int Position;
        private int Xpos;
        private int Ypos;
        private readonly Maze Maze;
        public Enemy(Maze maze, Random ran)
        {
            r = ran;
            Maze = maze;
            spawn();
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
        public void spawn()
        {
            Position = r.Next(0, Maze.getXsize() * Maze.getYsize() - 1);
            Xpos = Maze.getXcoordinate(Position);
            Ypos = Maze.getYcoordinate(Position);
        }
        public string getType()
        {
            return "Enemy";
        }
        public void move(Maze maze, int playerPos)
        {
            Dir[] directions = { Dir.up, Dir.right, Dir.down, Dir.left };
            // H distances will be the manhattan distance between the player position and the given node
            int[] Hdistances = new int[maze.getXsize() * maze.getYsize()];
            // G distances will be the current shortest path discovered to get to that node
            int[] Gdistances = new int[maze.getXsize() * maze.getYsize()];
            // F distances will be the sum of the P and H distances for that node. 
            // This makes it so that the algorithm will try to search in the vague direction of the end point (the player)
            int[] Fdistances = new int[maze.getXsize() * maze.getYsize()];
            int[] previous = new int[maze.getXsize() * maze.getYsize()];
            List<int> unvisitedNodes = new List<int>();
            for (int i = 0; i < maze.getXsize() * maze.getYsize(); i++)
            {
                Hdistances[i] = Math.Abs(maze.getXcoordinate(playerPos) - maze.getXcoordinate(i)) + Math.Abs(maze.getYcoordinate(playerPos) - maze.getYcoordinate(i));
                Gdistances[i] = 1000;
                Fdistances[i] = 1000;
                previous[i] = -1;
                unvisitedNodes.Add(i);
            }
            Gdistances[Position] = 0;
            Fdistances[Position] = Hdistances[Position];
            unvisitedNodes.Remove(0);
            while (unvisitedNodes.Count > 0 && unvisitedNodes.Contains(playerPos))
            {
                // Getting node with shortest distance:
                // Has to be in unvisitedNodes, have lowest corresponding distance
                int nodeVal = 1001;
                int node = -1;
                foreach (int i in unvisitedNodes)
                {
                    if (Fdistances[i] < nodeVal)
                    {
                        nodeVal = Fdistances[i];
                        node = i;
                    }
                }
                unvisitedNodes.Remove(node);
                // Creates a list of all the possible moves the enemy could do
                List<int> possibleMoves = new List<int>();
                foreach (Dir d in directions)
                {
                    try
                    {
                        int Temp = maze.getDirection(node, d);
                        if (unvisitedNodes.Contains(Temp) && !maze.getAdjList()[node].Contains(Temp))
                        {
                            possibleMoves.Add(Temp);
                        }
                    }
                    catch (NotInListException) { }
                }

                // thisPath is the same for each one as the distance is the same each time.

                int thisPath = Gdistances[node] + 1;
                foreach (int i in possibleMoves)
                {
                    if (thisPath < Gdistances[i])
                    {
                        Gdistances[i] = thisPath;
                        Fdistances[i] = Hdistances[i] + thisPath;
                        // 'i' is the node that it's talking about, the actual value is the node prior to it.
                        previous[i] = node;
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
        }
    }
}
