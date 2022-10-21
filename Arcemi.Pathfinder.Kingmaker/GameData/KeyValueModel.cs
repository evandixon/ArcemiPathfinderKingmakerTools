using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class PlayerKingdomLeaderSpecificBonusModel : Model
    {
        public PlayerKingdomLeaderSpecificBonusModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Key { get => A.Value<string>(); set => A.Value(value); }
        public int Value { get => A.Value<int>(); set => A.Value(value); }
    }
}