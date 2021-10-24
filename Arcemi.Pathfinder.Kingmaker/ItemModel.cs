﻿#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class WeaponItemModel : ItemModel, IEnchantmentLevelItemModel
    {
        public const string TypeRef = "Kingmaker.Items.ItemEntityWeapon, Assembly-CSharp";
        public WeaponItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int EnchantmentLevel
        {
            get => Enchantments.WeaponLevel.GetLevelFrom(Facts.Items);
            set => Enchantments.WeaponLevel.SetLevelOn(Facts.Items, value);
        }

        public int MaxEnchantmentLevel => Enchantments.WeaponLevel.Levels.Count;
    }

    public class UsableItemModel : ItemModel
    {
        public const string TypeRef = "Kingmaker.Items.ItemEntityUsable, Assembly-CSharp";
        public UsableItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
    }

    public class ShieldItemModel : ItemModel, IEnchantmentLevelItemModel
    {
        public const string TypeRef = "Kingmaker.Items.ItemEntityShield, Assembly-CSharp";
        public ShieldItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ArmorComponentModel ArmorComponent => A.Object(factory: a => new ArmorComponentModel(a));

        public int EnchantmentLevel
        {
            get => Enchantments.ShieldLevel.GetLevelFrom(ArmorComponent.Facts.Items);
            set => Enchantments.ShieldLevel.SetLevelOn(ArmorComponent.Facts.Items, value);
        }

        public int MaxEnchantmentLevel => Enchantments.ArmorLevel.Levels.Count;
    }

    public class ArmorItemModel : ItemModel, IEnchantmentLevelItemModel
    {
        public const string TypeRef = "Kingmaker.Items.ItemEntityArmor, Assembly-CSharp";
        public ArmorItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int EnchantmentLevel
        {
            get => Enchantments.ArmorLevel.GetLevelFrom(Facts.Items);
            set => Enchantments.ArmorLevel.SetLevelOn(Facts.Items, value);
        }

        public int MaxEnchantmentLevel => Enchantments.ArmorLevel.Levels.Count;
    }

    public class SimpleItemModel : ItemModel
    {
        public const string TypeRef = "Kingmaker.Items.ItemEntitySimple, Assembly-CSharp";
        public SimpleItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
    }

    public class ItemModel : RefModel
    {
        public ItemModel(ModelDataAccessor accessor) : base(accessor)
        {
            N.On(nameof(Blueprint), nameof(RawData));
        }

        public RawItemData RawData => Mappings.RawItems.GetByBlueprint(Blueprint);
        public DescriptiveItemData DescriptiveData => Mappings.DescriptiveItems.GetByBlueprint(Blueprint);

        public string DisplayName => (DescriptiveData?.Name)
            .OrIfEmpty(A.Res.GetItemName(Blueprint))
            .OrIfEmpty(RawData?.Name.AsDisplayable())
            .OrIfEmpty(Blueprint);
        public string DisplayType => (DescriptiveData?.SubType)
            .OrIfEmpty(DescriptiveData?.Type)
            .OrIfEmpty(ItemType?.AsDisplayable())
            .OrIfEmpty(RawData?.TypeName)
            .OrIfEmpty(null);

        public string DisplayDescription => (DescriptiveData?.Description).OrIfEmpty(null);

        public bool IsStackable => true;
        public bool IsChargable => ItemType == Kingmaker.ItemType.Usable;

        public ItemType? ItemType
        {
            get {
                switch (Type) {
                    case WeaponItemModel.TypeRef: return Kingmaker.ItemType.Weapon;
                    case ArmorItemModel.TypeRef: return Kingmaker.ItemType.Armor;
                    case ShieldItemModel.TypeRef: return Kingmaker.ItemType.Shield;
                    case UsableItemModel.TypeRef: return Kingmaker.ItemType.Usable;
                    case SimpleItemModel.TypeRef: return Kingmaker.ItemType.Simple;
                }
                return null;
            }
        }
        public PartsContainerModel Parts => A.Object(factory: a => new PartsContainerModel(a), createIfNull: true);
        public FactsContainerModel Facts => A.Object(factory: a => new FactsContainerModel(a), createIfNull: true);
        public string Type { get => A.Value<string>("$type"); }
        public string Blueprint { get => A.Value<string>("m_Blueprint"); set => A.Value(value, "m_Blueprint"); }
        public int Count { get => A.Value<int?>("m_Count") ?? 1; set => A.Value(value, "m_Count"); }
        public int InventorySlotIndex { get => A.Value<int>("m_InventorySlotIndex"); set => A.Value(value, "m_InventorySlotIndex"); }
        public TimeSpan Time { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public int Charges { get => A.Value<int?>() ?? 1; set => A.Value(value); }
        public bool IsIdentified { get => A.Value<bool>(); set => A.Value(value); }
        //public TimeSpan? SellTime { get => A.Value<TimeSpan?>(); set => A.Value(value); }
        public bool IsNonRemovable { get => A.Value<bool>(); set => A.Value(value); }
        public InventoryModel Collection => A.Object<InventoryModel>();
        //public object Ability => A.Object();
        //public object ActivatableAbility => A.Object();
        //public object Enchantments => A.Object("m_Enchantments");
        public string WielderRef => A.Value<string>("m_WielderRef");
        public HoldingSlotModel HoldingSlot => A.Object(factory: a => new HoldingSlotModel(a));

        public CraftedPartItemModel SetAsCrafted()
        {
            return (CraftedPartItemModel)Parts.Items.Add(CraftedPartItemModel.Prepare);
        }

        public static ItemModel Create(ModelDataAccessor accessor)
        {
            var type = accessor.Value<string>("$type", "$type");
            switch (type) {
                case WeaponItemModel.TypeRef: return new WeaponItemModel(accessor);
                case ArmorItemModel.TypeRef: return new ArmorItemModel(accessor);
                case ShieldItemModel.TypeRef: return new ShieldItemModel(accessor);
                case UsableItemModel.TypeRef: return new UsableItemModel(accessor);
                case SimpleItemModel.TypeRef: return new SimpleItemModel(accessor);
            }
            return new ItemModel(accessor);
        }

        private static void AddRequiredItemProperties(JObject jObj, ItemType itemType)
        {
            switch (itemType) {
                case Kingmaker.ItemType.Weapon:
                    jObj.Add("$type", WeaponItemModel.TypeRef);
                    break;
                case Kingmaker.ItemType.Armor:
                    jObj.Add("$type", ArmorItemModel.TypeRef);
                    break;
                case Kingmaker.ItemType.Shield:
                    jObj.Add("$type", ShieldItemModel.TypeRef);
                    break;
                case Kingmaker.ItemType.Usable:
                    jObj.Add("$type", UsableItemModel.TypeRef);
                    jObj.Add("Charges", 1);
                    break;
                default:
                    jObj.Add("$type", SimpleItemModel.TypeRef);
                    break;
            }
            jObj.Add("Time", TimeSpan.Zero);
            jObj.Add("IsIdentified", true);
            jObj.Add("UniqueId", Guid.NewGuid().ToString());
        }

        public static void Duplicate(IReferences refs, JObject jObj, ItemModel item)
        {
            // $type is a reserved value and used by the serializer, it must be the second after the $id field.
            jObj.Add("$type", item.Type);
            jObj.Add("UniqueId", Guid.NewGuid().ToString());
            jObj.Add("Collection", refs.CreateReference(item.Collection.Id));
            item.A.ShallowMerge(jObj);
            jObj.Remove("m_WielderRef");
        }

        public static void Prepare(InventoryModel inventory, IReferences refs, JObject jObj, ItemType itemType)
        {
            AddRequiredItemProperties(jObj, itemType);
            jObj.Add("Collection", refs.CreateReference(inventory.Id));

            //var addArmorComponent = itemType == ItemType.Shield;
            //if (addArmorComponent) {
            //    var component = refs.Create();
            //    AddDefaultItemProperties(component);
            //    component.Add("m_ModifierDescriptor", "Shield");
            //    component.Add("m_Modifiers", null);
            //    component.Add("m_DexBonusLimeterAC", null);
            //    component.Add("m_InventorySlotIndex", -1);
            //    component.Add("Collection", null);
            //    component.Add("Charges", 0);
            //    if (rawData != null && rawData.TryGetComponent(ItemType.Armor, out var item)) {
            //        component.Add("m_Blueprint", item.Blueprint);
            //    }
            //    jObj.Add("ArmorComponent", component);
            //}

            //var addWeaponComponent = itemType == ItemType.Weapon;
            //if (addWeaponComponent) {
            //    var component = refs.Create();
            //    AddDefaultItemProperties(component);
            //    component.Add("Second", null);
            //    component.Add("ForceSecondary", false);
            //    component.Add("IsSecondPartOfDoubleWeapon", false);
            //    component.Add("IsShield", true);
            //    component.Add("Collection", null);
            //    component.Add("Charges", 0);
            //    jObj.Add("WeaponComponent", component);
            //}
        }
    }
}