using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintData : Model
    {
        public BlueprintData(ModelDataAccessor accessor) : base(accessor)
        {
        }


        public static BlueprintData Factory(ModelDataAccessor accessor)
        {
            var type = accessor.TypeValue();

            if (string.Equals(type, BlueprintAbility.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintAbility(accessor);
            }
            if (string.Equals(type, BlueprintArchetype.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintArchetype(accessor);
            }
            if (string.Equals(type, BlueprintCharacterClass.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintCharacterClass(accessor);
            }
            else if (string.Equals(type, BlueprintFeature.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintFeature(accessor);
            }
            if (string.Equals(type, BlueprintFeatureSelection.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintFeatureSelection(accessor);
            }
            if (string.Equals(type, BlueprintFeatureReplaceSpellbook.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintFeatureReplaceSpellbook(accessor);
            }
            if (string.Equals(type, BlueprintParametrizedFeature.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintParametrizedFeature(accessor);
            }
            if (string.Equals(type, BlueprintSpellbook.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintSpellbook(accessor);
            }
            if (string.Equals(type, BlueprintSpellList.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintSpellList(accessor);
            }
            if (string.Equals(type, BlueprintSpellsTable.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintSpellsTable(accessor);
            }
            if (string.Equals(type, BlueprintProgression.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintProgression(accessor);
            }

            return new BlueprintData(accessor);
        }

        public ListAccessor<BlueprintComponent> Components => A.List(factory: BlueprintComponent.Factory);

        /// <summary>
        /// Gets spells added as a result of this feature
        /// </summary>
        public IEnumerable<(BlueprintReference<BlueprintAbility> spellReference, int spellLevel)> GetAddedSpells(string characterClassId, string archetypeId, int? level = null)
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

                    yield return (addSpellComponent.m_Spell, addSpellComponent.SpellLevel);
                }
            }
        }
    }
}
