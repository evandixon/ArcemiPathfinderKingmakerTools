using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.Models
{
    public class ProgressionBlueprintModel
    {
        public string BlueprintId { get; set; }
        public List<ProgressionBlueprintLevel> Levels { get; set; } = new();
    }

    public class ProgressionBlueprintLevel
    { 
        public int Level { get; set; }
        public List<string> FeatureBlueprintIds { get; set; } = new();
    }

}
