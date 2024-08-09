using System.Windows.Documents;
using System.Collections.Generic;
using System;

namespace NEA
{
    public interface IVisible
    {
        int GetPosition();
        int GetYpos();
        int GetXpos();
        string GetType();
        string GetName();
        string GetSprite();
        ConsoleColor GetColour();
    }
}
