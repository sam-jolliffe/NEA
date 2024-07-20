using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly Maze maze;
        public Power_Up(Maze mazeIn, Random ran)
        {
            r = ran;
            maze = mazeIn;
            spawn();
        }
        public Power_Up(Maze mazeIn, Random ran, int position)
        {
            r = ran;
            maze = mazeIn;
            spawn(position);
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
        public virtual void use()
        {

        }
        public void spawn()
        {
            Position = r.Next(0, maze.getXsize() * maze.getYsize() - 1);
            Xpos = maze.getXcoordinate(Position);
            Ypos = maze.getYcoordinate(Position);
        }
        public void spawn(int position)
        {
            Position = position;
            Xpos = maze.getXcoordinate(Position);
            Ypos = maze.getYcoordinate(Position);
        }
    }
}
