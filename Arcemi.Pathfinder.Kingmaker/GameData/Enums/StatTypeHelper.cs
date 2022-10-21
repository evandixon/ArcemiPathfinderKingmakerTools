using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Enums
{
	public static class StatTypeHelper
	{
		public static StatType[] Skills = new[]
		{
			StatType.SkillMobility,
			StatType.SkillAthletics,
			StatType.SkillPersuasion,
			StatType.SkillThievery,
			StatType.SkillLoreNature,
			StatType.SkillPerception,
			StatType.SkillStealth,
			StatType.SkillUseMagicDevice,
			StatType.SkillLoreReligion,
			StatType.SkillKnowledgeWorld,
			StatType.SkillKnowledgeArcana
		};

		public static StatType[] Attributes = new[]
		{
			StatType.Strength,
			StatType.Dexterity,
			StatType.Constitution,
			StatType.Intelligence,
			StatType.Wisdom,
			StatType.Charisma
		};

		public static StatType[] Saves = new[]
		{
			StatType.SaveFortitude,
			StatType.SaveReflex,
			StatType.SaveWill
		};

		public static readonly StatType[] Knowledges = new[]
		{
			StatType.SkillLoreNature,
			StatType.SkillLoreReligion,
			StatType.SkillKnowledgeWorld,
			StatType.SkillKnowledgeArcana
		};

		public static bool IsAttribute(this StatType stat)
		{
			return Attributes.Contains(stat);
		}

		public static bool IsSkill(this StatType stat)
		{
			return Skills.Contains(stat);
		}

		public static bool IsKnowledge(this StatType stat)
		{
			return Knowledges.Contains(stat);
		}
	}
}
