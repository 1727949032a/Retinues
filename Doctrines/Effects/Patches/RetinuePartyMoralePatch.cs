using System;
using HarmonyLib;
using Retinues.Doctrines.Catalog;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace Retinues.Doctrines.Effects.Patches
{
	// Token: 0x020000DB RID: 219
	[HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetEffectivePartyMorale")]
	internal static class RetinuePartyMoralePatch
	{
		// Token: 0x060008EE RID: 2286 RVA: 0x0002CA08 File Offset: 0x0002AC08
		[SafeMethod(null, true, null)]
		private static void Postfix(MobileParty mobileParty, ref ExplainedNumber __result)
		{
			if (!DoctrineAPI.IsDoctrineUnlocked<BoundByHonor>())
			{
				return;
			}
			if (!mobileParty.IsMainParty)
			{
				return;
			}
			WRoster memberRoster = new WParty(mobileParty).MemberRoster;
			if (memberRoster == null)
			{
				return;
			}
			float num = Math.Max(0f, __result.ResultNumber) * (memberRoster.RetinueRatio * 0.2f);
			if (num <= 0f)
			{
				return;
			}
			__result.Add(num, L.T("retinue_morale_bonus_bound_by_honor", "Retinue (Bound by Honor)"), null);
		}
	}
}
