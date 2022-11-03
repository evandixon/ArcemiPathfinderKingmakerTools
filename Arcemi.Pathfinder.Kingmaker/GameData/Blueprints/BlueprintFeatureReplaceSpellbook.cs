using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintFeatureReplaceSpellbook : BlueprintFeature
    {
        public new const string TypeRef = "43f1ee69992dc1848b97e623487f6442, BlueprintFeatureReplaceSpellbook";

        public BlueprintFeatureReplaceSpellbook(ModelDataAccessor accessor) : base(accessor)
        {
        }

        /// <summary>
        /// List of blueprint references that can be selected
        /// </summary>
        public BlueprintReference<BlueprintSpellbook> m_Spellbook => A.Value<BlueprintReference<BlueprintSpellbook>>();
    }
}
