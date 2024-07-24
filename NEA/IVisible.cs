using System.Windows.Documents;
using System.Collections.Generic;
namespace NEA
{
    public interface IVisible
    {
        int getPosition();
        int getYpos();
        int getXpos();
        void spawn(List<int> objectPositions);
        string getType();
    }
}
