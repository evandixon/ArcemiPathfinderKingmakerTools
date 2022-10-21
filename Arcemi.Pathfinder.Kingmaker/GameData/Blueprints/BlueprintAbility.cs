using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintAbility : BlueprintData
    {
        public const string TypeRef = "da11db195c86e0d4dae17a2c03a4ba9a, BlueprintAbility";

        public BlueprintAbility(ModelDataAccessor accessor) : base(accessor)
        {
        }

        /// <summary>
        /// Type of ability, e.g. "Spell"
        /// </summary>
        public string Type => A.Value<string>();
    }
}
