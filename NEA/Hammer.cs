using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NEA
{
    internal class Hammer : Power_Up
    {
        public Hammer(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
        public Hammer(Maze maze, Random ran, int position) : base(maze, ran, position)
        {
        }
        public override void Use(int playerPos)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Program.DisplayMaze();
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Use WASD or arrow keys to choose an adjacent wall around you to break.");
            bool isValid = false;
            while (!isValid)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                Dir direction = Dir.up;
                if (key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow)
                {
                    direction = Dir.up;
                    isValid = true;
                }
                else if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow)
                {
                    direction = Dir.left;
                    isValid = true;
                }
                else if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow)
                {
                    direction = Dir.down;
                    isValid = true;
                }
                else if (key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow)
                {
                    direction= Dir.right;
                    isValid = true;
                }
                if (isValid)
                {
                    maze.RemoveEdge(playerPos, maze.GetDirection(playerPos, direction));
                }
            }
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.WriteLine("                                                                                                               ");
            Console.SetCursorPosition(0, 0);
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Gray;
        }

        public override string GetDescription()
        {
            return "Can be used to break a wall that is directly adjacent to the user.";
        }
        public override string GetName()
        {
            return "Hammer";
        }
    }
}
