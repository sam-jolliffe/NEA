using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    public class BaseEnemy : Enemy
    {
        public BaseEnemy(Maze maze, Random ran) : base(maze, ran)
        {
        }
    }
}
