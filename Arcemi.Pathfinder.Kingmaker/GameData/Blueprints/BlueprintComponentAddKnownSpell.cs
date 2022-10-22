using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintComponentAddKnownSpell : BlueprintComponent
    {
        public const string TypeRef = "8e5769c529dd1e34ea98b610c8222125, AddKnownSpell";

        public BlueprintComponentAddKnownSpell(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public BlueprintReference<BlueprintCharacterClass> m_CharacterClass => A.Value<BlueprintReference<BlueprintCharacterClass>>();
        public int SpellLevel => A.Value<int>();
        public BlueprintReference<BlueprintAbility> m_Spell => A.Value<BlueprintReference<BlueprintAbility>>();
        public BlueprintReference<BlueprintArchetype> m_Archetype => A.Value<BlueprintReference<BlueprintArchetype>>();
    }
}
