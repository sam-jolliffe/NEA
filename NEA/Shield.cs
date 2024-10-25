using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Shield : Power_Up
    {
        public Shield(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
        public Shield(Maze maze, Random ran, int position) : base(maze, ran, position)
        {
        }
        public Shield() : base()
        {
        }
        public override void Use(int playerPos)
        {

        }
        public void Use(int playerPos, ref List<IVisible> objects)
        {
            Enemy Enny = null;
            foreach (IVisible obj in objects)
            {
                if (obj.GetType() == "Enemy" && ((Enemy)obj).GetPosition() == playerPos)
                {
                    Enny = (Enemy)obj;
                }
            }
            objects.Remove(Enny);
            Enny.SetPosition(Enny.GetSecondPreviousposition());
            objects.Add(Enny);
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.DarkMagenta;
        }

        public override string GetDescription()
        {
            return "A shield which automatically activates when any type of enemy attacks you, and knocks them back two places in the maze.";
        }
        public override string GetName()
        {
            return "Shield";
        }
    }
}
