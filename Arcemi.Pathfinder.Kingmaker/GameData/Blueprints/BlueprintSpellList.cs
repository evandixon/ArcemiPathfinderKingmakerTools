using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintSpellList : BlueprintData
    {
        public const string TypeRef = "1585acbfcc93f0a439fa39dde12070c3, BlueprintSpellList";

        public BlueprintSpellList(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BlueprintSpellListEntry> SpellsByLevel => A.List<BlueprintSpellListEntry>();
    }
}
