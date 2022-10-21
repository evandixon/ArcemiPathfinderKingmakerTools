using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class QuestBookModel : RefModel
    {
        public QuestBookModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public PartsContainerModel Parts => A.Object(factory: a => new PartsContainerModel(a));
        public FactsContainerModel Facts => A.Object(factory: a => new FactsContainerModel(a));
    }
}