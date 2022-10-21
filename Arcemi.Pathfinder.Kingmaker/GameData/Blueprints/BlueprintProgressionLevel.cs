using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintProgressionLevel : Model
    {
        public BlueprintProgressionLevel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Level { get => A.Value<int>(); set => A.Value(value); }

        public ListValueAccessor<BlueprintReference<BlueprintFeature>> m_Features { get => A.ListValue<BlueprintReference<BlueprintFeature>>(); }

        public static BlueprintProgressionLevel Factory(ModelDataAccessor accessor)
        {
            return new BlueprintProgressionLevel(accessor);
        }
    }
}
