using Arcemi.Pathfinder.Kingmaker.GameData.Enums;
using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintParametrizedFeature : BlueprintFeature
    {
        public new const string TypeRef = "f3444d227e0a4610998bc7bbee404fcd, BlueprintParametrizedFeature";

        public BlueprintParametrizedFeature(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public FeatureParameterType ParameterType => Enum.Parse<FeatureParameterType>(A.Value<string>());

        public ListValueAccessor<BlueprintReference> BlueprintParameterVariants => A.ListValue<BlueprintReference>();
        public ListValueAccessor<BlueprintReference> CustomParameterVariants => A.ListValue<BlueprintReference>();

        public List<string> GetParameterValues()
        {
            switch (ParameterType)
            {
                case FeatureParameterType.SpellSchool:
                    return Enum.GetValues<SpellSchool>().Select(s => s.ToString("f")).ToList();
                case FeatureParameterType.WeaponCategory:
                    return Enum.GetValues<WeaponCategory>().Select(s => s.ToString("f")).ToList();
                case FeatureParameterType.Custom:
                case FeatureParameterType.SpellSpecialization:
                case FeatureParameterType.LearnSpell:
                case FeatureParameterType.Skill:
                case FeatureParameterType.FeatureSelection:
                default:
                    return new List<string>();
            }
        }
    }
}
