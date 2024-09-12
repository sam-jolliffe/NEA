using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class FreezingEnemy : Enemy
    {
        public FreezingEnemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos) : base(mazeIn, ran, objectPositions, playerPos)
        {
        }
        public override string GetName()
        {
            return "Freezer";
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Cyan;
        }
        public override void Move(int playerPos)
        {
            Dir[] directions = { Dir.up, Dir.right, Dir.down, Dir.left };
            bool IsNextToPlayer = false;
            foreach (Dir direction in directions)
            {
                if (Position == maze.GetDirection(playerPos, direction) && maze.GetAdjList()[Position].Contains(playerPos))
                {
                    IsNextToPlayer = true;
                }
            }
            if (IsNextToPlayer)
            {
                Program.SetTimeFrozen(3);
            }
            base.Move(playerPos);
        }
    }
}
