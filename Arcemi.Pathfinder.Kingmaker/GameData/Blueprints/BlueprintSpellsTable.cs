using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintSpellsTable : BlueprintData
    {
        public const string TypeRef = "65cf33ed832812849867702df5c642fe, BlueprintSpellsTable";

        public BlueprintSpellsTable(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BlueprintSpellsTableLevel> Components => A.List(factory: BlueprintSpellsTableLevel.Factory);
    }
}
