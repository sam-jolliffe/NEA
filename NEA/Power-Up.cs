using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace NEA
{
    public abstract class Power_Up : IVisible
    {
        private static Random r;
        private int Position;
        private int Xpos;
        private int Ypos;
        protected readonly Maze maze;
        public Power_Up(Maze mazeIn, Random ran, List<int> InObjectPositions)
        {
            r = ran;
            maze = mazeIn;
            Spawn(InObjectPositions);
        }
        public Power_Up(Maze mazeIn, Random ran, int position)
        {
            r = ran;
            maze = mazeIn;
            Spawn(position);
        }
        public string GetSprite()
        {
            return "()";
        }
        public virtual ConsoleColor GetColour()
        {
            return ConsoleColor.Green;
        }
        new public string GetType()
        {
            return "Power-up";
        }
        public virtual string GetName()
        {
            return "Base powerup";
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
        public virtual string GetDescription()
        {
            return "Base Powerup";
        }
        public virtual void Use(int playerPos)
        {

        }
        public void Spawn(List<int> objectPositions)
        {
            bool valid = false;
            while (!valid)
            {
                Position = r.Next(0, maze.GetXsize() * maze.GetYsize() - 1);
                if (!objectPositions.Contains(Position))
                {
                    valid = true;
                }
            }
            Xpos = maze.GetXcoordinate(Position);
            Ypos = maze.GetYcoordinate(Position);
        }
        public void Spawn(int position)
        {
            Position = position;
            Xpos = maze.GetXcoordinate(Position);
            Ypos = maze.GetYcoordinate(Position);
        }
    }
}
