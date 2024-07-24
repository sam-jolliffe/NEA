using System;
using System.Collections.Generic;

namespace NEA
{
    public class BaseEnemy : Enemy
    {
        public BaseEnemy(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
    }
}
