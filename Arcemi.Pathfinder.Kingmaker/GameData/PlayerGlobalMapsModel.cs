using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class PlayerGlobalMapsModel : RefModel
    {
        public PlayerGlobalMapsModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public IReadOnlyList<PlayerArmyModel> Armies => A.List("m_Armies", a => new PlayerArmyModel(a));
    }
}