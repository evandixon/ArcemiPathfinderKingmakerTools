using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class LearnedSpellModel : RefModel
    {
        public LearnedSpellModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public bool CopiedFromScroll { get => A.Value<bool>(); set => A.Value(value); }
    }
}