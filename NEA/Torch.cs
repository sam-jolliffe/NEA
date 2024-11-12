using System;
using System.Collections.Generic;

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
        public Torch() : base()
        {
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Blue;
        }
        public override string GetDescription()
        {
            return "A Torch which grants the you a wider FOV to see more";
        }
        public override string GetName()
        {
            return "Torch";
        }
        public override void Use(int playerPos)
        {
            Program.IncreaseFOV();
            Program.SetDefaultFOV();
        }
    }
}