using System;
using System.Collections.Generic;

namespace NEA
{
    public class BaseEnemy : Enemy
    {
        public BaseEnemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos) : base(mazeIn, ran, objectPositions, playerPos)
        {
        }


        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Red;
        }
    }
}
