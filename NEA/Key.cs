using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Key : IVisible
    {
        private int Position;
        private int Xpos;
        private int Ypos;
        private readonly Maze maze;
        public Key(int position, Maze Maze)
        {
            Position = position;
            maze = Maze;
        }

        public int getPosition()
        {
            return Position;
        }

        public string getType()
        {
            return "Key";
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
            Xpos = maze.getXcoordinate(Position);
            Ypos = maze.getYcoordinate(Position);
        }
    }
}
