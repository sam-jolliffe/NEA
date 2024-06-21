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
        public static int size = 25;
        public static Maze maze = new Maze(size);
        public static Random r = new Random();
        static void playGame()
        {
            Console.WriteLine("Fullscreen the window, then press any key to continue");
            Console.ReadKey();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            maze.createGraph();
            maze.generateMaze(0);
            int newPos = 0;
            int oldPos;
            bool hasWon = false;
            int endPoint = getRandom(maze.getXsize() * maze.getYsize());
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
            string move = Console.ReadKey().Key.ToString().ToUpper();
            if (move == "W")
            {
                pos = maze.getUp(pos);
            }
            if (move == "A")
            {
                pos = maze.getLeft(pos);
            }
            if (move == "S")
            {
                pos = maze.getDown(pos);
            }
            if (move == "D")
            {
                pos = maze.getRight(pos);
            }
            return pos;
        }
        static int getRandom(int maxNum)
        {
            return r.Next(maxNum + 1);
        }
        static void Main(string[] args)
        {
            playGame();
            Console.ReadKey();
        }
    }
}
