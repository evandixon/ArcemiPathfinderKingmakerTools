using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.Models
{
    public class ProgressionLevelBlueprintModel
    {
        public int Level { get; set; }
        public List<FeatureBlueprintModel> Features { get; set; } = new();
    }
}
