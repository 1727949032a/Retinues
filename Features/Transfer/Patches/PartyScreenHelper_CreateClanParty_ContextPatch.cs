using System;
using HarmonyLib;
using Helpers;

namespace Retinues.Features.Transfer.Patches
{
	// Token: 0x020000B4 RID: 180
	[HarmonyPatch(typeof(PartyScreenHelper))]
	internal static class PartyScreenHelper_CreateClanParty_ContextPatch
	{
		// Token: 0x06000774 RID: 1908 RVA: 0x000265EC File Offset: 0x000247EC
		[HarmonyPrefix]
		[HarmonyPatch("OpenScreenAsCreateClanPartyForHero")]
		private static void Prefix_OpenScreenAsCreateClanPartyForHero()
		{
			PartyScreenContext.IsCreateClanPartyScreenActive = true;
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x000265F4 File Offset: 0x000247F4
		[HarmonyPostfix]
		[HarmonyPatch("ClosePartyPresentation")]
		private static void Postfix_ClosePartyPresentation()
		{
			PartyScreenContext.IsCreateClanPartyScreenActive = false;
		}
	}
}
