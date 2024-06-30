using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    public class Player : IVisible
    {
        private static Random r = new Random();
        private int Position;
        private int Xpos;
        private int Ypos;
        public int getPosition()
        {
            return Position;
        }
        public void setPosition(int newPos)
        {
            Position = newPos;
        }

        public string getType()
        {
            return "Player";
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
    }
}
