using System;
using HarmonyLib;
using Retinues.Game.Wrappers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;

namespace Retinues.Features.Swaps.Patches
{
	// Token: 0x020000B9 RID: 185
	[HarmonyPatch(typeof(MilitiaPartyComponent), "CreateMilitiaParty")]
	internal static class MilitiaSwap_Initialize_Postfix
	{
		// Token: 0x06000781 RID: 1921 RVA: 0x00026A18 File Offset: 0x00024C18
		private static void Postfix(MilitiaPartyComponent __instance, MobileParty __result)
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
