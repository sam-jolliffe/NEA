using System;
using System.Collections.Generic;

namespace NEA
{
    public abstract class Enemy : IVisible
    {
        protected static int numOfEnemies = 0;
        protected static int canMove = -1;
        protected static Random r;
        protected int Position;
        protected int[] PreviousPositions = new int[2];
        protected int Xpos;
        protected int Ypos;
        protected readonly Maze maze;
        public Enemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos)
        {
            numOfEnemies++;
            r = ran;
            maze = mazeIn;
            Spawn(objectPositions, playerPos);
        }
        public Enemy()
        {

        }
        public string GetSprite()
        {
            return "[]";
        }
        public virtual string GetName()
        {
            return "";
        }
        public virtual string GetDescription()
        {
            return "Base enemy";
        }
        public virtual ConsoleColor GetColour()
        {
            return ConsoleColor.Red;
        }
        public int GetPosition()
        {
            return Position;
        }
        public int GetXpos()
        {
            return Xpos;
        }
        public int GetYpos()
        {
            return Ypos;
        }
        public int GetSecondPreviousposition()
        {
            return PreviousPositions[1];
        }
        public void Spawn(List<int> objectPositions, int playerPos)
        {
            bool valid = false;
            while (!valid)
            {
                Position = r.Next(0, maze.GetXsize() * maze.GetYsize() - 1);
                if (!objectPositions.Contains(Position) && Math.Abs(maze.GetXcoordinate(Position) - maze.GetXcoordinate(playerPos)) >= maze.GetXsize() / 6 && Math.Abs(maze.GetYcoordinate(Position) - maze.GetYcoordinate(playerPos)) >= maze.GetXsize() / 6)
                {
                    valid = true;
                }
            }
            PreviousPositions[1] = PreviousPositions[0];
            PreviousPositions[0] = Position;
            Xpos = maze.GetXcoordinate(Position);
            Ypos = maze.GetYcoordinate(Position);
        }
        new public string GetType()
        {
            return "Enemy";
        }
        public virtual void Move(int playerPos)
        {
            //Console.BackgroundColor = ConsoleColor.Black;
            //Console.ForegroundColor = ConsoleColor.White;
            // CanMove is static, so all of the enemies move every other time, but not all at once.
            canMove += r.Next(0, 2);
            if (canMove % 2 == 0 || canMove < 0) return;
            PreviousPositions[1] = PreviousPositions[0];
            PreviousPositions[0] = Position;
            Dir[] directions = { Dir.up, Dir.right, Dir.down, Dir.left };
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
            int count = 15;
            //Console.SetCursorPosition(0, count+=2);
            //Console.Write($"goal node: {playerPos}");
            //Console.SetCursorPosition(0, count +=2);
            //Console.Write($"Node:");
            //Console.SetCursorPosition(0, count+=2);
            //Console.Write($"fdist:");
            count = 16;
            for (int i = 0; i < NumNodes; i++)
            {
                //Console.SetCursorPosition(4 * i + 10, count);
                //Console.Write($"{i}");
                Hdistances[i] = Math.Abs(maze.GetXcoordinate(playerPos) - maze.GetXcoordinate(i)) + Math.Abs(maze.GetYcoordinate(playerPos) - maze.GetYcoordinate(i));
                Gdistances[i] = 1000;
                // Fdistances[i] = 1000;
                Fdistances[i] = 1000 + Hdistances[i];
                previous[i] = -1;
                unvisitedNodes.Add(i);
                //Console.SetCursorPosition(4 * i + 10, count + 1);
                //Console.Write($"{Fdistances[i]}");
            }
            Gdistances[Position] = 0;
            Fdistances[Position] = Hdistances[Position];
            unvisitedNodes.Remove(0);
            while (unvisitedNodes.Count > 0 && unvisitedNodes.Contains(playerPos))
            {
                count++;
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
                //count += 2;
                //for (int i = 0; i < NumNodes; i++)
                //{
                //    Console.SetCursorPosition(4 * i + 10, count);
                //    Console.Write(Fdistances[i]);
                //}
            }

            //Console.ReadKey();
            //Console.Clear();
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
            //Console.BackgroundColor = ConsoleColor.White;
            //Console.ForegroundColor = ConsoleColor.Black;
        }
        public static void ChangeCanMove(int newVal)
        {
            canMove = newVal;
        }
        public static int GetNumOfEnemies()
        {
            return numOfEnemies;
        }
        public void SetPosition(int pos)
        {
            Position = pos;
        }

    }
}
