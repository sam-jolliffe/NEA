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
        static Random random = new Random();
        readonly static int size = 25;
        readonly static Maze maze = new Maze(size, random);
        static Player player = new Player(maze, random);
        static void playGame(bool showGeneration)
        {
            List<IVisible> objects = new List<IVisible>();
            // Adding enemies
            for (int i = 0; i < 1; i++)
            {
                objects.Add(new BaseEnemy(maze, random));
            }
            // Adding power-ups
            for (int i = 0; i < 5; i++)
            {
                objects.Add(new Stun(maze, random));
            }
            Console.WriteLine(objects.Count());
            // Adding the player
            objects.Add(player);
            maze.createGraph();
            maze.generateMaze(player.getPosition(), showGeneration, objects);
            int oldPos;
            bool hasWon = false;
            bool hasLost = false;
            // Keeps taking a move and re-displaying the board until the user reaches the end
            Console.SetCursorPosition(0, 0);
            maze.displayGraph(objects);
            while (!hasWon && !hasLost)
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
                    maze.displayGraph(objects);
                    if (player.getPosition() == maze.getEndPoint())
                    {
                        hasWon = true;
                    }
                    // Making a list of the positions of all enemies and power-ups
                    List<int> enemyPositions = new List<int>();
                    List<int> powerupPositions = new List<int>();
                    List<Enemy> enemies = new List<Enemy>();
                    List<Power_Up> powerUps = new List<Power_Up>();
                    foreach (IVisible obj in objects)
                    {
                        if (obj.getType() == "Enemy")
                        {
                            enemyPositions.Add(obj.getPosition());
                            enemies.Add((Enemy)obj);
                        }
                        else if (obj.getType() == "Power-up")
                        {
                            powerupPositions.Add(obj.getPosition());
                            powerUps.Add((Power_Up)obj);
                        }
                    }
                    if (enemyPositions.Contains(player.getPosition()))
                    {
                        hasLost = true;
                    }
                    if (powerupPositions.Contains(player.getPosition()))
                    {
                        foreach (Power_Up powerUp in powerUps)
                        {
                            if (powerUp.getPosition() == player.getPosition())
                            {
                                player.addToInventory(powerUp);
                                objects.Remove(powerUp);
                            }
                        }
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
            if (hasWon)
            {
                Console.WriteLine(@"
                                                                                                                                                
                                                                                                                                                
YYYYYYY       YYYYYYY                                          WWWWWWWW                           WWWWWWWW                                        !!! 
Y:::::Y       Y:::::Y                                          W::::::W                           W::::::W                                       !!:!!
Y:::::Y       Y:::::Y                                          W::::::W                           W::::::W                                       !:::!
Y::::::Y     Y::::::Y                                          W::::::W                           W::::::W                                       !:::!
YYY:::::Y   Y:::::YYY   ooooooooooo     uuuuuu    uuuuuu        W:::::W           WWWWW           W:::::W    ooooooooooo     nnnn  nnnnnnnn      !:::!
   Y:::::Y Y:::::Y    oo:::::::::::oo   u::::u    u::::u         W:::::W         W:::::W         W:::::W   oo:::::::::::oo   n:::nn::::::::nn    !:::!
    Y:::::Y:::::Y    o:::::::::::::::o  u::::u    u::::u          W:::::W       W:::::::W       W:::::W   o:::::::::::::::o  n::::::::::::::nn   !:::!
     Y:::::::::Y     o:::::ooooo:::::o  u::::u    u::::u           W:::::W     W:::::::::W     W:::::W    o:::::ooooo:::::o  nn:::::::::::::::n  !:::!
      Y:::::::Y      o::::o     o::::o  u::::u    u::::u            W:::::W   W:::::W:::::W   W:::::W     o::::o     o::::o    n:::::nnnn:::::n  !:::!
       Y:::::Y       o::::o     o::::o  u::::u    u::::u             W:::::W W:::::W W:::::W W:::::W      o::::o     o::::o    n::::n    n::::n  !:::!
       Y:::::Y       o::::o     o::::o  u::::u    u::::u              W:::::W:::::W   W:::::W:::::W       o::::o     o::::o    n::::n    n::::n  !!:!!
       Y:::::Y       o::::o     o::::o  u:::::uuuu:::::u               W:::::::::W     W:::::::::W        o::::o     o::::o    n::::n    n::::n   !!! 
       Y:::::Y       o:::::ooooo:::::o  u:::::::::::::::uu              W:::::::W       W:::::::W         o:::::ooooo:::::o    n::::n    n::::n     
    YYYY:::::YYYY    o:::::::::::::::o   u:::::::::::::::u               W:::::W         W:::::W          o:::::::::::::::o    n::::n    n::::n   !!! 
    Y:::::::::::Y     oo:::::::::::oo     uu::::::::uu:::u                W:::W           W:::W            oo:::::::::::oo     n::::n    n::::n  !!:!!
    YYYYYYYYYYYYY       ooooooooooo         uuuuuuuu  uuuu                 WWW             WWW               ooooooooooo       nnnnnn    nnnnnn   !!! 
                                                                                                                                                
                                                                                                                                                
                                                                                                                                                
                                                                                                                                                
                                                                                                                                                
                                                                                                                                                
                                                                                                                                                
");
            }
            else if (hasLost)
            {
                Console.WriteLine(@"
                                                                                                                                                   
                                                                                                                                                     
YYYYYYY       YYYYYYY                                          LLLLLLLLLLL                                                            tttt             !!! 
Y:::::Y       Y:::::Y                                          L:::::::::L                                                         ttt:::t            !!:!!
Y:::::Y       Y:::::Y                                          L:::::::::L                                                         t:::::t            !:::!
Y::::::Y     Y::::::Y                                          LL:::::::LL                                                         t:::::t            !:::!
YYY:::::Y   Y:::::YYY   ooooooooooo     uuuuuu    uuuuuu         L:::::L                    ooooooooooo         ssssssssss   ttttttt:::::ttttttt      !:::!
   Y:::::Y Y:::::Y    oo:::::::::::oo   u::::u    u::::u         L:::::L                  oo:::::::::::oo     ss::::::::::s  t:::::::::::::::::t      !:::!
    Y:::::Y:::::Y    o:::::::::::::::o  u::::u    u::::u         L:::::L                 o:::::::::::::::o  ss:::::::::::::s t:::::::::::::::::t      !:::!
     Y:::::::::Y     o:::::ooooo:::::o  u::::u    u::::u         L:::::L                 o:::::ooooo:::::o  s::::::ssss:::::stttttt:::::::tttttt      !:::!
      Y:::::::Y      o::::o     o::::o  u::::u    u::::u         L:::::L                 o::::o     o::::o   s:::::s  ssssss       t:::::t            !:::!
       Y:::::Y       o::::o     o::::o  u::::u    u::::u         L:::::L                 o::::o     o::::o     s::::::s            t:::::t            !:::!
       Y:::::Y       o::::o     o::::o  u::::u    u::::u         L:::::L                 o::::o     o::::o        s::::::s         t:::::t            !!:!!
       Y:::::Y       o::::o     o::::o  u:::::uuuu:::::u         L:::::L         LLLLLL  o::::o     o::::o  ssssss   s:::::s       t:::::t    tttttt   !!! 
       Y:::::Y       o:::::ooooo:::::o  u:::::::::::::::uu     LL:::::::LLLLLLLLL:::::L  o:::::ooooo:::::o  s:::::ssss::::::s      t::::::tttt:::::t     
    YYYY:::::YYYY    o:::::::::::::::o   u:::::::::::::::u     L::::::::::::::::::::::L  o:::::::::::::::o  s::::::::::::::s       tt::::::::::::::t   !!! 
    Y:::::::::::Y     oo:::::::::::oo     uu::::::::uu:::u     L::::::::::::::::::::::L   oo:::::::::::oo    s:::::::::::ss          tt:::::::::::tt  !!:!!
    YYYYYYYYYYYYY       ooooooooooo         uuuuuuuu  uuuu     LLLLLLLLLLLLLLLLLLLLLLLL     ooooooooooo       sssssssssss              ttttttttttt     !!! 
                                                                                                                                                   
                                                                                                                                                   
                                                                                                                                                   
                                    
");
            }
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
            else if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow)
            {
                pos = maze.getLeft(pos);
            }
            else if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow)
            {
                pos = maze.getDown(pos);
            }
            else if (key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow)
            {
                pos = maze.getRight(pos);
            }
            else if (key.Key == ConsoleKey.E)
            {
                player.showInventory();
                pos = -1;
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
