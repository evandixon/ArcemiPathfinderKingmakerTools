using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintFeature : BlueprintData
    {
        public const string TypeRef = "cb208b98ceacca84baee15dba53b1979, BlueprintFeature";

        public BlueprintFeature(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BlueprintComponent> Components => A.List(factory: BlueprintComponent.Factory);

        /// <summary>
        /// Gets spells added as a result of this feature
        /// </summary>
        public IEnumerable<BlueprintReference<BlueprintAbility>> GetAddedSpells(string characterClassId, string archetypeId, int? level = null)
        {
            foreach (var component in Components)
            {
                if (component is BlueprintComponentAddKnownSpell addSpellComponent)
                {
                    if (addSpellComponent.m_CharacterClass.Id != characterClassId)
                    {
                        continue;
                    }

                    if (addSpellComponent.m_Archetype != null && addSpellComponent.m_Archetype.Id != archetypeId)
                    {
                        continue;
                    }

                    if (level.HasValue && addSpellComponent.SpellLevel != level.Value)
                    {
                        continue;
                    }

                    yield return addSpellComponent.m_Spell;
                }
            }
        }
    }
}
