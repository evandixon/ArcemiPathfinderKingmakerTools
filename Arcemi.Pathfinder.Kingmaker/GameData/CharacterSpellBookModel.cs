﻿using Arcemi.Pathfinder.Kingmaker.Infrastructure;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker.GameData
{
    public class CharacterSpellbookModel : RefModel
    {
        public CharacterSpellbookModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public int BaseLevelInternal { get => A.Value<int>("m_BaseLevelInternal"); set => A.Value(value, "m_BaseLevelInternal"); }
        public int MythicLevelInternal { get => A.Value<int>("m_MythicLevelInternal"); set => A.Value(value, "m_MythicLevelInternal"); }
        public string Type { get => A.Value<string>("m_Type"); set => A.Value(value, "m_Type"); }
        public ListValueAccessor<int> SpontaneousSlots => A.ListValue<int>("m_SpontaneousSlots");
        public IEnumerable<SpellIndexAccessor> SpontaneousSlotsAccessors => SpontaneousSlots.Select((x, i) => new SpellIndexAccessor(i, SpontaneousSlots));
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string OppositionDescriptors { get => A.Value<string>(); set => A.Value(value); }
        public ListValueAccessor<int> BonusSpellSlots => A.ListValue<int>();
        public IEnumerable<SpellIndexAccessor> BonusSpellSlotsAccessors => BonusSpellSlots.Select((x, i) => new SpellIndexAccessor(i, BonusSpellSlots));
        public ListD2Accessor<LearnedSpellModel> KnownSpells => A.ListD2("m_KnownSpells", factory: a => new LearnedSpellModel(a));
        public ListD2Accessor<LearnedSpellModel> SpecialSpells => A.ListD2("m_SpecialSpells", factory: a => new LearnedSpellModel(a));
        public ListD2Accessor<CustomSpellModel> CustomSpells => A.ListD2("m_CustomSpells", factory: a => new CustomSpellModel(a));
        public ListD2Accessor<MemorizedSpellModel> MemorizedSpells => A.ListD2("m_MemorizedSpells", factory: a => new MemorizedSpellModel(a));
        public ListValueAccessor<string> SpecialLists => A.ListValue<string>("m_SpecialLists");
        public ListValueAccessor<string> OppositionSchools => A.ListValue<string>();

        public void EnableCustomSpells()
        {
            if (CustomSpells != null) return;

            var list = A.ListD2("m_CustomSpells", factory: a => new CustomSpellModel(a));
            list.EnsureRank1Count(11);
        }

        public static void Factory(IReferences refs, JObject obj)
        {
            var bookObj = refs.Create();
            obj.Add("Value", bookObj);
            bookObj.Add("m_KnownSpells", refs.NewArray(11, () => new JArray()));
            bookObj.Add("m_SpecialSpells", refs.NewArray(11, () => new JArray()));
            bookObj.Add("m_SpecialLists", new JArray());
            bookObj.Add("m_CustomSpells", refs.NewArray(11, () => new JArray()));
            bookObj.Add("m_MemorizedSpells", refs.NewArray(11, () => new JArray()));
            bookObj.Add("m_SpontaneousSlots", refs.NewArray(11, initialValue: 0));
            bookObj.Add("OppositionSchools", new JArray());
            bookObj.Add("ExOppositionSchools", new JArray());
            bookObj.Add("BonusSpellSlots", refs.NewArray(11, initialValue: 0));
        }
    }
}