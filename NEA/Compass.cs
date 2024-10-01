using System;
using System.Collections.Generic;

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
            List<(double, double)> EdgeVectors = new List<(double, double)>();
            for (int i = 0; i < 360; i++)
            {
                EdgeVectors.Add((FOV * Math.Cos(i), FOV * Math.Sin(i)));
            }
            (double, double) exitVector = (FOV * Math.Cos(endPos), FOV * Math.Sin(endPos));
            // Working out angles: 
            List<double> angles = new List<double>();
            foreach ((double, double) doubleDouble in EdgeVectors)
            {
                // Equation for cos(theta) from vectors
                angles.Add(((doubleDouble.Item1 * exitVector.Item1) + (doubleDouble.Item2 * exitVector.Item2))
                    /
                    (Math.Sqrt(Math.Pow(doubleDouble.Item1, 2) + Math.Pow(doubleDouble.Item2, 2)) * Math.Sqrt(Math.Pow(exitVector.Item1, 2) + Math.Pow(exitVector.Item2, 2))));

            }
            double greatestNum = 0;
            (double, double) DoubleVector = (-1, -1);
            foreach (double angle in angles)
            {
                if (angle > greatestNum)
                {
                    greatestNum = angle;
                    DoubleVector = EdgeVectors[angles.IndexOf(angle)];
                }
            }
            // Making the doubles into integers
            (int, int) NodeVector = ((int)DoubleVector.Item1, (int)DoubleVector.Item2);
            // Adding the vector to the player's position
            int node = playerPos + NodeVector.Item1 + playerPos + (maze.GetXsize() * NodeVector.Item2);
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
