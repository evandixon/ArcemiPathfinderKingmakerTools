using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

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
            if (string.Equals(type, BlueprintCharacterClass.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintCharacterClass(accessor);
            }
            if (string.Equals(type, BlueprintArchetype.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintArchetype(accessor);
            }
            if (string.Equals(type, BlueprintFeature.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintFeature(accessor);
            }
            if (string.Equals(type, BlueprintProgression.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintProgression(accessor);
            }
            return new BlueprintData(accessor);
        }
    }
}
