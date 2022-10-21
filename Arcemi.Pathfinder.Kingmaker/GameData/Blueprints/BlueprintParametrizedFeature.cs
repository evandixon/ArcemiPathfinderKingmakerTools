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
    }
}
