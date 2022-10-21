using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class RuntimeComponentDataModel : RefModel
    {
        public RuntimeComponentDataModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public FactItemModel AddedFact => A.Object(factory: FactItemModel.Factory);

        public T NewAddedFact<T>()
            where T : FactItemModel
        {
            return (T)A.NewObject(nameof(AddedFact), FactItemModel.GetPreparation<T>(), FactItemModel.Factory);
        }
    }
}