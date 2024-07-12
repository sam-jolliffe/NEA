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
        public void move(Maze maze)
        {
            Dir[] directions = { Dir.up, Dir.right, Dir.down, Dir.left };
            List<int> randomDirections = maze.randomize(new List<int> { 0, 1, 2, 3 });
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
            }
        }
    }
}
