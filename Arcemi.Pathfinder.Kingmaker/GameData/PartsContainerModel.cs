using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class PartsContainerModel : RefModel
    {
        public PartsContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<PartItemModel> Items => A.List("m_Parts", PartItemModel.Factory, createIfNull: true);
    }
}