using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintComponentRemoveFeatureOnApply : BlueprintComponent
    {
        public const string TypeRef = "f46c5f588edca9e47b0dc0509fbcf591, RemoveFeatureOnApply";

        public BlueprintComponentRemoveFeatureOnApply(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string m_Feature { get => A.Value<string>(); set => A.Value(value); }
    }
}
