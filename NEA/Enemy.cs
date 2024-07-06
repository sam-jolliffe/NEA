using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            bool moved = false;
            while (!moved)
            {
                int direction = r.Next(0, 4);
                int tempPos = Position;
                if (direction == 0) tempPos = maze.getUp(tempPos);
                else if (direction == 1) tempPos = maze.getRight(tempPos);
                else if (direction == 2) tempPos = maze.getDown(tempPos);
                else tempPos = maze.getLeft(tempPos);
                if (tempPos != -1 && !maze.getEdges(tempPos).Contains(Position))
                {
                    Position = tempPos;
                    moved = true;
                }
            }
        }
    }
}
