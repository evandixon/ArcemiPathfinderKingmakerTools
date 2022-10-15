using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintProgression : BlueprintFeature
    {
        public new const string TypeRef = "bec71e89a676a99458c9e2d0804f2a0c, BlueprintProgression";

        public BlueprintProgression(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BlueprintProgressionLevel> LevelEntries => A.List(factory: BlueprintProgressionLevel.Factory);
    }
}
