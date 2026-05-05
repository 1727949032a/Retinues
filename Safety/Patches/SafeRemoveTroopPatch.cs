using System;
using HarmonyLib;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace Retinues.Safety.Patches
{
	// Token: 0x0200004A RID: 74
	[HarmonyPatch(typeof(TroopRoster), "RemoveTroop")]
	internal static class SafeRemoveTroopPatch
	{
		// Token: 0x06000169 RID: 361 RVA: 0x0000A548 File Offset: 0x00008748
		private static bool Prefix(TroopRoster __instance, CharacterObject troop, int numberToRemove, UniqueTroopDescriptor troopSeed, int xp)
		{
			bool result;
			try
			{
				if (__instance.FindIndexOfTroop(troop) < 0)
				{
					Log.Error("Tried to remove " + (((troop != null) ? troop.StringId : null) ?? "NULL") + " not in roster. ");
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Prefix failed", null);
				result = false;
			}
			return result;
		}
	}
}
