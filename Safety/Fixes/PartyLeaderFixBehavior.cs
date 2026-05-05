using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;

namespace Retinues.Safety.Fixes
{
	// Token: 0x02000056 RID: 86
	[SafeClass]
	public class PartyLeaderFixBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000193 RID: 403 RVA: 0x0000B6CB File Offset: 0x000098CB
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000B6CD File Offset: 0x000098CD
		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000B6E6 File Offset: 0x000098E6
		private void OnGameLoadFinished()
		{
			PartyLeaderFixBehavior.FixPartyLeaders();
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000B6F0 File Offset: 0x000098F0
		public static void FixPartyLeaders()
		{
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (mobileParty != null && mobileParty.LeaderHero == null)
				{
					if (mobileParty.IsMainParty)
					{
						PartyLeaderFixBehavior.EnsurePartyLeader(mobileParty, Hero.MainHero);
					}
					else if (mobileParty.IsLordParty)
					{
						PartyLeaderFixBehavior.EnsurePartyLeader(mobileParty, null);
					}
				}
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000B76C File Offset: 0x0000996C
		private static void EnsurePartyLeader(MobileParty party, Hero hero = null)
		{
			if (((party != null) ? party.PartyComponent : null) == null)
			{
				Log.Warn(string.Format("Party {0} has no leader but also no PartyComponent; skipping.", (party != null) ? party.Name : null));
				return;
			}
			Log.Warn(string.Format("Party {0} has no leader, attempting to assign one.", party.Name));
			if (hero == null)
			{
				hero = PartyLeaderFixBehavior.FindHeroLeader(party);
			}
			if (hero == null)
			{
				return;
			}
			if (hero.PartyBelongedTo != null && hero.PartyBelongedTo != party && !party.IsMainParty)
			{
				Log.Warn(string.Format("Candidate leader {0} already belongs to party {1}; not changing leader for {2}.", hero.Name, hero.PartyBelongedTo.Name, party.Name));
				return;
			}
			party.PartyComponent.ChangePartyLeader(hero);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000B814 File Offset: 0x00009A14
		private static Hero FindHeroLeader(MobileParty party)
		{
			List<Hero> list = new List<Hero>();
			foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
			{
				CharacterObject character = troopRosterElement.Character;
				if (character != null && character.IsHero)
				{
					CharacterObject character2 = troopRosterElement.Character;
					if (((character2 != null) ? character2.HeroObject : null) != null)
					{
						list.Add(troopRosterElement.Character.HeroObject);
					}
				}
			}
			Hero hero = null;
			if (list.Count == 0)
			{
				Log.Warn(string.Format("No heroes found in party {0}", party.Name));
			}
			else if (list.Count == 1)
			{
				hero = list[0];
			}
			else
			{
				Log.Info(string.Format("Multiple heroes found in party {0}.", party.Name));
				List<Hero> list2 = (from h in list
				where h.IsLord
				select h).ToList<Hero>();
				if (list2.Count == 1)
				{
					hero = list2[0];
				}
				else if (list2.Count > 1)
				{
					Log.Warn("Multiple lord heroes found in party, cannot select leader.");
				}
			}
			if (hero != null)
			{
				Log.Info(string.Format("Selecting hero {0} as leader of party {1}.", hero.Name, party.Name));
			}
			else
			{
				Log.Warn(string.Format("Failed to find leader for party {0}.", party.Name));
			}
			return hero;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000B978 File Offset: 0x00009B78
		[CommandLineFunctionality.CommandLineArgumentFunction("fix_party_leaders", "retinues")]
		public static string FixCommand(List<string> args)
		{
			PartyLeaderFixBehavior.FixPartyLeaders();
			return "Fix applied.";
		}
	}
}
