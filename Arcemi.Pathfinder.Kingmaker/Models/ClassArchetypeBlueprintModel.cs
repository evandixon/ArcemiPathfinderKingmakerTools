using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.Models
{
    public class ClassArchetypeBlueprintModel
    {
        public string ReplacementSpellbook { get; set; }
        public bool RemoveSpellbook { get; set; }
        public List<ProgressionLevelBlueprintModel> AddFeatures { get; set; } = new();
        public List<ProgressionLevelBlueprintModel> RemoveFeatures { get; set; } = new();
    }
}
