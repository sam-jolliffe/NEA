using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Stun : Power_Up
    {
        public Stun(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
        public Stun(Maze maze, Random ran, int position) : base(maze, ran, position)
        {
        }
        public Stun() : base()
        {
        }
        public override void Use(int playerPos)
        {
            Enemy.ChangeCanMove(-2 * Enemy.GetNumOfEnemies());
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Green;
        }

        public override string GetDescription()
        {
            return "A Stun which freezes all enemies for an average of two turns.";
        }
        public override string GetName()
        {
            return "Stun";
        }
    }
}
