using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class ProgressionSelectionSourceModel : RefModel
    {
        public ProgressionSelectionSourceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Blueprint => A.Value<string>();
    }
}