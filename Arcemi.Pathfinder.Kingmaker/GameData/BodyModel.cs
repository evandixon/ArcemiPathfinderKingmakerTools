﻿#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class BodyModel : RefModel
    {
        public BodyModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public bool HandsAreEnabled { get => A.Value<bool>(); set => A.Value(value); }
        public bool IsPolymorphed { get => A.Value<bool>(); set => A.Value(value); }
        public int CurrentHandsEquipmentSetIndex { get => A.Value<int>("m_CurrentHandsEquipmentSetIndex"); set => A.Value(value, "m_CurrentHandsEquipmentSetIndex"); }

        public ItemModel EmptyHandWeapon => A.Object<ItemModel>("m_EmptyHandWeapon");

        public IEnumerable<HoldingSlotEntry> All
        {
            get
            {
                var index = 1;
                foreach (var equipmentSet in HandsEquipmentSets)
                {
                    yield return new HoldingSlotEntry("Weapon Set Primary #" + index, equipmentSet.PrimaryHand);
                    yield return new HoldingSlotEntry("Weapon Set Secondary #" + index++, equipmentSet.SecondaryHand);
                }
                yield return new HoldingSlotEntry("Head", Head);
                yield return new HoldingSlotEntry("Neck", Neck);
                yield return new HoldingSlotEntry("Shoulders", Shoulders);
                yield return new HoldingSlotEntry("Armor", Armor);
                yield return new HoldingSlotEntry("Belt", Belt);
                yield return new HoldingSlotEntry("Wrist", Wrist);
                yield return new HoldingSlotEntry("Ring #1", Ring1);
                yield return new HoldingSlotEntry("Ring #2", Ring2);
                yield return new HoldingSlotEntry("Gloves", Gloves);
                yield return new HoldingSlotEntry("Feet", Feet);

                index = 1;
                foreach (var slot in QuickSlots)
                {
                    yield return new HoldingSlotEntry("Quick Slot #" + index++, slot);
                }
            }
        }

        public IReadOnlyList<HandsEquipmentSetModel> HandsEquipmentSets => A.List<HandsEquipmentSetModel>("m_HandsEquipmentSets");
        public IReadOnlyList<HoldingSlotModel> QuickSlots => A.List<HoldingSlotModel>("m_QuickSlots");

        public HoldingSlotModel Armor => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Belt => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Head => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Feet => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Gloves => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Neck => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Ring1 => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Ring2 => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Wrist => A.Object<HoldingSlotModel>();
        public HoldingSlotModel Shoulders => A.Object<HoldingSlotModel>();
    }
}