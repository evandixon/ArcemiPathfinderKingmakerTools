﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Player
{
    public class PlayerKingdomMoraleStateModel : RefModel
    {
        public PlayerKingdomMoraleStateModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int CurrentValue { get => A.Value<int>(); set => A.Value(value); }
        public int MinValue { get => A.Value<int>(); set => A.Value(value); }
        public int MaxValue { get => A.Value<int>(); set => A.Value(value); }
    }
}