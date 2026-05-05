using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Configuration;
using Retinues.Game;
using Retinues.Troops.Save;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Retinues.Safety.Legacy
{
	// Token: 0x02000054 RID: 84
	[SafeClass]
	public static class LegacyTroopSaveConverter
	{
		// Token: 0x06000187 RID: 391 RVA: 0x0000B06C File Offset: 0x0000926C
		[return: TupleElementNames(new string[]
		{
			"clan",
			"kingdom"
		})]
		public static ValueTuple<FactionSaveData, FactionSaveData> ConvertLegacyFactionData(List<LegacyTroopSaveData> roots)
		{
			Log.Info(string.Format("{0} legacy root troops found, migrating.", roots.Count));
			FactionSaveData factionSaveData = new FactionSaveData();
			FactionSaveData factionSaveData2 = new FactionSaveData();
			foreach (LegacyTroopSaveData legacyTroopSaveData in roots)
			{
				TroopSaveData troopSaveData = LegacyTroopSaveConverter.ConvertLegacyTroopData(legacyTroopSaveData);
				FactionSaveData factionSaveData3 = LegacyTroopSaveConverter.IsKingdom(legacyTroopSaveData.StringId) ? factionSaveData2 : factionSaveData;
				switch (LegacyTroopSaveConverter.GetCategory(legacyTroopSaveData.StringId))
				{
				case RootCategory.RetinueBasic:
					factionSaveData3.RetinueBasic = troopSaveData;
					break;
				case RootCategory.RetinueElite:
					factionSaveData3.RetinueElite = troopSaveData;
					break;
				case RootCategory.RootBasic:
					factionSaveData3.RootBasic = troopSaveData;
					break;
				case RootCategory.RootElite:
					factionSaveData3.RootElite = troopSaveData;
					break;
				case RootCategory.MilitiaMelee:
					factionSaveData3.MilitiaMelee = troopSaveData;
					break;
				case RootCategory.MilitiaMeleeElite:
					factionSaveData3.MilitiaMeleeElite = troopSaveData;
					break;
				case RootCategory.MilitiaRanged:
					factionSaveData3.MilitiaRanged = troopSaveData;
					break;
				case RootCategory.MilitiaRangedElite:
					factionSaveData3.MilitiaRangedElite = troopSaveData;
					break;
				case RootCategory.CaravanGuard:
					factionSaveData3.CaravanGuard = troopSaveData;
					break;
				case RootCategory.CaravanMaster:
					factionSaveData3.CaravanMaster = troopSaveData;
					break;
				case RootCategory.Villager:
					factionSaveData3.Villager = troopSaveData;
					break;
				default:
					Log.Warn("Legacy troop '" + legacyTroopSaveData.StringId + "' has unrecognized category, skipping.");
					break;
				}
			}
			return new ValueTuple<FactionSaveData, FactionSaveData>(factionSaveData, factionSaveData2);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000B1E8 File Offset: 0x000093E8
		public static TroopSaveData ConvertLegacyTroopData(LegacyTroopSaveData data)
		{
			TroopSaveData troopSaveData = new TroopSaveData();
			troopSaveData.StringId = data.StringId;
			troopSaveData.VanillaStringId = data.VanillaStringId;
			troopSaveData.Name = data.Name;
			troopSaveData.Level = data.Level;
			troopSaveData.IsFemale = data.IsFemale;
			troopSaveData.CultureId = data.CultureId;
			troopSaveData.Race = data.Race;
			IEnumerable<LegacyTroopSaveData> upgradeTargets = data.UpgradeTargets;
			Func<LegacyTroopSaveData, TroopSaveData> selector;
			if ((selector = LegacyTroopSaveConverter.<>O.<0>__ConvertLegacyTroopData) == null)
			{
				selector = (LegacyTroopSaveConverter.<>O.<0>__ConvertLegacyTroopData = new Func<LegacyTroopSaveData, TroopSaveData>(LegacyTroopSaveConverter.ConvertLegacyTroopData));
			}
			troopSaveData.UpgradeTargets = upgradeTargets.Select(selector).ToList<TroopSaveData>();
			troopSaveData.EquipmentData = LegacyTroopSaveConverter.ConvertEquipmentData(data);
			troopSaveData.SkillData = new TroopSkillData(LegacyTroopSaveConverter.SkillsFromCode(data.SkillCode));
			troopSaveData.BodyData = (Config.EnableTroopCustomization ? LegacyTroopSaveConverter.ConvertBodyData(data) : null);
			return troopSaveData;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000B2BC File Offset: 0x000094BC
		private static TroopEquipmentData ConvertEquipmentData(LegacyTroopSaveData data)
		{
			string item = new Equipment().CalculateEquipmentCode();
			List<string> list;
			List<bool> list2;
			if (data.EquipmentCode != null)
			{
				list = new List<string>(2)
				{
					data.EquipmentCode,
					item
				};
				list2 = new List<bool>(2)
				{
					false,
					true
				};
			}
			else if (data.EquipmentCodes == null || data.EquipmentCodes.Count == 0)
			{
				list = new List<string>(2)
				{
					item,
					item
				};
				list2 = new List<bool>(2)
				{
					false,
					true
				};
			}
			else if (data.EquipmentCodes.Count == 1)
			{
				list = new List<string>(2)
				{
					data.EquipmentCodes[0],
					item
				};
				list2 = new List<bool>(2)
				{
					false,
					true
				};
			}
			else
			{
				list = data.EquipmentCodes;
				list2 = Enumerable.Repeat<bool>(false, list.Count).ToList<bool>();
				list2[1] = true;
			}
			return new TroopEquipmentData
			{
				Codes = list,
				Civilians = list2
			};
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000B3CC File Offset: 0x000095CC
		private static TroopBodySaveData ConvertBodyData(LegacyTroopSaveData data)
		{
			TroopBodySaveData troopBodySaveData = new TroopBodySaveData
			{
				AgeMin = data.AgeMin,
				AgeMax = data.AgeMax,
				WeightMin = data.WeightMin,
				WeightMax = data.WeightMax,
				BuildMin = data.BuildMin,
				BuildMax = data.BuildMax
			};
			if (data.HeightMin > 0f && data.HeightMax > 0f)
			{
				troopBodySaveData.HeightMin = data.HeightMin;
				troopBodySaveData.HeightMax = data.HeightMax;
			}
			return troopBodySaveData;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000B45A File Offset: 0x0000965A
		private static bool IsElite(string id)
		{
			return id != null && id.Contains("_elite_");
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000B46C File Offset: 0x0000966C
		private static bool IsKingdom(string id)
		{
			return id != null && id.Contains("_kingdom_");
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000B480 File Offset: 0x00009680
		private static RootCategory GetCategory(string id)
		{
			if (id == null)
			{
				return RootCategory.Other;
			}
			if (id.EndsWith("_retinue"))
			{
				if (!LegacyTroopSaveConverter.IsElite(id))
				{
					return RootCategory.RetinueBasic;
				}
				return RootCategory.RetinueElite;
			}
			else if (id.EndsWith("_mmilitia"))
			{
				if (!LegacyTroopSaveConverter.IsElite(id))
				{
					return RootCategory.MilitiaMelee;
				}
				return RootCategory.MilitiaMeleeElite;
			}
			else if (id.EndsWith("_rmilitia"))
			{
				if (!LegacyTroopSaveConverter.IsElite(id))
				{
					return RootCategory.MilitiaRanged;
				}
				return RootCategory.MilitiaRangedElite;
			}
			else
			{
				if (!LegacyTroopSaveConverter.IsElite(id))
				{
					return RootCategory.RootBasic;
				}
				return RootCategory.RootElite;
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000B4EC File Offset: 0x000096EC
		public static Dictionary<SkillObject, int> SkillsFromCode(string skillsString)
		{
			Dictionary<SkillObject, int> dictionary = new Dictionary<SkillObject, int>();
			if (string.IsNullOrWhiteSpace(skillsString))
			{
				return dictionary;
			}
			foreach (KeyValuePair<string, int> keyValuePair in (from part in skillsString.Split(new char[]
			{
				';'
			})
			select part.Split(new char[]
			{
				':'
			}) into parts
			where parts.Length == 2
			select parts).ToDictionary((string[] parts) => parts[0], (string[] parts) => int.Parse(parts[1])))
			{
				SkillObject @object = MBObjectManager.Instance.GetObject<SkillObject>(keyValuePair.Key);
				if (@object != null)
				{
					dictionary[@object] = keyValuePair.Value;
				}
			}
			return dictionary;
		}

		// Token: 0x02000129 RID: 297
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000358 RID: 856
			public static Func<LegacyTroopSaveData, TroopSaveData> <0>__ConvertLegacyTroopData;
		}
	}
}
