using System;
using HarmonyLib;
using Retinues.Configuration;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace Retinues.Features.Volunteers.Patches
{
	// Token: 0x020000AD RID: 173
	[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "OnTroopRecruited")]
	public static class VolunteerSwapForAI
	{
		// Token: 0x06000748 RID: 1864 RVA: 0x00024E88 File Offset: 0x00023088
		[SafeMethod(null, true, null)]
		private static void Postfix(Hero recruiter, Settlement settlement, Hero recruitmentSource, CharacterObject troop, int count)
		{
			if (settlement == null || recruiter == null || troop == null || count <= 0)
			{
				return;
			}
			if (recruiter.IsHumanPlayerCharacter)
			{
				return;
			}
			MobileParty partyBelongedTo = recruiter.PartyBelongedTo;
			if (partyBelongedTo == null)
			{
				return;
			}
			WSettlement wsettlement = new WSettlement(settlement);
			WCharacter wcharacter = new WCharacter(troop);
			if (!wcharacter.IsValid)
			{
				return;
			}
			bool flag = new WHero(recruiter).PlayerFaction != null;
			WFaction playerFaction = wsettlement.PlayerFaction;
			bool flag2 = playerFaction != null;
			WFaction wfaction = null;
			bool flag3 = false;
			if (flag2 && Config.AllLordsCanRecruitCustomTroops)
			{
				flag3 = true;
				wfaction = (playerFaction ?? Player.Clan);
			}
			if (!flag3 && flag2 && Config.VassalLordsCanRecruitCustomTroops && flag)
			{
				flag3 = true;
				wfaction = (playerFaction ?? Player.Clan);
			}
			if (!flag3 && Config.VassalLordsRecruitCustomTroopsAnywhere && flag)
			{
				flag3 = true;
				if (wfaction == null)
				{
					wfaction = (Player.Clan ?? playerFaction);
				}
			}
			if (!flag3 || wfaction == null)
			{
				if (!flag2 || !wcharacter.IsCustom)
				{
					return;
				}
				WCulture culture = wsettlement.Culture;
				if (culture == null)
				{
					return;
				}
				WCharacter wcharacter2 = wcharacter.IsElite ? culture.RootElite : culture.RootBasic;
				if (wcharacter2 == null)
				{
					return;
				}
				WCharacter wcharacter3 = TroopMatcher.PickBestFromTree(wcharacter2, wcharacter, null, false);
				if (wcharacter3 == null || wcharacter3 == wcharacter)
				{
					return;
				}
				TroopRoster memberRoster = partyBelongedTo.MemberRoster;
				memberRoster.RemoveTroop(wcharacter.Base, count, default(UniqueTroopDescriptor), 0);
				memberRoster.AddToCounts(wcharacter3.Base, count, false, 0, 0, true, -1);
				Log.Debug(string.Format("VolunteerSwapForAI: unauthorized {0} recruited {1}x {2} ", (recruiter != null) ? recruiter.Name : null, count, wcharacter) + string.Format("-> reverted to native {0}.", wcharacter3));
				return;
			}
			else
			{
				WCharacter wcharacter4 = wcharacter.IsElite ? wfaction.RootElite : wfaction.RootBasic;
				if (wcharacter4 == null)
				{
					return;
				}
				WCharacter wcharacter5 = TroopMatcher.PickBestFromTree(wcharacter4, wcharacter, null, false);
				if (wcharacter5 == null || wcharacter5 == wcharacter)
				{
					return;
				}
				TroopRoster memberRoster2 = partyBelongedTo.MemberRoster;
				memberRoster2.RemoveTroop(wcharacter.Base, count, default(UniqueTroopDescriptor), 0);
				memberRoster2.AddToCounts(wcharacter5.Base, count, false, 0, 0, true, -1);
				return;
			}
		}
	}
}
