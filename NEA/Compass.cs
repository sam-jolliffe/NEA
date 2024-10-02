using System;
using System.Collections.Generic;
using System.Linq;

namespace NEA
{
    public class Compass : Power_Up
    {
        private bool isVisible = false;
        public Compass(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
        public Compass(Maze maze, Random ran, int position) : base(maze, ran, position)
        {
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Red;
        }
        public override string GetDescription()
        {
            return "Allows the user to see a dot on the outside of their FOV to see the direction of the exit.";
        }
        public override string GetName()
        {
            return "Compass";
        }
        public int GetPosition(int playerPos, int endPos, int FOV)
        {
            if (isVisible)
            {
                return GetDisplayNode(playerPos, endPos, FOV);
            }
            else
            {
                return base.GetPosition();
            }
        }
        public bool GetIsVisible()
        {
            return isVisible;
        }
        public int GetDisplayNode(int playerPos, int endPos, int FOV)
        {
            if (Math.Pow(maze.GetXcoordinate(playerPos) - maze.GetXcoordinate(endPos), 2) + Math.Pow(maze.GetYcoordinate(playerPos) - maze.GetYcoordinate(endPos), 2) <= Math.Pow(FOV, 2))
            {
                isVisible = false;
                return 0;
            }
            // Vectors from the user to various places around the edge of the FOV
            // i is the bearing from horizontally right, anticlockwise
            // Item1 is the x, Item2 is the y.
            List<(double, double)> EdgeVectors = new List<(double, double)>();
            for (int i = 0; i < 360; i+=6)
            {
                EdgeVectors.Add((FOV * Math.Cos(i), FOV * Math.Sin(i)));
            }
            (double, double) exitVector = (maze.GetXcoordinate(endPos) - maze.GetXcoordinate(playerPos), maze.GetYcoordinate(playerPos) - maze.GetYcoordinate(endPos));
            // Working out angles: 
            List<double> angles = new List<double>();
            foreach ((double, double) doubleDouble in EdgeVectors)
            {
                // Equation for cos(theta) from vectors
                angles.Add(((doubleDouble.Item1 * exitVector.Item1) + (doubleDouble.Item2 * exitVector.Item2))
                    /
                    (FOV * Math.Sqrt(Math.Pow(exitVector.Item1, 2) + Math.Pow(exitVector.Item2, 2))));
                // Console.WriteLine($"Score: {angles.Last()}, X: {doubleDouble.Item1}, Y: {doubleDouble.Item2}");
            }
            double greatestNum = 0;
            (double, double) DoubleVector = (-1, -1);
            // Cos Theta ranges from -1 to 1, and we want the closest value to 1. 
            foreach (double angle in angles)
            {
                if (angle > greatestNum)
                {
                    greatestNum = angle;
                    DoubleVector = EdgeVectors[angles.IndexOf(angle)];
                }
            }
            // Console.WriteLine($"Final decided vector: X: {DoubleVector.Item1} Y: {DoubleVector.Item2}");
            // Making the doubles into integers
            (int, int) NodeVector = ((int)Math.Round(DoubleVector.Item1, MidpointRounding.AwayFromZero), (int)Math.Round(DoubleVector.Item2, MidpointRounding.AwayFromZero));
            //Console.WriteLine($"X: {NodeVector.Item1}, Y:{NodeVector.Item2} ");
            // Console.ReadKey();
            // Adding the vector to the player's position
            int node = playerPos + NodeVector.Item1 + (maze.GetXsize() * -NodeVector.Item2);

            //Console.WriteLine($"PlayerPos: {playerPos}, Node: {node}");
            //Console.ReadKey();

            Position = node;
            // Console.WriteLine(node);
            return node;
        }
        public override void Use(int playerPos)
        {
            isVisible = true;
        }
    }
}
