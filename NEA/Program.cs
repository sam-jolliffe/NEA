using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NEA
{
    internal class Program
    {
        static Random random = new Random();
        readonly static int size = 5;
        readonly static Maze maze = new Maze(size, random);
        static Player player = new Player(maze, random);
        static void playGame()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            List<IVisible> objects = new List<IVisible>();
            // Adding enemies
            for (int i = 0; i < 2; i++)
            {
                objects.Add(new BaseEnemy(maze, random));
            }
            // Adding power-ups
            for (int i = 0; i < 10; i++)
            {
                objects.Add(new Stun(maze, random));
            }
            Console.WriteLine(objects.Count());
            // Adding the player
            objects.Add(player);
            maze.createGraph();
            maze.generateMaze(player.getPosition(), objects);
            int oldPos;
            bool hasWon = false;
            bool hasLost = false;
            // Keeps taking a move and re-displaying the board until the user reaches the end
            Console.SetCursorPosition(0, 0);
            maze.displayGraph(objects);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.move(maze);
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
            stopwatch.Stop();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            if (hasWon)
            {
                AddToLeaderboard((int)stopwatch.Elapsed.TotalSeconds);
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
            Console.WriteLine("\n\n\n\n Press any key to play again");
            Console.ReadKey(true);
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
        static int MainMenu()
        {
            int yPos = 1;
            Console.Clear();
            Console.WriteLine(@"Would you like to: 
  Start Game
  View Leaderboard
  How To Play?
  Exit");
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
                        return 1;
                    }
                    else if (yPos == 2)
                    {
                        return 2;
                    }
                    else if (yPos == 3)
                    {
                        return 3;
                    }
                    else if (yPos == 4)
                    {
                        return 4;
                    }
                }
                else if ((key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow) && yPos > 1)
                {
                    yPos--;
                }
                else if ((key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow) && yPos < 4)
                {
                    yPos++;
                }
            }
        }
        static void DisplayLeaderBoard()
        {
            const string fileName = "Leaderboard.txt";
            List<string> names = new List<string>();
            List<int> times = new List<int>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string[] temps = new string[2];
                while (!sr.EndOfStream)
                {
                    temps = sr.ReadLine().Split(' ');
                    names.Add(temps[0]);
                    times.Add(int.Parse(temps[1]));
                }
            }
            for (int i = 0; i < names.Count(); i++)
            {
                Console.WriteLine($"{names[i]}: {formatTime(times[i])}");
            }
            Console.ReadKey();
        }
        static void AddToLeaderboard(int seconds)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            string name = "";
            bool validName = false;
            while (!validName)
            {
                Console.WriteLine("Enter your name:");
                name = Console.ReadLine();
                if (name.Length > 0) validName = true;
            }
            const string fileName = "Leaderboard.txt";
            List<string> lines = new List<string>();
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                while (!streamReader.EndOfStream)
                {
                    lines.Add(streamReader.ReadLine());
                }
            }
            lines = InsertionSort(name, seconds, lines);
            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                foreach (string line in lines)
                {
                    streamWriter.WriteLine(line);
                }
            }
        }
        static void HowToPlay()
        {
            Console.WriteLine("Just play the game dumbass");
            Console.ReadKey();
        }
        public static string formatTime(int seconds)
        {
            int[] times = new int[3];
            times[0] = seconds / 3600; // Hours
            times[1] = (seconds - (times[0] * 3600)) / 60; // Minutes
            times[2] = seconds - (times[0] * 3600) - (times[1] * 60); // Seconds
            string[] isPlural = new string[3];
            bool[] exists = new bool[5];
            string[] names = { "hour", "minute", "second" };
            for (int i = 0; i < 3; i++)
            {
                isPlural[i] = times[i] != 1 ? "s" : "";
                exists[i] = times[i] == 0 ? false : true; 
            }
            int lastIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                if (exists[i]) lastIndex = i;
            }
            int secondLastIndex = 0;
            for (int i = 0; i < lastIndex; i++)
            {
                if (exists[i]) secondLastIndex = i;
            }
            string returnString = "";
            for (int i = 0; i < exists.Length; i++)
            {
                if (exists[i])
                {
                    if (i == lastIndex) returnString += $"{times[i]} {names[i]}{isPlural[i]}";
                    else if (i == secondLastIndex) returnString += $"{times[i]} {names[i]}{isPlural[i]} and ";
                    else returnString += $"{times[i]} {names[i]}{isPlural[i]}, ";
                }
            }
            return returnString;
        }
        static List<string> InsertionSort(string name, int time, List<string> list)
        {
            List<int> times = new List<int>();
            List<string> returnList = new List<string>();
            // Isolates the integers from the list
            foreach (string s in list)
            {
                string[] tempArr = s.Split(' ');
                times.Add(int.Parse(tempArr[1]));
            }
            // Inserts value
            bool done = false;
            int index = 0;
            while (!done)
            {
                if (time <= times[index])
                {
                    done = true;
                    returnList.Add($"{name} {time}");
                }
                returnList.Add(list[index]);
                index++;
            }
            while (index < list.Count)
            {
                returnList.Add(list[index]);
                index++;
            }
            return returnList;
        }
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.CursorVisible = false;
                int choice = MainMenu();
                Console.Clear();
                if (choice == 1) playGame();
                else if (choice == 2) DisplayLeaderBoard();
                else if (choice == 3) HowToPlay();
                else exit = true;
            }
        }
    }
}
