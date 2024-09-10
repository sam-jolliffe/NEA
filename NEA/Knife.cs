using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Knife : Power_Up
    {
        public Knife(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
        public Knife(Maze maze, Random ran, int position) : base(maze, ran, position)
        {
        }
        public override void Use(int playerPos)
        {
            Dir[] Directions = {Dir.left, Dir.right, Dir.up, Dir.down};
            List<IVisible> CopyOfObjects = Program.GetObjects();
            List<Enemy> Enemies = new List<Enemy>();
            List<Enemy> AdjacentEnemies = new List<Enemy>();
            List<Dir> AdjacentEnemyDirections = new List<Dir>();
            foreach (IVisible obj in CopyOfObjects)
            {
                if (obj.GetType() == "Enemy")
                {
                    Enemies.Add((Enemy)obj);
                }
            }
            foreach (Enemy enemy in Enemies)
            {
                foreach (Dir d in Directions)
                {
                    if (maze.GetDirection(enemy.GetPosition(), d) == playerPos && !maze.GetAdjList()[playerPos].Contains(enemy.GetPosition()))
                    {
                        AdjacentEnemies.Add(enemy);
                        AdjacentEnemyDirections.Add(d);
                    }
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Program.DisplayMaze();
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            if (AdjacentEnemies.Count == 0)
            {
                Console.WriteLine("There are no enemies adjacent to you");
            }
            else
            {
                Console.WriteLine("Use WASD or arrow keys to choose an enemy around you to kill");
            }
            bool isValid = false;
            while (!isValid)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                Dir direction = Dir.up;
                if ((key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow) && AdjacentEnemyDirections.Contains(Dir.up))
                {
                    direction = Dir.up;
                    isValid = true;
                }
                else if ((key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow) && AdjacentEnemyDirections.Contains(Dir.left))
                {
                    direction = Dir.left;
                    isValid = true;
                }
                else if ((key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow) && AdjacentEnemyDirections.Contains(Dir.down))
                {
                    direction = Dir.down;
                    isValid = true;
                }
                else if ((key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow) && AdjacentEnemyDirections.Contains(Dir.right))
                {
                    direction = Dir.right;
                    isValid = true;
                }
                if (isValid)
                {
                    // Removing the enemy
                    foreach (Enemy enemy in AdjacentEnemies)
                    {
                        if (enemy.GetPosition() == maze.GetDirection(playerPos, direction))
                        {

                        }
                    }
                }
            }
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.WriteLine("                                                                                                               ");
            Console.SetCursorPosition(0, 0);
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Black;
        }

        public override string GetDescription()
        {
            return "Can be used to kill an enemy which is directly next to the user.";
        }
        public override string GetName()
        {
            return "Knife";
        }
    }
}
