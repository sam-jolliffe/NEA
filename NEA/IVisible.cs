﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    public interface IVisible
    {
        int getPosition();
        int getYpos();
        int getXpos();
        void spawn();
        string getType();
    }
}
