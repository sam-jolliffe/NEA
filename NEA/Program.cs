﻿using System;
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
            int size = 3;
            Maze maze = new Maze(size);
            Dictionary<int, List<int>> adjList = maze.createGraph();
            adjList = maze.recursiveBacktracking(0);
            Console.ReadKey();
            // Console.WriteLine($"");
        }
    }
}
