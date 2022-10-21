using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintSpellListEntry : Model
    {
        public BlueprintSpellListEntry(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int SpellLevel => A.Value<int>();

        public ListValueAccessor<BlueprintReference<BlueprintAbility>> m_Spells => A.ListValue<BlueprintReference<BlueprintAbility>>();
    }
}
