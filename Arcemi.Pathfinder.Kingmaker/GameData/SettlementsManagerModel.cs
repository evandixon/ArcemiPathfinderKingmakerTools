using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class SettlementsManagerModel : RefModel
    {
        public SettlementsManagerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<SettlementStateModel> SettlementStates => A.List("m_SettlementStates", a => new SettlementStateModel(a));
    }
}