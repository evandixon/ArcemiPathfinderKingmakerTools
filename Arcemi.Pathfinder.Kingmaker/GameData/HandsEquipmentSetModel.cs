﻿#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion


using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class HandsEquipmentSetModel : RefModel
    {
        public HandsEquipmentSetModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public HoldingSlotModel PrimaryHand => A.Object<HoldingSlotModel>();
        public HoldingSlotModel SecondaryHand => A.Object<HoldingSlotModel>();

    }
}