using System;
using System.Collections.Generic;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Retinues.Game.Wrappers
{
	// Token: 0x0200009A RID: 154
	[SafeClass]
	public class WParty : BaseFactionMember
	{
		// Token: 0x0600069A RID: 1690 RVA: 0x0002119B File Offset: 0x0001F39B
		public WParty(MobileParty party)
		{
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x0600069B RID: 1691 RVA: 0x000211AA File Offset: 0x0001F3AA
		public static IEnumerable<WParty> All
		{
			get
			{
				return new WParty.<get_All>d__2(-2);
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x000211B3 File Offset: 0x0001F3B3
		public MobileParty Base
		{
			get
			{
				return this._party;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x0600069D RID: 1693 RVA: 0x000211BB File Offset: 0x0001F3BB
		public WRoster MemberRoster
		{
			get
			{
				if (this._memberRoster == null)
				{
					this._memberRoster = new WRoster(this._party.MemberRoster, this);
				}
				return this._memberRoster;
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x000211E2 File Offset: 0x0001F3E2
		public WRoster PrisonRoster
		{
			get
			{
				if (this._party.PrisonRoster == null)
				{
					return null;
				}
				return new WRoster(this._party.PrisonRoster, this);
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x00021204 File Offset: 0x0001F404
		public WCharacter Leader
		{
			get
			{
				if (this._party.LeaderHero == null)
				{
					return null;
				}
				return new WCharacter(this._party.LeaderHero.CharacterObject);
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x0002122C File Offset: 0x0001F42C
		public override WFaction Clan
		{
			get
			{
				if (this._ownerClan == null)
				{
					if (this._party.ActualClan != null)
					{
						this._ownerClan = this._party.ActualClan;
					}
					Hero leaderHero = this._party.LeaderHero;
					if (((leaderHero != null) ? leaderHero.Clan : null) != null)
					{
						this._ownerClan = this._party.LeaderHero.Clan;
					}
					Settlement homeSettlement = this._party.HomeSettlement;
					if (((homeSettlement != null) ? homeSettlement.OwnerClan : null) != null)
					{
						this._ownerClan = this._party.HomeSettlement.OwnerClan;
					}
				}
				if (this._ownerClan == null)
				{
					return null;
				}
				return new WFaction(this._ownerClan);
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x060006A1 RID: 1697 RVA: 0x000212D2 File Offset: 0x0001F4D2
		public override WFaction Kingdom
		{
			get
			{
				if (!(this.Clan != null))
				{
					return null;
				}
				return new WFaction(this._ownerClan.Kingdom);
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x000212F4 File Offset: 0x0001F4F4
		public override string StringId
		{
			get
			{
				return this._party.StringId;
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x00021304 File Offset: 0x0001F504
		public string Name
		{
			get
			{
				string result;
				try
				{
					result = this._party.Name.ToString();
				}
				catch
				{
					result = "Unknown Party";
				}
				return result;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x00021340 File Offset: 0x0001F540
		public bool IsMainParty
		{
			get
			{
				return this._party.IsMainParty;
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x0002134D File Offset: 0x0001F54D
		public bool IsLordParty
		{
			get
			{
				return this._party.IsLordParty;
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x0002135A File Offset: 0x0001F55A
		public bool IsCustomParty
		{
			get
			{
				return this._party.IsCustomParty;
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x00021367 File Offset: 0x0001F567
		public bool IsVillager
		{
			get
			{
				return this._party.IsVillager;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060006A8 RID: 1704 RVA: 0x00021374 File Offset: 0x0001F574
		public bool IsCaravan
		{
			get
			{
				return this._party.IsCaravan;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x00021381 File Offset: 0x0001F581
		public bool IsBandit
		{
			get
			{
				return this._party.IsBandit;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x0002138E File Offset: 0x0001F58E
		public bool IsGarrison
		{
			get
			{
				return this._party.IsGarrison;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x0002139B File Offset: 0x0001F59B
		public bool IsMilitia
		{
			get
			{
				return this._party.IsMilitia;
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x000213A8 File Offset: 0x0001F5A8
		public int PartySizeLimit
		{
			get
			{
				return this._party.Party.PartySizeLimit;
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x000213BA File Offset: 0x0001F5BA
		public float Morale
		{
			get
			{
				return this._party.Morale;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060006AE RID: 1710 RVA: 0x000213C7 File Offset: 0x0001F5C7
		public Army Army
		{
			get
			{
				return this._party.Army;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x000213D4 File Offset: 0x0001F5D4
		public bool IsInArmy
		{
			get
			{
				return this.Army != null;
			}
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x000213E0 File Offset: 0x0001F5E0
		public static void SwapAll(bool members, bool prisoners, bool skipMainParty = false, bool skipLordParties = false, bool skipCustomParties = false, bool skipGarrisons = false, bool skipCaravans = false, bool skipVillagers = false, bool skipBandits = false, bool skipMilitia = false)
		{
			foreach (WParty wparty in WParty.All)
			{
				if ((!skipMainParty || !wparty.IsMainParty) && (!skipLordParties || !wparty.IsLordParty) && (!skipCustomParties || !wparty.IsCustomParty) && (!skipGarrisons || !wparty.IsGarrison) && (!skipCaravans || !wparty.IsCaravan) && (!skipVillagers || !wparty.IsVillager) && (!skipBandits || !wparty.IsBandit) && (!skipMilitia || !wparty.IsMilitia))
				{
					if (members)
					{
						WRoster memberRoster = wparty.MemberRoster;
						if (memberRoster != null)
						{
							memberRoster.SwapTroops(null, true);
						}
					}
					if (prisoners)
					{
						WRoster prisonRoster = wparty.PrisonRoster;
						if (prisonRoster != null)
						{
							prisonRoster.SwapTroops(null, true);
						}
					}
				}
			}
		}

		// Token: 0x04000196 RID: 406
		private readonly MobileParty _party = party;

		// Token: 0x04000197 RID: 407
		private WRoster _memberRoster;

		// Token: 0x04000198 RID: 408
		private Clan _ownerClan;
	}
}
