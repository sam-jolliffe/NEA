using NEA_testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Program
    {
        private readonly static int size = 25;
        private readonly static Maze maze = new Maze(size);
        static void playGame()
        {
            maze.createGraph();
            maze.generateMaze(0);
            int newPos = 0;
            int oldPos;
            bool hasWon = false;
            // Makes sure that the end point isn't too close to the user
            bool validEndPoint = false;
            int endPoint = 0;
            while (!validEndPoint)
            {
                endPoint = maze.getRandom(maze.getXsize() * maze.getYsize());
                if (maze.getXcoordinate(endPoint) + maze.getYcoordinate(endPoint) > (maze.getXsize() + maze.getYsize()) / 2)
                {
                    validEndPoint = true;
                }
            }
            // Keeps taking a move and re-displaying the board until the user reaches the end
            Console.SetCursorPosition(0, 0);
            maze.displayGraph(newPos, endPoint);
            while (!hasWon)
            {
                oldPos = newPos;
                newPos = takeTurn(oldPos);
                if (newPos == endPoint)
                {
                    hasWon = true;
                }
                else if (newPos == -1)
                {
                    newPos = oldPos;
                }
                else if (!maze.getAdjList()[oldPos].Contains(newPos))
                {
                    Console.SetCursorPosition(0, 0);
                    maze.displayGraph(newPos, endPoint);
                }
                else
                {
                    newPos = oldPos;
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine(@"
__   __                                        _                                             
\ \ / /                                       | |                                            
 \ V /   ___   _   _  __      __  ___   _ __  | |                                            
  \ /   / _ \ | | | | \ \ /\ / / / _ \ | '_ \ | |                                            
  | |  | (_) || |_| |  \ V  V / | (_) || | | ||_|                                            
  \_/   \___/  \__,_|   \_/\_/   \___/ |_| |_|(_)                                            
                                                                                             
                                                                                             
 _____                                    _           _         _    _                     _ 
/  __ \                                  | |         | |       | |  (_)                   | |
| /  \/  ___   _ __    __ _  _ __   __ _ | |_  _   _ | |  __ _ | |_  _   ___   _ __   ___ | |
| |     / _ \ | '_ \  / _` || '__| / _` || __|| | | || | / _` || __|| | / _ \ | '_ \ / __|| |
| \__/\| (_) || | | || (_| || |   | (_| || |_ | |_| || || (_| || |_ | || (_) || | | |\__ \|_|
 \____/ \___/ |_| |_| \__, ||_|    \__,_| \__| \__,_||_| \__,_| \__||_| \___/ |_| |_||___/(_)
                       __/ |                                                                 
                      |___/                                                                  
");
        }
        static int takeTurn(int pos)
        {
            Console.SetCursorPosition(0, 0);
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow)
            {
                pos = maze.getUp(pos);
            }
            if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow)
            {
                pos = maze.getLeft(pos);
            }
            if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow)
            {
                pos = maze.getDown(pos);
            }
            if (key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow)
            {
                pos = maze.getRight(pos);
            }
            return pos;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Fullscreen the window, then press any key to continue");
            Console.ReadKey(true);
            while (true)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                playGame();
                Console.WriteLine("\n\n\n\n Press any key to play again");
                Console.ReadKey(true);
            }
        }
    }
}
