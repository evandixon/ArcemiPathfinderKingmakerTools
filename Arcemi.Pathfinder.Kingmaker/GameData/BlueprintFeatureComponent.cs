using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class BlueprintFeatureComponent : Model
    {
        public BlueprintFeatureComponent(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Name { get => A.Value<string>("name"); set => A.Value(value, "name"); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }

        public static BlueprintFeatureComponent Factory(ModelDataAccessor accessor)
        {
            return new BlueprintFeatureComponent(accessor);
        }
    }
}
