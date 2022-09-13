using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintFeatureSelection : BlueprintFeature
    {
        public new const string TypeRef = "ea44b1ffb0675104cb178df6459f3a21, BlueprintFeatureSelection";

        public BlueprintFeatureSelection(ModelDataAccessor accessor) : base(accessor)
        {
        }

        /// <summary>
        /// List of blueprint references that can be selected
        /// </summary>
        public ListValueAccessor<string> m_AllFeatures { get => A.ListValue<string>(); }
    }
}
