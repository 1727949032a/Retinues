using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Retinues.Features.Swaps.Patches
{
	// Token: 0x020000B7 RID: 183
	[HarmonyPatch(typeof(Settlement), "AddMilitiasToParty")]
	internal static class PlayerMilitiaSpawnPatch
	{
		// Token: 0x0600077B RID: 1915 RVA: 0x000267A8 File Offset: 0x000249A8
		private static bool IsValidChar(CharacterObject co)
		{
			return co != null && !co.IsHero && co.IsReady && co.IsInitialized;
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x000267C5 File Offset: 0x000249C5
		private static bool IsValid(WCharacter w)
		{
			return w != null && w.IsActive && !w.IsHero && w.Base != null && PlayerMilitiaSpawnPatch.IsValidChar(w.Base);
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x000267F8 File Offset: 0x000249F8
		private static void AddLaneSafe(Settlement s, MobileParty party, CharacterObject basic, CharacterObject elite, float ratio, ref int remaining)
		{
			if (PlayerMilitiaSpawnPatch.Helper_RefInt == null)
			{
				return;
			}
			if (remaining <= 0)
			{
				return;
			}
			if (!PlayerMilitiaSpawnPatch.IsValidChar(basic) && !PlayerMilitiaSpawnPatch.IsValidChar(elite))
			{
				return;
			}
			try
			{
				object[] array = new object[]
				{
					party,
					basic,
					elite,
					ratio,
					remaining
				};
				PlayerMilitiaSpawnPatch.Helper_RefInt.Invoke(s, array);
				remaining = (int)array[4];
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "AddLane failed (continuing with remaining lanes)", null);
			}
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x0002688C File Offset: 0x00024A8C
		private static bool Prefix(Settlement __instance, [HarmonyArgument(0)] MobileParty militaParty, [HarmonyArgument(1)] int militiaToAdd)
		{
			if (__instance == null || militaParty == null || militiaToAdd <= 0 || PlayerMilitiaSpawnPatch.Helper_RefInt == null)
			{
				return true;
			}
			bool result;
			try
			{
				WSettlement wsettlement = new WSettlement(__instance);
				if (wsettlement.PlayerFaction == null)
				{
					result = true;
				}
				else
				{
					WCharacter militiaMelee = wsettlement.MilitiaMelee;
					WCharacter militiaMeleeElite = wsettlement.MilitiaMeleeElite;
					WCharacter militiaRanged = wsettlement.MilitiaRanged;
					WCharacter militiaRangedElite = wsettlement.MilitiaRangedElite;
					if (!PlayerMilitiaSpawnPatch.IsValid(militiaMelee) || !PlayerMilitiaSpawnPatch.IsValid(militiaMeleeElite) || !PlayerMilitiaSpawnPatch.IsValid(militiaRanged) || !PlayerMilitiaSpawnPatch.IsValid(militiaRangedElite))
					{
						result = true;
					}
					else
					{
						float ratio;
						float num;
						Campaign.Current.Models.SettlementMilitiaModel.CalculateMilitiaSpawnRate(__instance, out ratio, out num);
						int num2 = militiaToAdd;
						PlayerMilitiaSpawnPatch.AddLaneSafe(__instance, militaParty, militiaMelee.Base, militiaMeleeElite.Base, ratio, ref num2);
						PlayerMilitiaSpawnPatch.AddLaneSafe(__instance, militaParty, militiaRanged.Base, militiaRangedElite.Base, 1f, ref num2);
						Log.Debug(string.Format("{0}: custom militia used (add={1}, rem={2}).", wsettlement.Name, militiaToAdd, num2));
						result = false;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Fall back to vanilla", null);
				result = true;
			}
			return result;
		}

		// Token: 0x040001F0 RID: 496
		private static readonly MethodInfo Helper_RefInt = typeof(Settlement).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(delegate(MethodInfo m)
		{
			if (m.Name != "AddTroopToMilitiaParty")
			{
				return false;
			}
			ParameterInfo[] parameters = m.GetParameters();
			return parameters.Length == 5 && parameters[0].ParameterType == typeof(MobileParty) && parameters[1].ParameterType == typeof(CharacterObject) && parameters[2].ParameterType == typeof(CharacterObject) && parameters[3].ParameterType == typeof(float) && parameters[4].ParameterType.IsByRef && parameters[4].ParameterType.GetElementType() == typeof(int);
		});
	}
}
