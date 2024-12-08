using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class BlindingEnemy : Enemy
    {
        public BlindingEnemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos) : base(mazeIn, ran, objectPositions, playerPos)
        {
        }
        public BlindingEnemy() : base()
        {
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.DarkMagenta;
        }
        public override string GetName()
        {
            return "Blinder";
        }
        public override string GetDescription()
        {
            return "A blinding enemy that, when within a space of you will blind you for five turns.";
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
                throw new IsOnPlayerException();
            }
        }
    }
}
