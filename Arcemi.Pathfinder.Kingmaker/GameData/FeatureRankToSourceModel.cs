using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class FeatureRankToSourceModel : RefModel
    {
        public FeatureRankToSourceModel(ModelDataAccessor accessor) : base(accessor) { }

        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public int Level { get => A.Value<int>(); set => A.Value(value); }

        public static new void Prepare(IReferences refs, JObject obj)
        {
        }
    }
}