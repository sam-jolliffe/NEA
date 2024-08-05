using System;
using System.Collections.Generic;

namespace NEA
{
    internal class GhostEnemy : Enemy
    {
        public GhostEnemy(Maze mazeIn, Random ran, List<int> objectPositions, int playerPos) : base(mazeIn, ran, objectPositions, playerPos)
        {
        }
        public override void move(int playerPos)
        {
            if (r.Next(0, 11) < 5)
            {
                base.move(playerPos);
            }
            else
            {
                int enemyX = maze.getXcoordinate(Position);
                int enemyY = maze.getYcoordinate(Position);
                int playerX = maze.getXcoordinate(playerPos);
                int playerY = maze.getYcoordinate(playerPos);
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
                Position = maze.getDirection(Position, direction);
            }
        }
    }
}
