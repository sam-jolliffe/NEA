using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Shield : Power_Up
    {
        public Shield(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
        public Shield(Maze maze, Random ran, int position) : base(maze, ran, position)
        {
        }
        public Shield() : base()
        {
        }
        public override void Use(int playerPos)
        {

        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.DarkMagenta;
        }

        public override string GetDescription()
        {
            return "Automatically activates when any type of enemy attacks you, and knocks them back two places in the maze.";
        }
        public override string GetName()
        {
            return "Shield";
        }
    }
}
