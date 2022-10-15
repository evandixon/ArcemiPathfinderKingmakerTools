using System.Collections;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public interface IModelContainer : IEnumerable
    {
        void Refresh();
    }
}