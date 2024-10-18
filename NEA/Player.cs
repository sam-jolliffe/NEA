using System;
using System.Collections.Generic;
using System.Linq;

namespace NEA
{
    public class Player : IVisible
    {
        private static Random r;
        private readonly List<Power_Up> Inventory;
        private int Position;
        private int Xpos;
        private int Ypos;
        private readonly Maze Maze;
        private bool hasKey;
        public Player(Maze maze, Random ran)
        {
            r = ran;
            Maze = maze;
            Spawn();
            Inventory = new List<Power_Up>();
            hasKey = false;
        }
        public Player()
        {
        }
        public string GetDescription()
        {
            return "You";
        }
        public string GetSprite()
        {
            return "██";
        }
        public ConsoleColor GetColour()
        {
            return ConsoleColor.DarkBlue;
        }
        public string GetName()
        {
            return "Player";
        }
        public int GetPosition()
        {
            return Position;
        }
        public void SetPosition(int newPos)
        {
            Position = newPos;
        }
        public List<Power_Up> GetInventory()
        {
            return Inventory;
        }
        public void ShowInventory()
        {
            int yPos = 3;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("You have: \n\n");
            foreach (Power_Up powerup in Inventory)
            {
                Console.WriteLine($@"   {powerup.GetName()}:
   {powerup.GetDescription()}
");
            }
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
                    if (Inventory.Count() == 0) return;
                    // If the user presses enter, it uses that power-up
                    Inventory[yPos / 3 - 1].Use(Position);
                    if (Inventory[yPos / 3 - 1].GetName() == "Compass")
                    {
                        Program.AddObject(Inventory[yPos / 3 - 1]);
                    }
                    Inventory.Remove(Inventory[yPos / 3 - 1]);
                    Console.Clear();
                    return;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    return;
                }
                else if ((key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow) && yPos > 3)
                {
                    yPos -= 3;
                }
                else if ((key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow) && yPos < Inventory.Count() * 3)
                {
                    yPos += 3;
                }
            }
        }
        public void AddToInventory(Power_Up powerup)
        {
            Inventory.Add(powerup);
        }
        public void RemoveFromInventory(Power_Up powerup)
        {
            Inventory.Remove(powerup);
        }
        new public string GetType()
        {
            return "Player";
        }
        public int GetXpos()
        {
            return Xpos;
        }
        public int GetYpos()
        {
            return Ypos;
        }
        public bool GetHasKey()
        {
            return hasKey;
        }
        public void GotKey()
        {
            hasKey = true;
        }
        public void Spawn()
        {
            // ObjectPositions is justy a blank list, as the player is always the first objects created, so there is nowhere it can't spawn
            Position = r.Next(0, Maze.GetXsize() * Maze.GetYsize() - 1);
            Xpos = Maze.GetXcoordinate(Position);
            Ypos = Maze.GetYcoordinate(Position);
        }
    }
}
