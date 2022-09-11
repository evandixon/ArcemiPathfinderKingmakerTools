using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.Models
{
    public class ClassBlueprintModel
    {
        public string Id { get; set; }
        public string SpellbookId { get; set; }
        public bool IsMythic { get; set; }
        public ProgressionBlueprintModel Progression { get; set; }
        public List<ClassArchetypeBlueprintModel> Archetypes { get; set; } = new();
    }
}
