using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Torch : Power_Up
    {
          public Torch(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
        public Torch(Maze maze, Random ran, int position) : base(maze, ran, position)
        {
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Blue;
        }
        public override string GetDescription()
        {
            return "Grants the user a wider FOV to see more";
        }
        public override string GetName()
        {
            return "Torch";
        }
        public override void Use(int playerPos)
        {
            Program.IncreaseFOV();
        }
    }
}