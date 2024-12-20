﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NEA
{
    public enum Dir { up, right, down, left }
    internal class Program
    {
        // These numbers are: Size, BaseEnemies, Ghosts, Blinders, Freezers, Stuns, Hammers, Knives, Shields, DefaultFOV
        static readonly int[][] DifficultyStats = {
            new int[] { 25, 0, 0, 0, 0, 10, 10, 5, 5, 10}, // Practice
            new int[] { 15, 2, 0, 2, 2, 10, 5, 5, 5, 10}, // Easy
            new int[] { 20, 3, 1, 2, 1, 5, 3, 2, 3, 10}, // Medium
            new int[] { 25, 3, 3, 3, 3, 4, 2, 2, 2, 8}, // Hard
            new int[] { 25, 3, 4, 5, 3, 3, 1, 1, 1, 5} }; // Insane
        static readonly Random random = new Random();
        static Maze maze;
        static Player player;
        static int DifficultyNum;
        static int timeFrozen;
        static int timeBlinded;
        static int FOV;
        static List<IVisible> objects;
        static void Main(string[] args)
        {
            Console.WriteLine("Please maximise the window before running the game");
            Console.ReadKey();
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
        static void PlayGame()
        {
            SetDifficulty();
            if (DifficultyNum == 5) return;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            objects = AddObjects();
            maze.CreateGraph();
            maze.GenerateMaze(player.GetPosition(), objects);
            List<int> treasureRoomNodes = maze.GetTreasureRoomNodes();
            foreach (int roomNode in treasureRoomNodes)
            {
                int PowerUpType = random.Next(1, 5);
                switch (PowerUpType)
                {
                    case 1:
                        objects.Add(new Stun(maze, random, roomNode));
                        break;
                    case 2:
                        objects.Add(new Hammer(maze, random, roomNode));
                        break;
                    case 3:
                        objects.Add(new Knife(maze, random, roomNode));
                        break;
                    case 4:
                        objects.Add(new Shield(maze, random, roomNode));
                        break;
                }
            }
            objects.Add(new Key(maze.GetKeyPosition(), maze));
            objects.Add(player);
            int oldPos;
            bool hasWon = false;
            bool hasLost = false;
            timeFrozen = 0;
            timeBlinded = 0;
            // Keeps taking a move and re-displaying the board until the user reaches the end
            Console.SetCursorPosition(0, 0);
            maze.DisplayGraph(objects, FOV);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!hasWon && !hasLost)
            {
                if (timeFrozen > 0)
                {
                    timeFrozen--;
                }
                if (timeBlinded > 0)
                {
                    timeBlinded--;
                }
                else
                {
                    SetDefaultFOV();
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
                    if (timeFrozen > 0)
                    {
                        player.SetPosition(oldPos);
                    }
                    if (player.GetPosition() == maze.GetEndPoint() && player.GetHasKey() == true)
                    {
                        hasWon = true;
                    }
                    // Making a list of the positions of all enemies and power-ups
                    List<int> enemyPositions = new List<int>();
                    List<int> powerupPositions = new List<int>();
                    List<BlindingEnemy> blinders = new List<BlindingEnemy>();
                    List<Enemy> enemies = new List<Enemy>();
                    List<Power_Up> powerUps = new List<Power_Up>();
                    List<IVisible> toRemove = new List<IVisible>();
                    int keyPos = -1;
                    foreach (IVisible obj in objects)
                    {
                        switch (obj.GetType())
                        {
                            case "Enemy":
                                switch (obj.GetName())
                                {
                                    case "Blinder":
                                        try
                                        {
                                            ((Enemy)obj).Move(player.GetPosition());
                                            blinders.Add((BlindingEnemy)obj);
                                        }
                                        catch
                                        {
                                            FOV = 3;
                                            timeBlinded = 5;
                                            toRemove.Add(obj);
                                        }
                                        blinders.Add((BlindingEnemy)obj);
                                        break;
                                    case "Freezer":
                                        try
                                        {
                                            ((Enemy)obj).Move(player.GetPosition());
                                        }
                                        catch
                                        {
                                            timeFrozen = 5;
                                            toRemove.Add(obj);
                                        }
                                        break;
                                    default:
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
                                        break;
                                }
                                break;
                            case "Power-up":
                                powerupPositions.Add(obj.GetPosition());
                                powerUps.Add((Power_Up)obj);
                                break;
                            case "Key":
                                keyPos = obj.GetPosition();
                                break;
                            default:
                                break;
                        }
                    }
                    foreach (IVisible obj in toRemove)
                    {
                        objects.Remove(obj);
                    }
                    if (enemyPositions.Contains(player.GetPosition()))
                    {
                        hasLost = true;
                        Power_Up shield = new Shield();
                        foreach (Power_Up PowerUp in player.GetInventory())
                        {
                            if (PowerUp.GetName() == "Shield" && hasLost == true)
                            {
                                hasLost = false;
                                ((Shield)PowerUp).Use(player.GetPosition(), ref objects);
                                shield = PowerUp;
                            }
                        }
                        if (!hasLost)
                        {
                            player.RemoveFromInventory(shield);
                        }
                    }
                    else if (powerupPositions.Contains(player.GetPosition()))
                    {
                        foreach (Power_Up powerUp in powerUps)
                        {
                            if (powerUp.GetPosition() == player.GetPosition())
                            {
                                objects.Remove(powerUp);
                                player.AddToInventory(powerUp);
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
                AddToLeaderboard((int)stopwatch.Elapsed.TotalSeconds, DifficultyNum);
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
        static void SetDifficulty()
        {
            int yPos = 1;
            Console.Clear();
            Console.WriteLine(@"Select a difficulty: 
  Practice
  Easy
  Meduim
  Hard
  Insane
  Return to menu");
            ConsoleKeyInfo key;
            while (true)
            {
                Console.CursorLeft = 0;
                Console.Write(" ");
                Console.SetCursorPosition(0, yPos);
                Console.Write(">");
                key = Console.ReadKey(true);
                DifficultyNum = -1;
                if (key.Key == ConsoleKey.Enter)
                {
                    DifficultyNum = yPos - 1;
                    if (DifficultyNum != 5)
                    {
                        FOV = DifficultyStats[DifficultyNum][9];
                        maze = new Maze(DifficultyStats[DifficultyNum][0], random);
                        player = new Player(maze, random);
                    }
                    return;
                }
                else if ((key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow) && yPos > 1)
                {
                    yPos--;
                }
                else if ((key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow) && yPos < 6)
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
                        Console.CursorLeft = 30;
                        Console.Write($"{theseNames[i]}: ");
                        Console.CursorLeft = 60;
                        Console.Write($"{FormatTime(theseTimes[i])}");
                        Console.CursorLeft = 120;
                        Console.Write($"{theseDifficulties[i]} difficulty \n");
                        Console.CursorLeft = 30;
                        for (int j = 0; j < 110; j++)
                        {
                            Console.Write("-");
                        }
                        Console.Write("\n");
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
                case 0:
                    strDifficulty = "Practice";
                    break;
                case 1:
                    strDifficulty = "Easy";
                    break;
                case 2:
                    strDifficulty = "Medium";
                    break;
                case 3:
                    strDifficulty = "Hard";
                    break;
                case 4:
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
                    Console.WriteLine("Please enter another name. Your name must have between 1 and 25 characters, and not include any spaces.");
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
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            IVisible[] allTypes = { new Player(), new Key(), new BaseEnemy(), new GhostEnemy(), new BlindingEnemy(), new FreezingEnemy(), new Stun(), new Hammer(), new Torch(), new Knife(), new Compass(), new Shield() };
            foreach (IVisible obj in allTypes)
            {
                Console.ForegroundColor = obj.GetColour();
                Console.Write(obj.GetSprite());
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($":  {obj.GetDescription()}\n\n");
            }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"██");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($":  The exit. Get to this with the key to win.\n");
            Console.WriteLine(@"Controls:
W/Up Arrow : Move Up
A/Left Arrow : Move Left
S/Down Arrrow : Move Down
D/Right Arrrow : Move Right
E : Open Inventory
Escape : Open Help Menu");
            Console.ReadKey();
        }
        static string FormatTime(int seconds)
        {
            int[] times = new int[5];
            times[0] = seconds / 31536000; // Years
            times[1] = (seconds - (times[0] * 31536000)) / 86400; // Days
            times[2] = (seconds - (times[0] * 31536000) - (times[1] * 86400)) / 3600; // Hours
            times[3] = (seconds - (times[0] * 31536000) - (times[1] * 86400) - (times[2] * 3600)) / 60; // Minutes
            times[4] = seconds - (times[0] * 31536000) - (times[1] * 86400) - (times[2] * 3600) - (times[3] * 60);
            string[] isPlural = new string[5];
            bool[] exists = new bool[5];
            string[] names = { "year", "day", "hour", "minute", "second" };
            for (int i = 0; i < 5; i++) isPlural[i] = "";
            for (int i = 0; i < 5; i++)
            {
                if (times[i] != 1)
                {
                    isPlural[i] = "s";
                }
                if (times[i] == 0)
                {
                    exists[i] = false;
                }
                else
                {
                    exists[i] = true;
                }
            }
            int count = -1;
            string returnString = "";
            int lastCount = 0;
            int secondLastCount = 0;
            for (int i = 0; i < 5; i++)
            {
                if (exists[i]) lastCount = i;
            }
            for (int i = 0; i < lastCount; i++)
            {
                if (exists[i]) secondLastCount = i;
            }
            foreach (bool b in exists)
            {
                count++;
                if (b)
                {
                    if (count == lastCount)
                    {
                        returnString += $"{times[count]} {names[count]}{isPlural[count]}";
                    }
                    else if (count == secondLastCount)
                    {
                        returnString += $"{times[count]} {names[count]}{isPlural[count]} and ";
                    }
                    else
                    {
                        returnString += $"{times[count]} {names[count]}{isPlural[count]}, ";
                    }
                }
            }
            return returnString;
        }
        public static void IncreaseFOV() => DifficultyStats[DifficultyNum][9] += 5;
        public static void SetDefaultFOV() => FOV = DifficultyStats[DifficultyNum][9];
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
            int index = 0;
            bool hasAdded = false;
            while (index < list.Count())
            {
                if ((index >= times.Count || time <= times[index]) && !hasAdded)
                {
                    returnList.Add($"{name} {time} {difficulty}");
                    hasAdded = true;
                }
                returnList.Add(list[index]);
                index++;
            }
            return returnList;
        }
        public static void DisplayMaze()
        {
            maze.DisplayGraph(objects, FOV);
        }
        public static List<IVisible> GetObjects()
        {
            return objects;
        }
        static List<IVisible> AddObjects()
        {
            objects = new List<IVisible>();
            List<int> objectPositions = new List<int>(player.GetPosition());
            // Adding enemies
            for (int i = 0; i < DifficultyStats[DifficultyNum][1]; i++)
            {
                objects.Add(new BaseEnemy(maze, random, objectPositions, player.GetPosition()));
                objectPositions.Add(objects[i].GetPosition());
            }
            for (int i = 0; i < DifficultyStats[DifficultyNum][2]; i++)
            {
                objects.Add(new GhostEnemy(maze, random, objectPositions, player.GetPosition()));
                objectPositions.Add(objects[i].GetPosition());
            }
            for (int i = 0; i < DifficultyStats[DifficultyNum][3]; i++)
            {
                objects.Add(new BlindingEnemy(maze, random, objectPositions, player.GetPosition()));
                objectPositions.Add(objects[i].GetPosition());
            }
            for (int i = 0; i < DifficultyStats[DifficultyNum][4]; i++)
            {
                objects.Add(new FreezingEnemy(maze, random, objectPositions, player.GetPosition()));
                objectPositions.Add(objects[i].GetPosition());
            }
            // Adding power-ups
            for (int i = 0; i < DifficultyStats[DifficultyNum][5]; i++)
            {
                Stun tempStun = new Stun(maze, random, objectPositions);
                objects.Add(tempStun);
                objectPositions.Add(tempStun.GetPosition());
            }
            for (int i = 0; i < DifficultyStats[DifficultyNum][6]; i++)
            {
                Hammer tempHammer = new Hammer(maze, random, objectPositions);
                objects.Add(tempHammer);
                objectPositions.Add(tempHammer.GetPosition());
            }
            for (int i = 0; i < DifficultyStats[DifficultyNum][7]; i++)
            {
                Knife tempKnife = new Knife(maze, random, objectPositions);
                objects.Add(tempKnife);
                objectPositions.Add(tempKnife.GetPosition());
            }
            for (int i = 0; i < DifficultyStats[DifficultyNum][8]; i++)
            {
                Shield tempShield = new Shield(maze, random, objectPositions);
                objects.Add(tempShield);
                objectPositions.Add(tempShield.GetPosition());
            }
            Torch tempTorch = new Torch(maze, random, objectPositions);
            objects.Add(tempTorch);
            objectPositions.Add(tempTorch.GetPosition());

            Compass tempCompass = new Compass(maze, random, objectPositions);
            objects.Add(tempCompass);
            objectPositions.Add(tempCompass.GetPosition());

            return objects;
        }
        public static void AddObject(IVisible obj)
        {
            objects.Add(obj);
        }
        public static void RemoveFromObjects(IVisible obj)
        {
            objects.Remove(obj);
        }
        public static void SetTimeFrozen(int TimeFrozen)
        {
            timeFrozen = TimeFrozen;
        }
    }
}
