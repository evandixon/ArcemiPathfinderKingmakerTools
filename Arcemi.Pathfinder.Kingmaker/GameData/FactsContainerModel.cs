using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class FactsContainerModel : RefModel
    {
        public FactsContainerModel(ModelDataAccessor accessor) : base(accessor) { }

        public ListAccessor<FactItemModel> Items => A.List("m_Facts", FactItemModel.Factory, createIfNull: true);
    }
}