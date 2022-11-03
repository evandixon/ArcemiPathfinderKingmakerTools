using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Player
{
    public class PlayerGlobalMapsSpellManagerModel : RefModel
    {
        public PlayerGlobalMapsSpellManagerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int GlobalSpellPower { get => A.Value<int>("m_GlobalSpellPower"); set => A.Value(value, "m_GlobalSpellPower"); }
        public ListAccessor<PlayerGlobalMapsSpellModel> SpellBook => A.List("m_SpellBook", a => new PlayerGlobalMapsSpellModel(a));
        public ListAccessor<PlayerGlobalMapsActionBarSlotModel> GlobalMapActionBarSlots => A.List("m_GlobalMapActionBarSlots", a => new PlayerGlobalMapsActionBarSlotModel(a));
    }
}
