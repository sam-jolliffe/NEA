using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Player(Maze maze, Random ran)
        {
            r = ran;
            Maze = maze;
            spawn();
            Inventory = new List<Power_Up>();
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
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            if (Inventory.Count > 0)
            {
                Console.WriteLine("You have: \n\n");
                foreach (Power_Up powerup in Inventory)
                {
                    Console.WriteLine($@"{powerup.getName()}:
{powerup.getDescription()}
 ");
                }
            }
            else
            {
                Console.WriteLine("You have nothing in your inventory");
            }
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
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

        public void spawn()
        {
            Position = r.Next(0, Maze.getXsize() * Maze.getYsize() - 1);
            Xpos = Maze.getXcoordinate(Position);
            Ypos = Maze.getYcoordinate(Position);
        }
    }
}
