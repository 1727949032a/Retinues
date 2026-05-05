using System;
using HarmonyLib;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace Retinues.Safety.Patches
{
	// Token: 0x02000048 RID: 72
	[HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetEffectivePartyMorale")]
	internal static class GetEffectivePartyMorale_FailSafe
	{
		// Token: 0x06000166 RID: 358 RVA: 0x0000A410 File Offset: 0x00008610
		[HarmonyFinalizer]
		private static void Finalizer(DefaultPartyMoraleModel __instance, MobileParty mobileParty, bool includeDescription, ref ExplainedNumber __result, Exception __exception)
		{
			if (__exception == null)
			{
				return;
			}
			try
			{
				Log.Exception(__exception, "", null);
				__result = new ExplainedNumber(50f, includeDescription, null);
				__result.Add(0f, new TextObject("Retinues: fail-safe morale", null), null);
			}
			catch (Exception)
			{
				__result = new ExplainedNumber(50f, includeDescription, null);
				Log.Exception(__exception, "Failed to apply fail-safe morale", null);
			}
		}
	}
}
