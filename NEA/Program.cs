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
        static void playGame()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Fullscreen the window, then press any key to comtinue");
            Console.ReadKey();
            maze.createGraph();
            maze.recursiveBacktrackingConstructor(0);
            int pos = 0;
            while (true)
            {
                Console.SetCursorPosition(0, 0);
                maze.displayGraph(pos);
                takeTurn(ref pos);
            }
        }
        static void takeTurn(ref int pos)
        {
            Console.SetCursorPosition(0, 0);
            string move = Console.ReadKey().Key.ToString().ToUpper();
            Console.WriteLine(move);
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
            return;
        }
        static void Main(string[] args)
        {
            playGame();
            Console.ReadKey();
            // Console.WriteLine($"");
        }
    }
}
