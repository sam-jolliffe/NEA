using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    internal interface IVisible
    {
        int getPosition();
        int getYpos();
        int getXpos();
        bool spawn(Maze maze);
    }
}
