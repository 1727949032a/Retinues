using System;
using HarmonyLib;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Retinues.Safety.Patches
{
	// Token: 0x0200004B RID: 75
	[HarmonyPatch(typeof(TeleportHeroAction), "ApplyInternal")]
	internal static class TeleportHero_PreparePatch
	{
		// Token: 0x0600016A RID: 362 RVA: 0x0000A5B0 File Offset: 0x000087B0
		private static void Prefix(Hero hero, Settlement targetSettlement, MobileParty targetParty)
		{
			try
			{
				MobileParty mobileParty = (hero != null) ? hero.PartyBelongedTo : null;
				if (((mobileParty != null) ? mobileParty.MemberRoster : null) != null && mobileParty.MemberRoster.FindIndexOfTroop(hero.CharacterObject) < 0)
				{
					mobileParty.MemberRoster.AddToCounts(hero.CharacterObject, 1, true, 0, 0, true, -1);
					Log.Warn(string.Format("Re-added {0} to {1} before teleport.", (hero != null) ? hero.Name : null, (mobileParty != null) ? mobileParty.Name : null));
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Prefix failed", null);
			}
		}
	}
}
