using System;
using HarmonyLib;
using Retinues.Game.Wrappers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;

namespace Retinues.Features.Swaps.Patches
{
	// Token: 0x020000BA RID: 186
	[HarmonyPatch(typeof(VillagerPartyComponent), "CreateVillagerParty")]
	internal static class VillagerSwap_Initialize_Postfix
	{
		// Token: 0x06000782 RID: 1922 RVA: 0x00026A54 File Offset: 0x00024C54
		private static void Postfix(VillagerPartyComponent __instance, MobileParty __result)
		{
			if (__result == null)
			{
				return;
			}
			WParty wparty = new WParty(__result);
			if (wparty.PlayerFaction == null)
			{
				return;
			}
			WRoster memberRoster = wparty.MemberRoster;
			if (memberRoster == null)
			{
				return;
			}
			memberRoster.SwapTroops(null, true);
		}
	}
}
