using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NEA
{
    public enum Dir { up, right, down, left }
    internal class Program
    {
        static readonly Random random = new Random();
        static Maze maze;
        static Player player;
        static int size;
        static int DefaultFOV;
        static int FOV;
        static int BaseEnemies;
        static int Ghosts;
        static int Blinders;
        static int Stuns;
        static int Hammers;
        static List<IVisible> objects;
        static void PlayGame()
        {
            int difficulty = SetDifficulty();
            int TotalEnemies = BaseEnemies + Ghosts;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            objects = new List<IVisible>();
            List<int> objectPositions = new List<int>(player.GetPosition());
            // Adding enemies
            for (int i = 0; i < BaseEnemies; i++)
            {
                objects.Add(new BaseEnemy(maze, random, objectPositions, player.GetPosition()));
                objectPositions.Add(objects[i].GetPosition());
            }
            for (int i = 0; i < Ghosts; i++)
            {
                objects.Add(new GhostEnemy(maze, random, objectPositions, player.GetPosition()));
                objectPositions.Add(objects[i].GetPosition());
            }
            for (int i = 0; i < Blinders; i++)
            {
                objects.Add(new BlindingEnemy(maze, random, objectPositions, player.GetPosition()));
                objectPositions.Add(objects[i].GetPosition());
            }
            // Adding power-ups
            for (int i = 0; i < Stuns; i++)
            {
                Stun tempStun = new Stun(maze, random, objectPositions);
                objects.Add(tempStun);
                objectPositions.Add(tempStun.GetPosition());
            }
            for (int i = 0; i < Hammers; i++)
            {
                Hammer tempHammer = new Hammer(maze, random, objectPositions);
                objects.Add(tempHammer);
                objectPositions.Add(tempHammer.GetPosition());
            }
            maze.CreateGraph();
            maze.GenerateMaze(player.GetPosition(), objects);
            List<int> treasureRoomNodes = maze.GetTreasureRoomNodes();
            foreach (int roomNode in treasureRoomNodes)
            {
                objects.Add(new Stun(maze, random, roomNode));
            }
            objects.Add(new Key(maze.GetKeyPosition(), maze));
            objects.Add(player);
            int oldPos;
            bool hasWon = false;
            bool hasLost = false;
            // Keeps taking a move and re-displaying the board until the user reaches the end
            Console.SetCursorPosition(0, 0);
            maze.DisplayGraph(objects, FOV);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!hasWon && !hasLost)
            {
                if (BlindingEnemy.GetBlindersCount() <= 0)
                {
                    FOV = DefaultFOV;
                }
                oldPos = player.GetPosition();
                bool invalidTurn = false;
                Console.SetCursorPosition(0, 0);
                maze.DisplayGraph(objects, FOV);
                try
                {
                    player.SetPosition(TakeTurn(oldPos));
                }
                catch (NotInListException)
                {
                    player.SetPosition(oldPos);
                    invalidTurn = true;
                }
                if (!maze.GetAdjList()[oldPos].Contains(player.GetPosition()) && !invalidTurn)
                {
                    if (player.GetPosition() == maze.GetEndPoint() && player.GetHasKey() == true)
                    {
                        hasWon = true;
                    }
                    // Making a list of the positions of all enemies and power-ups
                    List<int> enemyPositions = new List<int>();
                    List<int> blinderPositions = new List<int>();
                    List<int> powerupPositions = new List<int>();
                    List<BlindingEnemy> blinders = new List<BlindingEnemy>();
                    List<Enemy> enemies = new List<Enemy>();
                    List<Power_Up> powerUps = new List<Power_Up>();
                    List<IVisible> toRemove = new List<IVisible>();
                    int keyPos = -1;
                    foreach (IVisible obj in objects)
                    {
                        if (obj.GetType() == "Enemy" && obj.GetName() != "Blinder")
                        {
                            enemies.Add((Enemy)obj);
                            try
                            {
                                ((Enemy)obj).Move(player.GetPosition());
                            }
                            catch
                            {
                                hasLost = true;
                            }
                            enemyPositions.Add(obj.GetPosition());
                        }
                        else if (obj.GetType() == "Enemy" && obj.GetName() == "Blinder")
                        {
                            try
                            {
                                ((Enemy)obj).Move(player.GetPosition());
                            }
                            catch
                            {
                                BlindingEnemy.BlinderRemoved();
                                BlindingEnemy.SetTimeBlinded(5 * (Blinders - 1));
                                FOV = 3;
                                toRemove.Add(obj);
                            }
                            blinders.Add((BlindingEnemy)obj);
                            blinderPositions.Add(obj.GetPosition());
                        }
                        else if (obj.GetType() == "Power-up")
                        {
                            powerupPositions.Add(obj.GetPosition());
                            powerUps.Add((Power_Up)obj);
                        }
                        else if (obj.GetType() == "Key")
                        {
                            keyPos = obj.GetPosition();
                        }
                    }
                    foreach (BlindingEnemy blinder in blinders)
                    {
                        if (blinder.GetPosition() == player.GetPosition())
                        {
                            BlindingEnemy.BlinderRemoved();
                            BlindingEnemy.SetTimeBlinded(5 * BlindingEnemy.GetBlindersCount());
                            FOV = 3;
                            toRemove.Add(blinder);
                        }
                    }
                    foreach (IVisible obj in toRemove)
                    {
                        objects.Remove(obj);
                    }
                    if (enemyPositions.Contains(player.GetPosition()))
                    {
                        hasLost = true;
                    }
                    else if (powerupPositions.Contains(player.GetPosition()))
                    {
                        foreach (Power_Up powerUp in powerUps)
                        {
                            if (powerUp.GetPosition() == player.GetPosition())
                            {
                                player.AddToInventory(powerUp);
                                objects.Remove(powerUp);
                            }
                        }
                    }
                    else if (keyPos == player.GetPosition())
                    {
                        player.GotKey();
                        Key tempKey = new Key(-1, maze);
                        foreach (IVisible obj in objects)
                        {
                            if (obj.GetType() == "Key")
                            {
                                tempKey = (Key)obj;
                            }
                        }
                        objects.Remove(tempKey);
                    }
                }
                else
                {
                    player.SetPosition(oldPos);
                }
            }
            stopwatch.Stop();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            if (hasWon)
            {
                AddToLeaderboard((int)stopwatch.Elapsed.TotalSeconds, difficulty);
                Console.WriteLine($@"
                                                                                                                                                
                                                                                                                                                
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


                                                               Your time was: {FormatTime((int)stopwatch.Elapsed.TotalSeconds)}                                                                                                                                       
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
        static int TakeTurn(int pos)
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow)
            {
                pos = maze.GetDirection(pos, Dir.up);
            }
            else if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow)
            {
                pos = maze.GetDirection(pos, Dir.left);
            }
            else if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow)
            {
                pos = maze.GetDirection(pos, Dir.down);
            }
            else if (key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow)
            {
                pos = maze.GetDirection(pos, Dir.right);
            }
            else if (key.Key == ConsoleKey.E)
            {
                player.ShowInventory();
                throw new NotInListException();
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                HowToPlay();
                throw new NotInListException();
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
                    switch (yPos)
                    {
                        case 1: return 1;
                        case 2: return 2;
                        case 3: return 3;
                        case 4: return 4;
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
        static int SetDifficulty()
        {
            int yPos = 1;
            Console.Clear();
            Console.WriteLine(@"Select a difficulty: 
  Practice
  Easy
  Meduim
  Hard
  Insane");
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
                    switch (yPos)
                    {
                        case 1:
                            size = 25;
                            BaseEnemies = 0;
                            Ghosts = 0;
                            Blinders = 0;
                            Stuns = 10;
                            Hammers = 5;
                            DefaultFOV = 15;
                            break;
                        case 2:
                            size = 15;
                            BaseEnemies = 2;
                            Ghosts = 0;
                            Blinders = 1;
                            Stuns = 10;
                            Hammers = 4;
                            DefaultFOV = 10;
                            break;
                        case 3:
                            size = 20;
                            BaseEnemies = 3;
                            Ghosts = 1;
                            Blinders = 2;
                            Stuns = 5;
                            Hammers = 3;
                            DefaultFOV = 10;
                            break;
                        case 4:
                            size = 25;
                            BaseEnemies = 3;
                            Ghosts = 3;
                            Blinders = 3;
                            Stuns = 4;
                            Hammers = 2;
                            DefaultFOV = 8;
                            break;
                        case 5:
                            size = 25; 
                            BaseEnemies = 2;
                            Ghosts = 4;
                            Blinders = 5;
                            Stuns = 3;
                            Hammers = 1;
                            DefaultFOV = 5;
                            break;
                    }
                    FOV = DefaultFOV;
                    maze = new Maze(size, random);
                    player = new Player(maze, random);
                    return yPos;
                }
                else if ((key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow) && yPos > 1)
                {
                    yPos--;
                }
                else if ((key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow) && yPos < 5)
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
            List<string> difficulties = new List<string>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string[] temps = new string[2];
                while (!sr.EndOfStream)
                {
                    temps = sr.ReadLine().Split(' ');
                    names.Add(temps[0]);
                    times.Add(int.Parse(temps[1]));
                    difficulties.Add(temps[2]);
                }
            }
            ConsoleKeyInfo key;
            int yPos = 1;
            while (true)
            {

                Console.SetCursorPosition(0, 0);
                Console.WriteLine($@"Would you like to see:
  All times
  Practice mode
  Easy difficulty
  Medium difficulty
  Hard difficulty
  Insane difficulty
  Return to menu");
                Console.CursorLeft = 0;
                Console.Write(" ");
                Console.SetCursorPosition(0, yPos);
                Console.Write(">");
                key = Console.ReadKey(true);
                string thisDifficulty = "";
                if (key.Key == ConsoleKey.Enter)
                {
                    switch (yPos)
                    {
                        case 1:
                            thisDifficulty = "All";
                            break;
                        case 2:
                            thisDifficulty = "Practice";
                            break;
                        case 3:
                            thisDifficulty = "Easy";
                            break;
                        case 4:
                            thisDifficulty = "Medium";
                            break;
                        case 5:
                            thisDifficulty = "Hard";
                            break;
                        case 6:
                            thisDifficulty = "Insane";
                            break;
                        case 7:
                            return;
                    }
                    List<string> theseNames = new List<string>();
                    List<int> theseTimes = new List<int>();
                    List<string> theseDifficulties = new List<string>();
                    for (int i = 0; i < names.Count(); i++)
                    {
                        if (difficulties[i] == thisDifficulty || thisDifficulty == "All")
                        {
                            theseNames.Add(names[i]);
                            theseTimes.Add(times[i]);
                            theseDifficulties.Add(difficulties[i]);
                        }
                    }
                    Console.Clear();
                    for (int i = 0; i < theseNames.Count(); i++)
                    {
                        Console.SetCursorPosition(30, i);
                        Console.WriteLine($"{theseNames[i]}: ");
                        Console.SetCursorPosition(60, i);
                        Console.WriteLine($"{FormatTime(theseTimes[i])}");
                        Console.SetCursorPosition(90, i);
                        Console.WriteLine($"{theseDifficulties[i]} difficulty");
                    }
                }
                else if ((key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow) && yPos > 1)
                {
                    yPos--;
                }
                else if ((key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow) && yPos < 7)
                {
                    yPos++;
                }
            }
        }
        static void AddToLeaderboard(int seconds, int difficulty)
        {
            // Getting the difficulty as a string to add
            string strDifficulty = "";
            switch (difficulty)
            {
                case 1:
                    strDifficulty = "Practice";
                    break;
                case 2:
                    strDifficulty = "Easy";
                    break;
                case 3:
                    strDifficulty = "Medium";
                    break;
                case 4:
                    strDifficulty = "Hard";
                    break;
                case 5:
                    strDifficulty = "Insane";
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            string name = "";
            bool validName = false;
            Console.WriteLine("Enter your name:");
            while (!validName)
            {
                name = Console.ReadLine();
                if (name.Length > 0 && !name.Contains(' ') && name.Length <= 25) validName = true;
                else
                {
                    Console.Clear();
                    Console.WriteLine("Please enter another name. Your name must have at between 1 and 25 characters, and not include any spaces.");
                }
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
            lines = InsertionSort(name, seconds, strDifficulty, lines);
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
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"██");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($": You\n");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"██");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($": The key. You will need to pick this up before you can go through the exit.\n");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"██");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($": The exit. Get to this with the key to win.\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($": The base enemy which moves on average every two turns and will kill you if it is on the same square as you.\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($": A ghost enemy that has a 1 in 3 chance to be able to pass through a wall in the direction of the player each move.\n");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"[]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($": A blinding enemy that, when within a space of you will blind you for five turns.\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"()");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($": A stun Power-Up which when used will freeze all enemies for an average of two turns.\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"()");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($": A hammer Power-Up which when used allow you to break any wall adjacent wall.\n");
            Console.ReadKey();
        }
        static string FormatTime(int seconds)
        {
            int[] times = new int[3];
            times[0] = seconds / 3600; // Hours
            times[1] = (seconds - (times[0] * 3600)) / 60; // Minutes
            times[2] = seconds - (times[0] * 3600) - (times[1] * 60); // Seconds
            string[] isPlural = new string[3];
            bool[] exists = new bool[3];
            string[] names = { "hour", "minute", "second" };
            for (int i = 0; i < 3; i++)
            {
                isPlural[i] = times[i] != 1 ? "s" : "";
                exists[i] = times[i] != 0;
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
                    returnString += $"{times[i]} {names[i]}{isPlural[i]}";
                    if (i == secondLastIndex) returnString += " and ";
                    else if (i != lastIndex) returnString += ", ";
                }
            }
            return returnString;
        }
        public static void SetFOV(int InFOV)
        {
            FOV = InFOV;
        }
        public static void SetDefaultFOV()
        {
            FOV = DefaultFOV;
        }
        static List<string> InsertionSort(string name, int time, string difficulty, List<string> list)
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
                    returnList.Add($"{name} {time} {difficulty}");
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
                if (choice == 1) PlayGame();
                else if (choice == 2) DisplayLeaderBoard();
                else if (choice == 3) HowToPlay();
                else exit = true;
            }
        }
        public static void DisplayMaze()
        {
            maze.DisplayGraph(objects, FOV);
        }
    }
}
