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
            spawn();
            Inventory = new List<Power_Up>();
            hasKey = false;
        }
        public int getPosition()
        {
            return Position;
        }
        public void setPosition(int newPos)
        {
            Position = newPos;
        }
        public List<Power_Up> getInventory()
        {
            return Inventory;
        }
        public void showInventory()
        {
            int yPos = 3;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("You have: \n\n");
            foreach (Power_Up powerup in Inventory)
            {
                Console.WriteLine($@"   {powerup.getName()}:
   {powerup.getDescription()}
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
                    // If the user presses enter, it uses that power-up
                    Inventory[yPos / 3 - 1].use();
                    Inventory.Remove(Inventory[yPos / 3 - 1]);
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
        public void addToInventory(Power_Up powerup)
        {
            Inventory.Add(powerup);
        }
        public void removeFromInventory(Power_Up powerup)
        {
            Inventory.Remove(powerup);
        }
        public string getType()
        {
            return "Player";
        }
        public int getXpos()
        {
            return Xpos;
        }
        public int getYpos()
        {
            return Ypos;
        }
        public bool getHasKey()
        {
            return hasKey;
        }
        public void gotKey()
        {
            hasKey = true;
        }
        public void spawn()
        {
            Position = r.Next(0, Maze.getXsize() * Maze.getYsize() - 1);
            Xpos = Maze.getXcoordinate(Position);
            Ypos = Maze.getYcoordinate(Position);
        }
    }
}
