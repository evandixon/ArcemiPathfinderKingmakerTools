﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Player
{
    public class PlayerKingdomLeaderSelectionModel : RefModel
    {

        public PlayerKingdomLeaderSelectionModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Blueprint { get => A.Value<string>("m_Blueprint"); set => A.Value(value, "m_Blueprint"); }
    }
}