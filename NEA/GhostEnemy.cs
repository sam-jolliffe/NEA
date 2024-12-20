﻿using System;
using System.Collections.Generic;

namespace NEA
{
    internal class GhostEnemy : Enemy
    {
        public GhostEnemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos) : base(mazeIn, ran, objectPositions, playerPos)
        {
        }
        public GhostEnemy() : base()
        {
        }
        public override string GetName()
        {
            return "Ghost";
        }
        public override string GetDescription()
        {
            return "A ghost enemy that has a 1 in 3 chance to be able to pass through a wall in the direction of the player each move.";
        }
        public override ConsoleColor GetColour()
        {
            return ConsoleColor.DarkCyan;
        }
        public override void Move(int playerPos)
        {
            if (r.Next(0, 10) < 7)
            {
                base.Move(playerPos);
            }
            else
            {
                canMove += r.Next(0, 2);
                if (canMove % 2 == 0 || canMove < 0) return;
                PreviousPositions[1] = PreviousPositions[0];
                PreviousPositions[0] = Position;
                int enemyX = maze.GetXcoordinate(Position);
                int enemyY = maze.GetYcoordinate(Position);
                int playerX = maze.GetXcoordinate(playerPos);
                int playerY = maze.GetYcoordinate(playerPos);
                List<Dir> directions = new List<Dir> { Dir.left, Dir.right, Dir.up, Dir.down };
                if (enemyX > playerX)
                {
                    directions.Remove(Dir.right);
                }
                else if (enemyX < playerX)
                {
                    directions.Remove(Dir.left);
                }
                if (enemyY > playerY)
                {
                    directions.Remove(Dir.down);
                }
                else if (enemyY < playerY)
                {
                    directions.Remove(Dir.up);
                }
                int furthest = 0;
                Dir direction = Dir.left;
                foreach (Dir dir in directions)
                {
                    if (dir == Dir.left || dir == Dir.right)
                    {
                        if (Math.Abs(playerX - enemyX) > furthest)
                        {
                            furthest = Math.Abs(playerX - enemyX);
                            direction = dir;
                        }
                    }
                    else if (dir == Dir.up || dir == Dir.down)
                    {
                        if (Math.Abs(playerY - enemyY) > furthest)
                        {
                            furthest = Math.Abs(playerY - enemyY);
                            direction = dir;
                        }
                    }
                }
                Position = maze.GetDirection(Position, direction);
            }
        }
    }
}
