using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class VendorTablesModel : RefModel
    {
        public VendorTablesModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<VendorTableModel> PersistentTables => A.List("m_PersistentTables", a => new VendorTableModel(a));
    }
}