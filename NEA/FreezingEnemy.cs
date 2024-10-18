using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace NEA
{
    internal class FreezingEnemy : Enemy
    {
        public FreezingEnemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos) : base(mazeIn, ran, objectPositions, playerPos)
        {
        }
        public FreezingEnemy() : base()
        {
        }
        public override string GetDescription()
        {
            return "A freezing enemy, that when it hits you will freeze you in place for three turns.";
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
            bool IsNextToPlayer = false;
            if (Position == playerPos)
            {
                IsNextToPlayer = true;
            }
            base.Move(playerPos);
            if (Position == playerPos)
            {
                IsNextToPlayer = true;
            }
            if (IsNextToPlayer)
            {
                Program.SetTimeFrozen(3);
                throw new IsOnPlayerException();
            }
        }
    }
}
