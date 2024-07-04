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
        private List<Power_Up> Inventory;
        private int Position;
        private int Xpos;
        private int Ypos;
        public Player(Maze maze, Random ran)
        {
            r = ran;
            spawn(maze);
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
            Console.WriteLine("You have: \n\n");
            foreach (Power_Up powerup in Inventory)
            {
                Console.WriteLine($@"{powerup.getName()}:
{powerup.getDescription()}
 ");
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

        public bool spawn(Maze maze)
        {
            Position = r.Next(0, maze.getXsize() * maze.getYsize() - 1);
            Xpos = maze.getXcoordinate(Position);
            Ypos = maze.getYcoordinate(Position);
            return true;
        }
    }
}
