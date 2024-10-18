using System;
using System.Collections.Generic;

namespace NEA
{
    public class BaseEnemy : Enemy
    {
        public BaseEnemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos) : base(mazeIn, ran, objectPositions, playerPos)
        {
        }

        public BaseEnemy() : base()
        {
        }
        public override string GetDescription()
        {
            return "The base enemy which moves on average every two turns and will kill you if it is on the same square as you.";
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Red;
        }
    }
}
