using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class ProgressionItemValueModel : RefModel
    {
        public ProgressionItemValueModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Level { get => A.Value<int>(); set => A.Value(value); }
        public IReadOnlyList<string> Archetypes => A.ListValue<string>();
    }
}