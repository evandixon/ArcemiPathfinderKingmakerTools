using Arcemi.Pathfinder.Kingmaker.Infrastructure;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Blueprints
{
    public class BlueprintSpellbook : BlueprintData
    {

        public const string TypeRef = "a42bdead3c333d744a3f0354bbe6c27c, BlueprintSpellbook";

        public BlueprintSpellbook(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public bool IsMythic => A.Value<bool>();
        public BlueprintReference<BlueprintSpellsTable> m_SpellsPerDay => A.Value<BlueprintReference<BlueprintSpellsTable>>();
        public BlueprintReference<BlueprintSpellsTable> m_SpellsKnown => A.Value<BlueprintReference<BlueprintSpellsTable>>();
        public BlueprintReference<BlueprintSpellsTable> m_SpellSlots => A.Value<BlueprintReference<BlueprintSpellsTable>>();
        public BlueprintReference m_SpellList => A.Value<BlueprintReference>();
        public BlueprintReference m_MythicSpellList => A.Value<BlueprintReference>();
        public BlueprintReference<BlueprintCharacterClass> m_CharacterClass => A.Value<BlueprintReference<BlueprintCharacterClass>>();
        public string CastingAttribute => A.Value<string>();
        public bool Spontaneous => A.Value<bool>();
        public int SpellsPerLevel => A.Value<int>();
        public bool AllSpellsKnown => A.Value<bool>();
        public string CantripsType => A.Value<string>();
        public int CasterLevelModifier => A.Value<int>();
        public bool CanCopyScrolls => A.Value<bool>();
        public bool IsArcane => A.Value<bool>();
        public bool IsArcanist => A.Value<bool>();
        public bool HasSpecialSpellList => A.Value<bool>();
    }
}
