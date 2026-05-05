using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;

namespace Retinues.Mods.BanditMilitias
{
	// Token: 0x0200005B RID: 91
	[SafeClass]
	internal static class BanditMilitiasTroopsPatcher
	{
		// Token: 0x060001AC RID: 428 RVA: 0x0000BD9C File Offset: 0x00009F9C
		public static void TryPatch(Harmony harmony)
		{
			try
			{
				Type type = AccessTools.TypeByName("BanditMilitias.Helper");
				Type type2 = AccessTools.TypeByName("BanditMilitias.Globals");
				if (type == null || type2 == null)
				{
					Log.Debug("BanditMilitias not found; skipping BanditMilitias compatibility patch.");
				}
				else
				{
					BanditMilitiasTroopsPatcher._recruitsField = AccessTools.Field(type2, "Recruits");
					BanditMilitiasTroopsPatcher._basicInfantryField = AccessTools.Field(type2, "BasicInfantry");
					BanditMilitiasTroopsPatcher._basicRangedField = AccessTools.Field(type2, "BasicRanged");
					BanditMilitiasTroopsPatcher._basicCavalryField = AccessTools.Field(type2, "BasicCavalry");
					MethodInfo methodInfo = AccessTools.Method(type, "InitMap", Type.EmptyTypes, null);
					if (methodInfo != null)
					{
						HarmonyMethod postfix = new HarmonyMethod(typeof(BanditMilitiasTroopsPatcher).GetMethod("InitMapPostfix", BindingFlags.Static | BindingFlags.NonPublic));
						harmony.Patch(methodInfo, null, postfix, null, null);
					}
					else
					{
						Log.Debug("BanditMilitias.Helper.InitMap not found; recruit pool filter skipped.");
					}
					MethodInfo methodInfo2 = AccessTools.Method(type, "InitMilitia", new Type[]
					{
						typeof(MobileParty),
						typeof(TroopRoster[]),
						typeof(Vec2)
					}, null);
					if (methodInfo2 != null)
					{
						HarmonyMethod prefix = new HarmonyMethod(typeof(BanditMilitiasTroopsPatcher).GetMethod("InitMilitiaPrefix", BindingFlags.Static | BindingFlags.NonPublic));
						harmony.Patch(methodInfo2, prefix, null, null, null);
					}
					else
					{
						Log.Debug("BanditMilitias.Helper.InitMilitia not found; militia roster filter skipped.");
					}
					Log.Info("BanditMilitias compatibility patch applied (InitMap/InitMilitia).");
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Failed to apply BanditMilitias compatibility patches.", null);
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000BF20 File Offset: 0x0000A120
		private static void InitMapPostfix()
		{
			try
			{
				BanditMilitiasTroopsPatcher.FilterRecruitPools();
				BanditMilitiasTroopsPatcher.FilterBasicBanditPools();
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "BanditMilitias.InitMapPostfix failed.", null);
			}
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000BF58 File Offset: 0x0000A158
		private static void FilterRecruitPools()
		{
			if (BanditMilitiasTroopsPatcher._recruitsField == null)
			{
				return;
			}
			IDictionary dictionary = BanditMilitiasTroopsPatcher._recruitsField.GetValue(null) as IDictionary;
			if (dictionary == null)
			{
				return;
			}
			int num = 0;
			foreach (object obj in dictionary)
			{
				IList list = ((DictionaryEntry)obj).Value as IList;
				if (list != null)
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						CharacterObject characterObject = list[i] as CharacterObject;
						if (characterObject != null && BanditMilitiasTroopsPatcher.IsCustom(characterObject))
						{
							list.RemoveAt(i);
							num++;
						}
					}
				}
			}
			if (num > 0)
			{
				Log.Debug(string.Format("[BanditMilitiasCompat] Removed {0} custom troops from Globals.Recruits.", num));
			}
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000C03C File Offset: 0x0000A23C
		private static void FilterBasicBanditPools()
		{
			BanditMilitiasTroopsPatcher.FilterBasicList(BanditMilitiasTroopsPatcher._basicInfantryField, "BasicInfantry");
			BanditMilitiasTroopsPatcher.FilterBasicList(BanditMilitiasTroopsPatcher._basicRangedField, "BasicRanged");
			BanditMilitiasTroopsPatcher.FilterBasicList(BanditMilitiasTroopsPatcher._basicCavalryField, "BasicCavalry");
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000C06C File Offset: 0x0000A26C
		private static void FilterBasicList(FieldInfo field, string name)
		{
			if (field == null)
			{
				return;
			}
			IList list = field.GetValue(null) as IList;
			if (list == null)
			{
				return;
			}
			int num = 0;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				CharacterObject characterObject = list[i] as CharacterObject;
				if (characterObject != null && BanditMilitiasTroopsPatcher.IsCustom(characterObject))
				{
					list.RemoveAt(i);
					num++;
				}
			}
			if (num > 0)
			{
				Log.Debug(string.Format("[BanditMilitiasCompat] Removed {0} custom troops from Globals.{1}.", num, name));
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000C0E8 File Offset: 0x0000A2E8
		private static void InitMilitiaPrefix(TroopRoster[] rosters)
		{
			try
			{
				if (rosters != null && rosters.Length != 0)
				{
					TroopRoster troopRoster = rosters[0];
					if (troopRoster != null)
					{
						BanditMilitiasTroopsPatcher.StripCustomFromRoster(troopRoster);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "BanditMilitias.InitMilitiaPrefix failed.", null);
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000C12C File Offset: 0x0000A32C
		private static void StripCustomFromRoster(TroopRoster roster)
		{
			try
			{
				for (int i = roster.Count - 1; i >= 0; i--)
				{
					TroopRosterElement elementCopyAtIndex = roster.GetElementCopyAtIndex(i);
					CharacterObject character = elementCopyAtIndex.Character;
					if (character != null && BanditMilitiasTroopsPatcher.IsCustom(character))
					{
						roster.AddToCounts(character, -elementCopyAtIndex.Number, false, -elementCopyAtIndex.WoundedNumber, 0, true, -1);
						Log.Debug("[BanditMilitiasCompat] Removed custom troop '" + character.StringId + "' from militia roster.");
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "StripCustomFromRoster failed.", null);
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000C1BC File Offset: 0x0000A3BC
		private static bool IsCustom(CharacterObject character)
		{
			if (character == null)
			{
				return false;
			}
			string stringId = character.StringId;
			return !string.IsNullOrEmpty(stringId) && (stringId.StartsWith("retinues_custom_", StringComparison.Ordinal) || stringId.StartsWith("ret_", StringComparison.Ordinal));
		}

		// Token: 0x040000A7 RID: 167
		private const string HelperTypeName = "BanditMilitias.Helper";

		// Token: 0x040000A8 RID: 168
		private const string GlobalsTypeName = "BanditMilitias.Globals";

		// Token: 0x040000A9 RID: 169
		private static FieldInfo _recruitsField;

		// Token: 0x040000AA RID: 170
		private static FieldInfo _basicInfantryField;

		// Token: 0x040000AB RID: 171
		private static FieldInfo _basicRangedField;

		// Token: 0x040000AC RID: 172
		private static FieldInfo _basicCavalryField;
	}
}
