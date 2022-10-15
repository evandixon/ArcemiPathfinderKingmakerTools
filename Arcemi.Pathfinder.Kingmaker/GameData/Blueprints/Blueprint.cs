using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    /// <summary>
    /// A model for a JBP blueprint file 
    /// </summary>
    public class Blueprint : Model
    {
        public Blueprint(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string AssetId { get => A.Value<string>("AssetId"); set => A.Value(value); }

        public BlueprintData Data => A.Object(factory: BlueprintData.Factory);
    }

    public class Blueprint<TBlueprintData> : Blueprint where TBlueprintData : BlueprintData
    {
        public Blueprint(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public Blueprint(Blueprint blueprint) : base(blueprint.GetAccessor())
        {
        }

        public new TBlueprintData Data => A.Object(factory: a => (TBlueprintData)BlueprintData.Factory(a));
    }
}
