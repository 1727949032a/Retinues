using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;

namespace Retinues.Troops
{
	// Token: 0x0200003A RID: 58
	[SafeClass]
	public static class TroopMatcher
	{
		// Token: 0x0600012A RID: 298 RVA: 0x00007F84 File Offset: 0x00006184
		private static WCharacter Pick(WCharacter troop, BaseFaction faction)
		{
			if (troop == null || faction == null)
			{
				return null;
			}
			WCharacter result;
			if (troop == troop.Faction.RetinueElite)
			{
				result = faction.RetinueElite;
			}
			else if (troop == troop.Faction.RetinueBasic)
			{
				result = faction.RetinueBasic;
			}
			else if (troop == troop.Faction.MilitiaMelee)
			{
				result = faction.MilitiaMelee;
			}
			else if (troop == troop.Faction.MilitiaMeleeElite)
			{
				result = faction.MilitiaMeleeElite;
			}
			else if (troop == troop.Faction.MilitiaRanged)
			{
				result = faction.MilitiaRanged;
			}
			else if (troop == troop.Faction.MilitiaRangedElite)
			{
				result = faction.MilitiaRangedElite;
			}
			else if (troop == troop.Faction.CaravanGuard)
			{
				result = faction.CaravanGuard;
			}
			else if (troop == troop.Faction.CaravanMaster)
			{
				result = faction.CaravanMaster;
			}
			else if (troop == troop.Faction.Villager)
			{
				result = faction.Villager;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000080B0 File Offset: 0x000062B0
		public static WCharacter PickBestFromFaction(BaseFaction faction, WCharacter troop, bool sameCategoryOnly = true, bool sameTierOnly = true, WCharacter fallback = null)
		{
			if (faction == null || troop == null || !troop.IsValid)
			{
				return null;
			}
			BaseFaction faction2 = troop.Faction;
			bool flag = troop == faction2.Villager || troop == faction2.CaravanGuard || troop == faction2.CaravanMaster || troop == faction2.MilitiaMelee || troop == faction2.MilitiaMeleeElite || troop == faction2.MilitiaRanged || troop == faction2.MilitiaRangedElite || troop == faction2.RetinueBasic || troop == faction2.RetinueElite;
			if (flag)
			{
				WCharacter wcharacter = TroopMatcher.Pick(troop, faction);
				if (TroopMatcher.<PickBestFromFaction>g__Valid|6_0(wcharacter))
				{
					return wcharacter;
				}
			}
			WCharacter rootBasic = faction2.RootBasic;
			bool flag2;
			if (rootBasic == null)
			{
				flag2 = false;
			}
			else
			{
				IEnumerable<WCharacter> tree = rootBasic.Tree;
				flag2 = ((tree != null) ? new bool?(tree.Contains(troop)) : null).GetValueOrDefault();
			}
			bool flag3;
			if (!flag2)
			{
				WCharacter rootElite = faction2.RootElite;
				if (rootElite == null)
				{
					flag3 = false;
				}
				else
				{
					IEnumerable<WCharacter> tree2 = rootElite.Tree;
					flag3 = ((tree2 != null) ? new bool?(tree2.Contains(troop)) : null).GetValueOrDefault();
				}
			}
			else
			{
				flag3 = true;
			}
			bool flag4 = flag3;
			if (sameCategoryOnly && !flag && !flag4)
			{
				return null;
			}
			if (flag4)
			{
				WCharacter wcharacter2 = troop.IsElite ? faction.RootElite : faction.RootBasic;
				if (wcharacter2 != null && wcharacter2.IsValid)
				{
					WCharacter wcharacter3 = TroopMatcher.PickBestFromTree(wcharacter2, troop, null, sameTierOnly);
					if (TroopMatcher.<PickBestFromFaction>g__Valid|6_0(wcharacter3))
					{
						return wcharacter3;
					}
				}
			}
			return fallback;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000823C File Offset: 0x0000643C
		public static WCharacter PickBestFromTree(WCharacter root, WCharacter troop, WCharacter exclude = null, bool sameTierOnly = true)
		{
			if (root == null || !root.IsValid || troop == null || !troop.IsValid)
			{
				return null;
			}
			IEnumerable<WCharacter> tree = root.Tree;
			List<WCharacter> list = ((tree != null) ? (from t in tree
			where t.IsValid && (exclude == null || t.StringId != exclude.StringId)
			select t).ToList<WCharacter>() : null) ?? new List<WCharacter>();
			if (list.Count == 0)
			{
				return null;
			}
			List<WCharacter> list2;
			if (sameTierOnly)
			{
				list2 = (from t in list
				where t.Tier == troop.Tier
				select t).ToList<WCharacter>();
			}
			else
			{
				int minDiff = list.Min((WCharacter t) => Math.Abs(t.Tier - troop.Tier));
				list2 = (from t in list
				where Math.Abs(t.Tier - troop.Tier) == minDiff
				select t).ToList<WCharacter>();
			}
			if (list2.Count == 0)
			{
				return null;
			}
			WCharacter wcharacter = null;
			int num = 0;
			foreach (WCharacter wcharacter2 in list2)
			{
				int num2 = TroopMatcher.EligibilityScore(wcharacter2, troop);
				if (wcharacter == null || num2 > num || (num2 == num && string.CompareOrdinal(wcharacter2.StringId, wcharacter.StringId) < 0))
				{
					wcharacter = wcharacter2;
					num = num2;
				}
			}
			if (wcharacter == null || !wcharacter.IsValid)
			{
				return null;
			}
			return wcharacter;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000083C0 File Offset: 0x000065C0
		private static int EligibilityScore(WCharacter troop, WCharacter retinue)
		{
			int num = 0;
			if (troop.IsMounted == retinue.IsMounted)
			{
				num += 1000000;
			}
			if (troop.IsRanged == retinue.IsRanged)
			{
				num += 100000;
			}
			if (troop.IsFemale == retinue.IsFemale)
			{
				num += 10000;
			}
			double num2 = Similarity.Jaccard(TroopMatcher.WeaponClasses(troop), TroopMatcher.WeaponClasses(retinue));
			num += (int)Math.Round(num2 * 1000.0 * 1.0);
			double num3 = Similarity.Cosine(TroopMatcher.Skills(troop), TroopMatcher.Skills(retinue));
			return num + (int)Math.Round(num3 * 1000.0 * 1.0);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00008474 File Offset: 0x00006674
		private static HashSet<string> WeaponClasses(WCharacter c)
		{
			HashSet<string> result;
			try
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				if (c == null)
				{
					result = hashSet;
				}
				else
				{
					foreach (EquipmentIndex slot in WEquipment.Slots)
					{
						try
						{
							WItem witem = c.Loadout.Battle.Get(slot);
							if (witem != null && witem.IsWeapon && !string.IsNullOrWhiteSpace(witem.Class))
							{
								hashSet.Add(witem.Class);
							}
						}
						catch
						{
						}
					}
					result = hashSet;
				}
			}
			catch
			{
				result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
			return result;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000854C File Offset: 0x0000674C
		private static Dictionary<string, int> Skills(WCharacter c)
		{
			Dictionary<string, int> result;
			try
			{
				Dictionary<SkillObject, int> dictionary = ((c != null) ? c.Skills : null) ?? new Dictionary<SkillObject, int>();
				Dictionary<string, int> dictionary2 = new Dictionary<string, int>(StringComparer.Ordinal);
				foreach (KeyValuePair<SkillObject, int> keyValuePair in dictionary)
				{
					SkillObject key = keyValuePair.Key;
					string text = (key != null) ? key.StringId : null;
					if (!string.IsNullOrEmpty(text))
					{
						dictionary2[text] = keyValuePair.Value;
					}
				}
				result = dictionary2;
			}
			catch
			{
				result = new Dictionary<string, int>(StringComparer.Ordinal);
			}
			return result;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00008600 File Offset: 0x00006800
		[CompilerGenerated]
		internal static bool <PickBestFromFaction>g__Valid|6_0(WCharacter x)
		{
			return x != null && x.IsValid && x.IsCustom;
		}

		// Token: 0x04000056 RID: 86
		private const int WEIGHT_MOUNTED = 1000000;

		// Token: 0x04000057 RID: 87
		private const int WEIGHT_RANGED = 100000;

		// Token: 0x04000058 RID: 88
		private const int WEIGHT_FEMALE = 10000;

		// Token: 0x04000059 RID: 89
		private const double WEIGHT_WEAP = 1.0;

		// Token: 0x0400005A RID: 90
		private const double WEIGHT_SKILL = 1.0;
	}
}
