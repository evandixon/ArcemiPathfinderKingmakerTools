using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Player
{
    public class PlayerMainCharacterModel : Model
    {
        public PlayerMainCharacterModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string UniqueId => A.Value<string>("m_UniqueId");
    }
}