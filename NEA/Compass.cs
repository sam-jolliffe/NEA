using System;
using System.Collections.Generic;

namespace NEA
{
    internal class Compass : Power_Up
    {
        bool isVisible = false;
        private int DisplayPosition;
        public Compass(Maze maze, Random ran, List<int> objectPositions) : base(maze, ran, objectPositions)
        {
        }
        public Compass(Maze maze, Random ran, int position) : base(maze, ran, position)
        {
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.Blue;
        }
        public override string GetDescription()
        {
            return "Allows the user to see a dot on the outside of their FOV to see the direction of the exit.";
        }
        public override string GetName()
        {
            return "Compass";
        }
        public int GetDisplayNode(int playerPos, int endPos, int FOV)
        {
            if (Math.Pow(maze.GetXcoordinate(playerPos) - maze.GetXcoordinate(endPos), 2) + Math.Pow(maze.GetYcoordinate(playerPos) - maze.GetYcoordinate(endPos), 2) <= Math.Pow(FOV, 2))
            {
                isVisible = false;
                return 0;
            }
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
            foreach (double angle in angles)
            {
                if (angle > greatestNum)
                {
                    greatestNum = angle;
                }
            }
            // Works out the angle from the user to the end.
            double angleFromUser = Math.Acos(greatestNum) * 180/Math.PI;

            return 2;
        }
        public override void Use(int playerPos)
        {
            isVisible = true;
        }
    }
}
