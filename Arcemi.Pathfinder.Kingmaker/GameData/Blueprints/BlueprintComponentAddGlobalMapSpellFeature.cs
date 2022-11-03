using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintComponentAddGlobalMapSpellFeature : BlueprintComponent
    {
        public const string TypeRef = "d56bda60f51949639771af665a6d0959, AddGlobalMapSpellFeature";

        public BlueprintComponentAddGlobalMapSpellFeature(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public BlueprintReference<BlueprintAbility> m_Spell => A.Value<BlueprintReference<BlueprintAbility>>();
    }
}
