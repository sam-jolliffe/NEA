using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int size = 10;
            Maze maze = new Maze(size);
            maze.createGraph();
            maze.recursiveBacktrackingConstructor(0);
            maze.displayGraph(0);
            Console.ReadKey();
            // Console.WriteLine($"");
        }
    }
}
