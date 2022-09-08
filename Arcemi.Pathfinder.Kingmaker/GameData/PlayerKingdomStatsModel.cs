﻿#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class PlayerKingdomStatsModel : RefModel
    {
        public PlayerKingdomStatsModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public IReadOnlyList<PlayerKingdomStatsAttributeModel> Attributes => A.List("m_Stats", a => new PlayerKingdomStatsAttributeModel(a));
    }
}
