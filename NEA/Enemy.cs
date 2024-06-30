using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    public abstract class Enemy : IVisible
    {
        private int Position;
        private int Xpos;
        private int Ypos;
        public Enemy(Maze maze)
        {
            spawn(maze);
        }
        public int getPosition()
        {
            throw new NotImplementedException();
        }

        public int getXpos()
        {
            throw new NotImplementedException();
        }

        public int getYpos()
        {
            throw new NotImplementedException();
        }

        public bool spawn(Maze maze)
        {
            Position = new Random().Next(0, maze.getXsize() * maze.getYsize() - 1);
            Xpos = maze.getXcoordinate(Position);
            Ypos = maze.getYcoordinate(Position);
            return true;
        }
    }
}
