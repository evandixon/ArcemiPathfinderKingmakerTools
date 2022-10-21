using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcemi.Pathfinder.Kingmaker.GameData.Enums
{
	public enum StatType
	{
		Unknown = 0,
		HitPoints = 10,
		TemporaryHitPoints = 22,
		Strength = 1,
		Dexterity = 2,
		Constitution = 3,
		Intelligence = 4,
		Wisdom = 5,
		Charisma = 6,
		BaseAttackBonus = 7,
		AdditionalAttackBonus = 8,
		AdditionalDamage = 9,
		AttackOfOpportunityCount = 18,
		AC = 11,
		AdditionalCMB = 12,
		AdditionalCMD = 13,
		SaveFortitude = 14,
		SaveWill = 0xF,
		SaveReflex = 0x10,
		SkillMobility = 17,
		SkillAthletics = 19,
		SkillPersuasion = 29,
		SkillThievery = 27,
		SkillLoreNature = 35,
		SkillPerception = 20,
		SkillStealth = 42,
		SkillUseMagicDevice = 43,
		SkillLoreReligion = 45,
		SkillKnowledgeWorld = 48,
		SkillKnowledgeArcana = 24,
		CheckBluff = 101,
		CheckDiplomacy = 102,
		CheckIntimidate = 103,
		Initiative = 26,
		Speed = 28,
		SneakAttack = 21,
		Reach = 23,
		DamageNonLethal = 104,
		BonusCasterLevel = 105
	}
}
