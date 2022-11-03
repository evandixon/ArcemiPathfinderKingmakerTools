using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Player
{
    public class PlayerGlobalMapsActionBarSlotModel : RefModel
    {
        public PlayerGlobalMapsActionBarSlotModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public PlayerGlobalMapsSpellModel SpellState => A.Object<PlayerGlobalMapsSpellModel>();
        public string UnitRef { get => A.Value<string>("m_UnitRef"); set => A.Value(value, "m_UnitRef"); }
    }
}
