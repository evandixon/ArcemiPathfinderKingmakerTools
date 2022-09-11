using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.Models
{
    public class ProgressionBlueprintModel
    {
        public string BlueprintId { get; set; }
        public List<ProgressionLevelBlueprintModel> Levels { get; set; } = new();
    }
}
