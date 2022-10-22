using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintComponent : Model
    {
        public BlueprintComponent(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Name { get => A.Value<string>("name"); set => A.Value(value, "name"); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }

        public static BlueprintComponent Factory(ModelDataAccessor accessor)
        {
            var type = accessor.TypeValue();
            if (string.Equals(type, BlueprintComponentAddKnownSpell.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintComponentAddKnownSpell(accessor);
            }
            if (string.Equals(type, BlueprintComponentRemoveFeatureOnApply.TypeRef, StringComparison.Ordinal))
            {
                return new BlueprintComponentRemoveFeatureOnApply(accessor);
            }

            return new BlueprintComponent(accessor);
        }
    }
}
