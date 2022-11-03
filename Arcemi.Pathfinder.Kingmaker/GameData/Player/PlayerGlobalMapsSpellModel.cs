using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Player
{
    public class PlayerGlobalMapsSpellModel : RefModel
    {
        public PlayerGlobalMapsSpellModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string BlueprintRef { get => A.Value<string>("m_BlueprintRef"); set => A.Value(value, "m_BlueprintRef"); }
        public TimeSpan LastUse { get => A.Value<TimeSpan>("m_LastUse"); set => A.Value(value, "m_LastUse"); }
    }
}
