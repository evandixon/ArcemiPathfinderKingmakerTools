using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class ProgressionSelectionSourceModel : RefModel
    {
        public ProgressionSelectionSourceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
    }
}