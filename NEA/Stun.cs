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
        public override void Use()
        {
            Enemy.ChangeCanMove(-2 * Enemy.GetNumOfEnemies());
        }
        public override string GetDescription()
        {
            return "A stun ability which will temporarily make all enemies pause where they are for one move.";
        }
        public override string GetName()
        {
            return "Stun";
        }
    }
}
