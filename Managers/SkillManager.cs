using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Experience;
using Retinues.Features.Staging;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Retinues.Managers
{
	// Token: 0x0200005F RID: 95
	[SafeClass]
	public static class SkillManager
	{
		// Token: 0x060001DD RID: 477 RVA: 0x0000DD14 File Offset: 0x0000BF14
		public static int SkillCapByTier(WCharacter troop)
		{
			if (troop == null)
			{
				return 0;
			}
			if (troop.IsHero)
			{
				return Config.SkillCapHeroes;
			}
			Option<int> o;
			switch (troop.Tier)
			{
			case 0:
				o = Config.SkillCapTier0;
				break;
			case 1:
				o = Config.SkillCapTier1;
				break;
			case 2:
				o = Config.SkillCapTier2;
				break;
			case 3:
				o = Config.SkillCapTier3;
				break;
			case 4:
				o = Config.SkillCapTier4;
				break;
			case 5:
				o = Config.SkillCapTier5;
				break;
			case 6:
				o = Config.SkillCapTier6;
				break;
			default:
				o = Config.SkillCapTier7Plus;
				break;
			}
			int num = o;
			if (troop.IsRetinue)
			{
				num += Config.RetinueSkillCapBonus;
			}
			if (DoctrineAPI.IsDoctrineUnlocked<IronDiscipline>())
			{
				num += 5;
			}
			if (troop.IsMaxTier && troop.IsCaptain)
			{
				num += 25;
			}
			return num;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000DDE8 File Offset: 0x0000BFE8
		public static int SkillTotalByTier(WCharacter troop)
		{
			if (troop == null)
			{
				return 0;
			}
			Option<int> o;
			switch (troop.Tier)
			{
			case 0:
				o = Config.SkillTotalTier0;
				break;
			case 1:
				o = Config.SkillTotalTier1;
				break;
			case 2:
				o = Config.SkillTotalTier2;
				break;
			case 3:
				o = Config.SkillTotalTier3;
				break;
			case 4:
				o = Config.SkillTotalTier4;
				break;
			case 5:
				o = Config.SkillTotalTier5;
				break;
			case 6:
				o = Config.SkillTotalTier6;
				break;
			default:
				o = Config.SkillTotalTier7Plus;
				break;
			}
			int num = o;
			if (troop.IsRetinue)
			{
				num += Config.RetinueSkillTotalBonus;
			}
			if (DoctrineAPI.IsDoctrineUnlocked<SteadfastSoldiers>())
			{
				num += 10;
			}
			if (troop.IsMaxTier && troop.IsCaptain)
			{
				num += 50;
			}
			return num;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000DEA7 File Offset: 0x0000C0A7
		public static int SkillPointsLeft(WCharacter troop)
		{
			if (troop == null)
			{
				return 0;
			}
			return SkillManager.SkillTotalByTier(troop) - troop.Skills.Values.Sum();
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000DECC File Offset: 0x0000C0CC
		public static int SkillPointXpCost(int fromValue)
		{
			if (ClanScreen.IsStudioMode)
			{
				return 0;
			}
			int num = Config.BaseSkillXpCost;
			int num2 = Config.SkillXpCostPerPoint;
			return num + num2 * fromValue;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000DEFC File Offset: 0x0000C0FC
		public static bool HasEnoughXpForNextPoint(WCharacter troop, SkillObject skill)
		{
			if (ClanScreen.IsStudioMode)
			{
				return true;
			}
			if (troop == null || skill == null)
			{
				return false;
			}
			PendingTrainData pendingTrainData = TrainStagingBehavior.Get(troop, skill);
			int num = (pendingTrainData != null) ? pendingTrainData.PointsRemaining : 0;
			int num2 = SkillManager.SkillPointXpCost(troop.GetSkill(skill) + num);
			return TroopXpBehavior.Get(troop) >= num2;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000DF4F File Offset: 0x0000C14F
		public static bool CanIncrementSkill(WCharacter troop, SkillObject skill)
		{
			return SkillManager.GetIncrementSkillReason(troop, skill) == null;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000DF60 File Offset: 0x0000C160
		public static TextObject GetIncrementSkillReason(WCharacter troop, SkillObject skill)
		{
			if (troop == null || skill == null)
			{
				return L.T("invalid_args", "Invalid arguments.");
			}
			int trueSkillValue = SkillManager.GetTrueSkillValue(troop, skill);
			List<PendingTrainData> list = TrainStagingBehavior.Get(troop);
			int num;
			if (list == null)
			{
				num = 0;
			}
			else
			{
				num = list.Sum((PendingTrainData d) => d.PointsRemaining);
			}
			int num2 = num;
			if (trueSkillValue >= SkillManager.SkillCapByTier(troop))
			{
				return L.T("skill_at_cap", "Skill is at cap for this troop.");
			}
			if (troop.IsHero)
			{
				return null;
			}
			if (SkillManager.SkillPointsLeft(troop) - num2 <= 0)
			{
				return L.T("no_skill_points_left", "No skill points left for this troop.");
			}
			if (!SkillManager.HasEnoughXpForNextPoint(troop, skill))
			{
				return L.T("not_enough_xp", "Not enough XP for next skill point.");
			}
			if (Config.CannotRaiseSkillAboveUpgradeLevel)
			{
				foreach (WCharacter wcharacter in troop.UpgradeTargets)
				{
					if (trueSkillValue >= SkillManager.GetTrueSkillValue(wcharacter, skill))
					{
						return L.T("cannot_exceed_child_skill", "Cannot exceed skill level of upgrade {CHILD}.").SetTextVariable("CHILD", wcharacter.Name);
					}
				}
			}
			return null;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000E06D File Offset: 0x0000C26D
		public static bool CanDecrementSkill(WCharacter troop, SkillObject skill)
		{
			return SkillManager.GetDecrementSkillReason(troop, skill) == null;
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000E07C File Offset: 0x0000C27C
		public static TextObject GetDecrementSkillReason(WCharacter troop, SkillObject skill)
		{
			if (troop == null || skill == null)
			{
				return L.T("invalid_args", "Invalid arguments.");
			}
			int trueSkillValue = SkillManager.GetTrueSkillValue(troop, skill);
			if (trueSkillValue <= 0)
			{
				return L.T("skill_zero", "Skill cannot go below zero.");
			}
			if (trueSkillValue <= troop.Loadout.ComputeSkillRequirement(skill))
			{
				return L.T("equipment_requirement", "Skill is required for equipped items.");
			}
			if (Config.CannotRaiseSkillAboveUpgradeLevel && troop.Parent != null && trueSkillValue <= SkillManager.GetTrueSkillValue(troop.Parent, skill))
			{
				return L.T("parent_skill", "Cannot go below parent {PARENT}'s skill level.").SetTextVariable("PARENT", troop.Parent.Name);
			}
			return null;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000E130 File Offset: 0x0000C330
		public static void ModifySkill(WCharacter troop, SkillObject skill, bool increment)
		{
			if (troop == null || skill == null)
			{
				return;
			}
			if (troop.IsHero)
			{
				int skill2 = troop.GetSkill(skill);
				int num = increment ? (skill2 + 1) : (skill2 - 1);
				if (num < 0)
				{
					num = 0;
				}
				troop.SetSkill(skill, num);
				return;
			}
			PendingTrainData pendingTrainData = TrainStagingBehavior.Get(troop, skill);
			int num2 = (pendingTrainData != null) ? pendingTrainData.PointsRemaining : 0;
			int num3 = troop.GetSkill(skill) + num2;
			if (!increment)
			{
				bool force = num2 > 0;
				TroopXpBehavior.RefundOnePoint(troop, num3, force);
				TrainStagingBehavior.ApplyChange(troop.StringId, skill, -1);
				return;
			}
			if (!TroopXpBehavior.TrySpend(troop, SkillManager.SkillPointXpCost(num3)))
			{
				return;
			}
			TrainStagingBehavior.Stage(troop, skill, 1);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000E1CA File Offset: 0x0000C3CA
		private static int GetTrueSkillValue(WCharacter troop, SkillObject skill)
		{
			int skill2 = troop.GetSkill(skill);
			PendingTrainData pendingTrainData = TrainStagingBehavior.Get(troop, skill);
			return skill2 + ((pendingTrainData != null) ? pendingTrainData.PointsRemaining : 0);
		}
	}
}
