using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintArchetype : BlueprintData
    {
        public const string TypeRef = "f04f17ea4eae4fb89afa5b4021444740, BlueprintArchetype";

        public BlueprintArchetype(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string m_ReplaceSpellbook { get => A.Value<string>(); set => A.Value(value); }
        public bool RemoveSpellbook { get => A.Value<bool>(); set => A.Value(value); }
        public ListAccessor<BlueprintProgressionLevel> AddFeatures => A.List(factory: BlueprintProgressionLevel.Factory);
        public ListAccessor<BlueprintProgressionLevel> RemoveFeatures => A.List(factory: BlueprintProgressionLevel.Factory);
    }
}
