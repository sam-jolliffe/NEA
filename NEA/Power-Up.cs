﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    public abstract class Power_Up : IVisible
    {
        private static Random r;
        private int Position;
        private int Xpos;
        private int Ypos;
        public Power_Up(Maze maze, Random ran)
        {
            r = ran;
            spawn(maze);
        }
        public string getType()
        {
            return "Power-up";
        }
        public virtual string getName()
        {
            return "Base powerup";
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
        public virtual string getDescription()
        {
            return "Base Powerup";
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