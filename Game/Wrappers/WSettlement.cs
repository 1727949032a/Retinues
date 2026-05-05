using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Configuration;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Retinues.Game.Wrappers
{
	// Token: 0x0200009D RID: 157
	[SafeClass]
	public class WSettlement : BaseFactionMember
	{
		// Token: 0x060006D4 RID: 1748 RVA: 0x00021F12 File Offset: 0x00020112
		public WSettlement(Settlement settlement)
		{
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x00021F21 File Offset: 0x00020121
		public static IEnumerable<WSettlement> All
		{
			get
			{
				return new WSettlement.<get_All>d__2(-2);
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x00021F2A File Offset: 0x0002012A
		public static IEnumerable<WSettlement> PlayerFactionSettlements
		{
			get
			{
				return new WSettlement.<get_PlayerFactionSettlements>d__4(-2);
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x060006D7 RID: 1751 RVA: 0x00021F34 File Offset: 0x00020134
		public static WSettlement Current
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				if (currentSettlement == null)
				{
					return null;
				}
				return new WSettlement(currentSettlement);
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x00021F52 File Offset: 0x00020152
		public Settlement Base
		{
			get
			{
				return this._settlement;
			}
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x060006D9 RID: 1753 RVA: 0x00021F5A File Offset: 0x0002015A
		public string Name
		{
			get
			{
				Settlement settlement = this._settlement;
				if (settlement == null)
				{
					return null;
				}
				TextObject name = settlement.Name;
				if (name == null)
				{
					return null;
				}
				return name.ToString();
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x00021F78 File Offset: 0x00020178
		public override string StringId
		{
			get
			{
				Settlement settlement = this._settlement;
				if (settlement == null)
				{
					return null;
				}
				return settlement.StringId;
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x060006DB RID: 1755 RVA: 0x00021F8B File Offset: 0x0002018B
		public bool IsTown
		{
			get
			{
				Settlement settlement = this._settlement;
				return settlement != null && settlement.IsTown;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x00021F9E File Offset: 0x0002019E
		public bool IsVillage
		{
			get
			{
				Settlement settlement = this._settlement;
				return settlement != null && settlement.IsVillage;
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x00021FB1 File Offset: 0x000201B1
		public bool IsCastle
		{
			get
			{
				Settlement settlement = this._settlement;
				return settlement != null && settlement.IsCastle;
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x00021FC4 File Offset: 0x000201C4
		public WHero Governor
		{
			get
			{
				Settlement settlement = this._settlement;
				bool flag;
				if (settlement == null)
				{
					flag = (null != null);
				}
				else
				{
					Town town = settlement.Town;
					flag = (((town != null) ? town.Governor : null) != null);
				}
				if (!flag)
				{
					return null;
				}
				Settlement settlement2 = this._settlement;
				Hero hero;
				if (settlement2 == null)
				{
					hero = null;
				}
				else
				{
					Town town2 = settlement2.Town;
					hero = ((town2 != null) ? town2.Governor : null);
				}
				return new WHero(hero);
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x060006DF RID: 1759 RVA: 0x00022018 File Offset: 0x00020218
		public List<WNotable> Notables
		{
			get
			{
				Settlement settlement = this._settlement;
				List<WNotable> list;
				if (settlement == null)
				{
					list = null;
				}
				else
				{
					list = (from n in settlement.Notables
					where n != null
					select new WNotable(n, this)).ToList<WNotable>();
				}
				return list ?? new List<WNotable>();
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x0002207A File Offset: 0x0002027A
		public WCulture Culture
		{
			get
			{
				Settlement settlement = this._settlement;
				return new WCulture((settlement != null) ? settlement.Culture : null);
			}
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x060006E1 RID: 1761 RVA: 0x00022094 File Offset: 0x00020294
		public override WFaction Clan
		{
			get
			{
				Settlement settlement = this._settlement;
				Clan clan = (settlement != null) ? settlement.OwnerClan : null;
				if (clan == null)
				{
					return null;
				}
				if (clan == TaleWorlds.CampaignSystem.Clan.PlayerClan)
				{
					return Player.Clan;
				}
				return new WFaction(clan);
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x000220D0 File Offset: 0x000202D0
		public override WFaction Kingdom
		{
			get
			{
				Settlement settlement = this._settlement;
				Clan clan = (settlement != null) ? settlement.OwnerClan : null;
				Kingdom kingdom = (clan != null) ? clan.Kingdom : null;
				if (kingdom == null)
				{
					return null;
				}
				IFaction faction = kingdom;
				WFaction kingdom2 = Player.Kingdom;
				if (faction == ((kingdom2 != null) ? kingdom2.Base : null))
				{
					return Player.Kingdom;
				}
				return new WFaction(kingdom);
			}
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x060006E3 RID: 1763 RVA: 0x00022124 File Offset: 0x00020324
		public WParty MilitiaParty
		{
			get
			{
				Settlement settlement = this._settlement;
				bool flag;
				if (settlement == null)
				{
					flag = (null != null);
				}
				else
				{
					MilitiaPartyComponent militiaPartyComponent = settlement.MilitiaPartyComponent;
					flag = (((militiaPartyComponent != null) ? militiaPartyComponent.MobileParty : null) != null);
				}
				if (!flag)
				{
					return null;
				}
				Settlement settlement2 = this._settlement;
				MobileParty party;
				if (settlement2 == null)
				{
					party = null;
				}
				else
				{
					MilitiaPartyComponent militiaPartyComponent2 = settlement2.MilitiaPartyComponent;
					party = ((militiaPartyComponent2 != null) ? militiaPartyComponent2.MobileParty : null);
				}
				return new WParty(party);
			}
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x00022178 File Offset: 0x00020378
		public WParty GarrisonParty
		{
			get
			{
				Settlement settlement = this._settlement;
				bool flag;
				if (settlement == null)
				{
					flag = (null != null);
				}
				else
				{
					Town town = settlement.Town;
					flag = (((town != null) ? town.GarrisonParty : null) != null);
				}
				if (!flag)
				{
					return null;
				}
				Settlement settlement2 = this._settlement;
				MobileParty party;
				if (settlement2 == null)
				{
					party = null;
				}
				else
				{
					Town town2 = settlement2.Town;
					party = ((town2 != null) ? town2.GarrisonParty : null);
				}
				return new WParty(party);
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x060006E5 RID: 1765 RVA: 0x000221CC File Offset: 0x000203CC
		public WCharacter MilitiaMelee
		{
			get
			{
				WFaction playerFaction = base.PlayerFaction;
				bool flag;
				if (playerFaction == null)
				{
					flag = false;
				}
				else
				{
					WCharacter militiaMelee = playerFaction.MilitiaMelee;
					flag = ((militiaMelee != null) ? new bool?(militiaMelee.IsActive) : null).GetValueOrDefault();
				}
				if (!flag)
				{
					WCulture culture = this.Culture;
					if (culture == null)
					{
						return null;
					}
					return culture.MilitiaMelee;
				}
				else
				{
					WFaction playerFaction2 = base.PlayerFaction;
					if (playerFaction2 == null)
					{
						return null;
					}
					return playerFaction2.MilitiaMelee;
				}
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x00022234 File Offset: 0x00020434
		public WCharacter MilitiaMeleeElite
		{
			get
			{
				WFaction playerFaction = base.PlayerFaction;
				bool flag;
				if (playerFaction == null)
				{
					flag = false;
				}
				else
				{
					WCharacter militiaMeleeElite = playerFaction.MilitiaMeleeElite;
					flag = ((militiaMeleeElite != null) ? new bool?(militiaMeleeElite.IsActive) : null).GetValueOrDefault();
				}
				if (!flag)
				{
					WCulture culture = this.Culture;
					if (culture == null)
					{
						return null;
					}
					return culture.MilitiaMeleeElite;
				}
				else
				{
					WFaction playerFaction2 = base.PlayerFaction;
					if (playerFaction2 == null)
					{
						return null;
					}
					return playerFaction2.MilitiaMeleeElite;
				}
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x060006E7 RID: 1767 RVA: 0x0002229C File Offset: 0x0002049C
		public WCharacter MilitiaRanged
		{
			get
			{
				WFaction playerFaction = base.PlayerFaction;
				bool flag;
				if (playerFaction == null)
				{
					flag = false;
				}
				else
				{
					WCharacter militiaRanged = playerFaction.MilitiaRanged;
					flag = ((militiaRanged != null) ? new bool?(militiaRanged.IsActive) : null).GetValueOrDefault();
				}
				if (!flag)
				{
					WCulture culture = this.Culture;
					if (culture == null)
					{
						return null;
					}
					return culture.MilitiaRanged;
				}
				else
				{
					WFaction playerFaction2 = base.PlayerFaction;
					if (playerFaction2 == null)
					{
						return null;
					}
					return playerFaction2.MilitiaRanged;
				}
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x00022304 File Offset: 0x00020504
		public WCharacter MilitiaRangedElite
		{
			get
			{
				WFaction playerFaction = base.PlayerFaction;
				bool flag;
				if (playerFaction == null)
				{
					flag = false;
				}
				else
				{
					WCharacter militiaRangedElite = playerFaction.MilitiaRangedElite;
					flag = ((militiaRangedElite != null) ? new bool?(militiaRangedElite.IsActive) : null).GetValueOrDefault();
				}
				if (!flag)
				{
					WCulture culture = this.Culture;
					if (culture == null)
					{
						return null;
					}
					return culture.MilitiaRangedElite;
				}
				else
				{
					WFaction playerFaction2 = base.PlayerFaction;
					if (playerFaction2 == null)
					{
						return null;
					}
					return playerFaction2.MilitiaRangedElite;
				}
			}
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0002236C File Offset: 0x0002056C
		[return: TupleElementNames(new string[]
		{
			"item",
			"count"
		})]
		public List<ValueTuple<WItem, int>> ItemCounts()
		{
			List<ValueTuple<WItem, int>> list = new List<ValueTuple<WItem, int>>();
			foreach (ItemRosterElement itemRosterElement in this._settlement.ItemRoster)
			{
				list.Add(new ValueTuple<WItem, int>(new WItem(itemRosterElement.EquipmentElement.Item), itemRosterElement.Amount));
			}
			return list;
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x000223E4 File Offset: 0x000205E4
		public static void SwapAll(bool members, bool prisoners, bool skipGarrisons = false, bool skipMilitias = false)
		{
			foreach (WSettlement wsettlement in WSettlement.All)
			{
				if (!skipGarrisons)
				{
					if (members)
					{
						WParty garrisonParty = wsettlement.GarrisonParty;
						if (garrisonParty != null)
						{
							WRoster memberRoster = garrisonParty.MemberRoster;
							if (memberRoster != null)
							{
								memberRoster.SwapTroops(null, true);
							}
						}
					}
					if (prisoners)
					{
						WParty garrisonParty2 = wsettlement.GarrisonParty;
						if (garrisonParty2 != null)
						{
							WRoster prisonRoster = garrisonParty2.PrisonRoster;
							if (prisonRoster != null)
							{
								prisonRoster.SwapTroops(null, true);
							}
						}
					}
				}
				if (!skipMilitias)
				{
					if (members)
					{
						WParty militiaParty = wsettlement.MilitiaParty;
						if (militiaParty != null)
						{
							WRoster memberRoster2 = militiaParty.MemberRoster;
							if (memberRoster2 != null)
							{
								memberRoster2.SwapTroops(null, true);
							}
						}
					}
					if (prisoners)
					{
						WParty militiaParty2 = wsettlement.MilitiaParty;
						if (militiaParty2 != null)
						{
							WRoster prisonRoster2 = militiaParty2.PrisonRoster;
							if (prisonRoster2 != null)
							{
								prisonRoster2.SwapTroops(null, true);
							}
						}
					}
				}
			}
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x000224BC File Offset: 0x000206BC
		public void SwapVolunteers(WFaction faction = null)
		{
			if (faction == null)
			{
				faction = base.PlayerFaction;
			}
			if (faction == null)
			{
				return;
			}
			if (Config.RestrictToSameCultureSettlements && faction.Culture != this.Culture)
			{
				return;
			}
			WFaction wfaction = null;
			float num = 0f;
			if (!Config.DisableKingdomTroops)
			{
				WFaction clan = this.Clan;
				WFaction kingdom = this.Kingdom;
				bool flag = clan != null && clan.IsPlayerClan;
				bool flag2 = kingdom != null && kingdom.IsPlayerKingdom;
				if (flag && flag2 && clan != null && kingdom != null)
				{
					if (faction == clan)
					{
						wfaction = kingdom;
						num = Config.KingdomVolunteersInClanFiefsProportion;
					}
				}
				else if (!flag && flag2 && kingdom != null && Player.Clan != null)
				{
					if (faction == kingdom)
					{
						wfaction = Player.Clan;
						num = Config.ClanVolunteersInKingdomFiefsProportion;
					}
				}
				else if (!flag2 && Player.Clan != null && Player.Kingdom != null && faction == Player.Clan && !Config.RestrictToOwnedSettlements)
				{
					wfaction = Player.Kingdom;
					num = Config.KingdomVolunteersInClanFiefsProportion;
				}
				if (num < 0f)
				{
					num = 0f;
				}
				else if (num > 1f)
				{
					num = 1f;
				}
				if (wfaction == null || num <= 0f)
				{
					wfaction = null;
					num = 0f;
				}
			}
			if (wfaction != null)
			{
				Log.Debug(string.Format("Swapping volunteers in settlement '{0}' for faction '{1}' with mix '{2}' (p={3:0.##}).", new object[]
				{
					this,
					faction,
					wfaction,
					num
				}));
			}
			else
			{
				Log.Debug(string.Format("Swapping volunteers in settlement '{0}' for faction '{1}'.", this, faction));
			}
			foreach (WNotable wnotable in this.Notables)
			{
				try
				{
					wnotable.SwapVolunteers(faction, wfaction, num);
				}
				catch (Exception arg)
				{
					Log.Error(string.Format("Exception while processing notable {0} in settlement {1}: {2}", wnotable, this, arg));
				}
			}
		}

		// Token: 0x0400019F RID: 415
		private readonly Settlement _settlement = settlement;
	}
}
