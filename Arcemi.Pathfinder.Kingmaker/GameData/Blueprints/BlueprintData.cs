using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;

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
    }
}
