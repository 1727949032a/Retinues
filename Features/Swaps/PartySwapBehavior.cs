using System;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace Retinues.Features.Swaps
{
	// Token: 0x020000B6 RID: 182
	[SafeClass]
	public class PartySwapBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000777 RID: 1911 RVA: 0x000266E8 File Offset: 0x000248E8
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x000266EA File Offset: 0x000248EA
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnDailyTickParty));
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00026704 File Offset: 0x00024904
		private void OnDailyTickParty(MobileParty party)
		{
			if (party == null)
			{
				return;
			}
			WParty wparty = new WParty(party);
			if (!wparty.IsMilitia && !wparty.IsVillager && !wparty.IsCaravan)
			{
				return;
			}
			WFaction playerFaction = wparty.PlayerFaction;
			if (playerFaction == null)
			{
				return;
			}
			try
			{
				if (wparty.IsCaravan)
				{
					WRoster memberRoster = wparty.MemberRoster;
					if (memberRoster != null)
					{
						memberRoster.SwapTroopsPreservingHeroes(playerFaction);
					}
				}
				else
				{
					WRoster memberRoster2 = wparty.MemberRoster;
					if (memberRoster2 != null)
					{
						memberRoster2.SwapTroops(playerFaction, true);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "PartySwapBehavior failed for " + wparty.Name, null);
			}
		}
	}
}
