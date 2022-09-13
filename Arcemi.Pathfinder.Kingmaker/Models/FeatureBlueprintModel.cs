using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.Models
{
    public class FeatureBlueprintModel
    {
        public string Id { get; set; }
        public bool IsSelection { get; set; }
        public List<string> RemoveFeaturesIdOnApply { get; set; } = new();
        public List<string> SelectionFeatureIdOptions { get; set; } = new();
    }
}
