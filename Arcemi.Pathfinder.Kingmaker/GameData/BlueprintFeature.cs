using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class BlueprintFeature : BlueprintData
    {
        public const string TypeRef = "cb208b98ceacca84baee15dba53b1979, BlueprintFeature";

        public BlueprintFeature(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BlueprintFeatureComponent> Components => A.List(factory: BlueprintFeatureComponent.Factory);
    }
}
