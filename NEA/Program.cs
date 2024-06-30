using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NEA
{
    internal class Program
    {
        readonly static int size = 25;
        readonly static Maze maze = new Maze(size);
        static void playGame(bool showGeneration)
        {
            List<IVisible> objects = new List<IVisible>();
            // Adding enemies
            for (int i = 0; i < 5; i++)
            {
                objects.Add(new BaseEnemy(maze));
            }
            // Adding power-ups
            for (int i = 0; i < 5; i++)
            {
                objects.Add(new Stun(maze));
            }
            Console.WriteLine(objects.Count());
            // Adding the player
            Player player = new Player();
            objects.Add(player);
            maze.createGraph();
            int newPos = player.getPosition();
            maze.generateMaze(newPos, showGeneration, objects);
            int oldPos;
            bool hasWon = false;
            // Keeps taking a move and re-displaying the board until the user reaches the end
            Console.SetCursorPosition(0, 0);
            maze.displayGraph(newPos, objects);
            while (!hasWon)
            {
                oldPos = player.getPosition();
                player.setPosition(takeTurn(oldPos));
                if (player.getPosition() == -1)
                {
                    player.setPosition(oldPos);
                }
                else if (!maze.getAdjList()[oldPos].Contains(player.getPosition()))
                {
                    Console.SetCursorPosition(0, 0);
                    maze.displayGraph(player.getPosition(), objects);
                    if (player.getPosition() == maze.getEndPoint())
                    {
                        hasWon = true;
                    } 
                }
                else
                {
                    player.setPosition(oldPos);
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
            Console.CursorVisible = false;
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
        static bool getChoice()
        {
            int yPos = 1;
            Console.Clear();
            Console.WriteLine(@"Would you like to see the maze generate?
  Yes
  No");
            ConsoleKeyInfo key;
            while (true)
            {
                Console.CursorLeft = 0;
                Console.Write(" ");
                Console.SetCursorPosition(0, yPos);
                Console.Write(">");
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (yPos == 1)
                    {
                        return true;
                    }
                    else if (yPos == 2)
                    {
                        return false;
                    }
                }
                else if ((key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow) && yPos > 1)
                {
                    yPos--;
                }
                else if ((key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow) && yPos < 2)
                {
                    yPos++;
                }
            }
        }
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            bool showAlgorithm = getChoice();
            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                playGame(showAlgorithm);
                Console.WriteLine("\n\n\n\n Press any key to play again");
                Console.ReadKey(true);
            }
        }
    }
}
