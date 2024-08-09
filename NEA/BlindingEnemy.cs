using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class BlindingEnemy : Enemy
    {
        static int TimeBlinded = 0;
        public BlindingEnemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos) : base(mazeIn, ran, objectPositions, playerPos)
        {
        }

        public override ConsoleColor GetColour()
        {
            return ConsoleColor.DarkMagenta;
        }
        public override string GetName()
        {
            return "Blinder";
        }
        public void SetTimeBlinded(int InTimeBlinded)
        {
            TimeBlinded = InTimeBlinded;
        }
        public override void Move(int playerPos)
        {
            if (TimeBlinded >= 0)
            {
                Program.SetDefaultFOV();
            }
            if (TimeBlinded > 0)
            {
                TimeBlinded--;
            }
            if (canMove < 0) return;
            Dir[] directions = { Dir.up, Dir.right, Dir.down, Dir.left };
            bool IsNextToPlayer = false;
            foreach (Dir direction in directions)
            {

                if (Position == maze.GetDirection(playerPos, direction))
                {
                    IsNextToPlayer = true;
                }
            }
            if (IsNextToPlayer)
            {
                // Shorten FOV
                TimeBlinded = 5;
                Program.SetFOV(3);
            }
            int NumNodes = maze.GetXsize() * maze.GetYsize();
            // H distances will be the manhattan distance between the player position and the given node
            int[] Hdistances = new int[NumNodes];
            // G distances will be the current shortest distance discovered to get to that node
            int[] Gdistances = new int[NumNodes];
            // F distances will be the sum of the P and H distances for that node. 
            // This makes it so that the algorithm will try to search in the vague direction of the end point (the player)
            int[] Fdistances = new int[NumNodes];
            int[] previous = new int[NumNodes];
            List<int> unvisitedNodes = new List<int>();
            for (int i = 0; i < NumNodes; i++)
            {
                Hdistances[i] = Math.Abs(maze.GetXcoordinate(playerPos) - maze.GetXcoordinate(i)) + Math.Abs(maze.GetYcoordinate(playerPos) - maze.GetYcoordinate(i));
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
                        int Temp = maze.GetDirection(node, d);
                        if (unvisitedNodes.Contains(Temp) && !maze.GetAdjList()[node].Contains(Temp))
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
            IsNextToPlayer = false;
            foreach (Dir direction in directions)
            {

                if (Position == maze.GetDirection(playerPos, direction))
                {
                    IsNextToPlayer = true;
                }
            }
            if (IsNextToPlayer)
            {
                // Shorten FOV
                TimeBlinded = 5;
                Program.SetFOV(3);
            }
        }
    }
}
