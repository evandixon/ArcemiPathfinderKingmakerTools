using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class BlueprintProgressionLevel : Model
    {
        public BlueprintProgressionLevel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Level { get => A.Value<int>(); set => A.Value(value); }
        public ListValueAccessor<string> m_Features { get => A.ListValue<string>(); }

        public static BlueprintProgressionLevel Factory(ModelDataAccessor accessor)
        {
            return new BlueprintProgressionLevel(accessor);
        }

        public List<string> GetFeatureBlueprintIds() => m_Features.Select(f => f.Replace("!bp_", "")).ToList();
    }
}
