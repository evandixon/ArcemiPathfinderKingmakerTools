using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class CharacterResourceContainerModel : RefModel
    {
        public CharacterResourceContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<AbilityResourceModel> PersistantResources => A.List(factory: a => new AbilityResourceModel(a));
    }
}