using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintCharacterClass : BlueprintData
    {
        public const string TypeRef = "5f2bdd25f161a0d4c97bee89cf923d77, BlueprintCharacterClass";

        public BlueprintCharacterClass(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BlueprintComponent> Components => A.List(factory: BlueprintComponent.Factory);

        public string m_Spellbook { get => A.Value<string>(); set => A.Value(value); }
        public bool IsMythic { get => A.Value<bool>(); set => A.Value(value); }
        public string m_Progression { get => A.Value<string>(); set => A.Value(value); }
        public ListValueAccessor<string> m_Archetypes { get => A.ListValue<string>(); }
    }
}
