using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Key : IVisible
    {
        private readonly int Position;
        private int Xpos;
        private int Ypos;
        private readonly Maze maze;
        public Key(int position, Maze Maze)
        {
            Position = position;
            maze = Maze;
        }
        public string GetSprite()
        {
            return "██";
        }
        public ConsoleColor GetColour()
        {
            return ConsoleColor.Magenta;
        }
        public int GetPosition()
        {
            return Position;
        }

        new public string GetType()
        {
            return "Key";
        }

        public int GetXpos()
        {
            return Xpos;
        }

        public int GetYpos()
        {
            return Ypos;
        }

        public void Spawn()
        {
            Xpos = maze.GetXcoordinate(Position);
            Ypos = maze.GetYcoordinate(Position);
        }
    }
}
