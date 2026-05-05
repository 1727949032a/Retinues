using System;
using HarmonyLib;
using Retinues.Game.Wrappers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;

namespace Retinues.Features.Swaps.Patches
{
	// Token: 0x020000B8 RID: 184
	[HarmonyPatch(typeof(CaravanPartyComponent), "CreateCaravanParty")]
	internal static class CaravanSwap_Initialize_Postfix
	{
		// Token: 0x06000780 RID: 1920 RVA: 0x000269E0 File Offset: 0x00024BE0
		private static void Postfix(CaravanPartyComponent __instance, MobileParty __result)
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
			memberRoster.SwapTroopsPreservingHeroes(null);
		}
	}
}
