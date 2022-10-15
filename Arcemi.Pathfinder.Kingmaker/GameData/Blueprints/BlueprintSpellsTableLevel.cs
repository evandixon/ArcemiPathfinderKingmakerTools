using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintSpellsTableLevel : Model
    {
        public BlueprintSpellsTableLevel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListValueAccessor<int> Count { get => A.ListValue<int>(); }

        public static BlueprintSpellsTableLevel Factory(ModelDataAccessor accessor)
        {
            return new BlueprintSpellsTableLevel(accessor);
        }
    }
}
