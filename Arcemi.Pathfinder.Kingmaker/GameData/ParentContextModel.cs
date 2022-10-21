using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class ParentContextModel : Model
    {
        public ParentContextModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string AssociatedBlueprint { get => A.Value<string>(); set => A.Value(value); }
    }
}