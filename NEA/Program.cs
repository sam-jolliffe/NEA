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
            int size = 15;
            Maze maze = new Maze(size);
            Dictionary<int, List<int>> adjList = maze.createGraph();
            List<int> ints = maze.getEdges(42);
            foreach (int i in ints.ToList())
            {
                maze.removeEdge(42, i);
            }
            maze.displayGraph();
            Console.ReadKey();
        }
    }
}
