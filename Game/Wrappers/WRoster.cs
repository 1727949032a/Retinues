using System;
using System.Collections.Generic;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace Retinues.Game.Wrappers
{
	// Token: 0x0200009B RID: 155
	[SafeClass]
	public class WRoster
	{
		// Token: 0x060006B1 RID: 1713 RVA: 0x000214B8 File Offset: 0x0001F6B8
		public WRoster(TroopRoster roster, WParty party)
		{
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060006B2 RID: 1714 RVA: 0x000214CE File Offset: 0x0001F6CE
		public TroopRoster Base
		{
			get
			{
				return this._roster;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x000214D6 File Offset: 0x0001F6D6
		public WParty Party
		{
			get
			{
				return this._party;
			}
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x000214DE File Offset: 0x0001F6DE
		public WRoster(TroopRoster roster) : this(roster, new WParty(Reflector.GetPropertyValue<PartyBase>(roster, "OwnerParty").MobileParty))
		{
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x000214FC File Offset: 0x0001F6FC
		public IEnumerable<WRosterElement> Elements
		{
			get
			{
				WRoster.<get_Elements>d__9 <get_Elements>d__ = new WRoster.<get_Elements>d__9(-2);
				<get_Elements>d__.<>4__this = this;
				return <get_Elements>d__;
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x0002150C File Offset: 0x0001F70C
		public int Count
		{
			get
			{
				return this._roster.TotalManCount;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x00021519 File Offset: 0x0001F719
		public int HealthyCount
		{
			get
			{
				return this._roster.TotalHealthyCount;
			}
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00021526 File Offset: 0x0001F726
		public int CountOf(WCharacter troop)
		{
			if (troop.Base == null)
			{
				Log.Warn("CountOf: troop has no base!");
				return 0;
			}
			return this._roster.GetTroopCount(troop.Base);
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0002154D File Offset: 0x0001F74D
		public void AddTroop(WCharacter troop, int healthy, int wounded = 0, int xp = 0, int index = -1)
		{
			if (troop.Base == null)
			{
				return;
			}
			this._roster.AddToCounts(troop.Base, healthy, false, wounded, xp, true, index);
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x00021572 File Offset: 0x0001F772
		public void RemoveTroop(WCharacter troop, int healthy, int wounded = 0)
		{
			if (troop.Base == null)
			{
				return;
			}
			this._roster.AddToCounts(troop.Base, -healthy, false, -wounded, 0, true, -1);
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00021598 File Offset: 0x0001F798
		public void SwapTroop(WCharacter troop, WCharacter target)
		{
			if (troop.Base == null || target.Base == null)
			{
				return;
			}
			foreach (WRosterElement wrosterElement in this.Elements)
			{
				if (wrosterElement.Troop == troop)
				{
					int number = wrosterElement.Number;
					int woundedNumber = wrosterElement.WoundedNumber;
					int xp = wrosterElement.Xp;
					int index = wrosterElement.Index;
					this._roster.AddToCounts(troop.Base, -number, false, -woundedNumber, 0, true, -1);
					this._roster.AddToCounts(target.Base, number, false, woundedNumber, xp, true, index);
					Log.Debug(string.Format("{0}: swapped {1}x {2} to {3}.", new object[]
					{
						this.Party.Name,
						number,
						troop.Name,
						target.Name
					}));
					break;
				}
			}
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x00021698 File Offset: 0x0001F898
		public void SwapTroops(WFaction faction = null, bool skipHeroParties = true)
		{
			if (this.Base == null)
			{
				return;
			}
			if (faction == null)
			{
				faction = this.Party.PlayerFaction;
			}
			if (faction == null)
			{
				return;
			}
			try
			{
				bool flag = false;
				TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
				foreach (WRosterElement wrosterElement in new List<WRosterElement>(this.Elements))
				{
					bool flag2;
					if (wrosterElement == null)
					{
						flag2 = (null != null);
					}
					else
					{
						WCharacter troop = wrosterElement.Troop;
						flag2 = (((troop != null) ? troop.Base : null) != null);
					}
					if (flag2)
					{
						if (wrosterElement.Troop.IsHero)
						{
							if (skipHeroParties)
							{
								return;
							}
							troopRoster.AddToCounts(wrosterElement.Troop.Base, wrosterElement.Number, false, wrosterElement.WoundedNumber, wrosterElement.Xp, true, -1);
						}
						else
						{
							WCharacter wcharacter = TroopMatcher.PickBestFromFaction(faction, wrosterElement.Troop, true, true, null) ?? wrosterElement.Troop;
							if (wcharacter != wrosterElement.Troop)
							{
								Log.Debug(string.Format("{0}: swapping {1}x {2} to {3}.", new object[]
								{
									this.Party.Name,
									wrosterElement.Number,
									wrosterElement.Troop.Name,
									wcharacter.Name
								}));
								flag = true;
							}
							troopRoster.AddToCounts(wcharacter.Base, wrosterElement.Number, false, wrosterElement.WoundedNumber, wrosterElement.Xp, true, -1);
						}
					}
				}
				TroopRoster @base = this.Base;
				@base.Clear();
				@base.Add(troopRoster);
				if (flag)
				{
					Log.Debug(string.Format("{0} (militia: {1}): swapped all troops to faction {2}.", this.Party.Name, this.Party.IsMilitia, ((faction != null) ? faction.Name : null) ?? "null"));
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "SwapTroops failed for " + this.Party.Name, null);
			}
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x000218B0 File Offset: 0x0001FAB0
		public void SwapTroopsPreservingHeroes(WFaction faction = null)
		{
			if (this.Base == null)
			{
				return;
			}
			if (faction == null)
			{
				faction = this.Party.PlayerFaction;
			}
			if (faction == null)
			{
				return;
			}
			try
			{
				bool flag = false;
				List<ValueTuple<WCharacter, int, int, int>> list = new List<ValueTuple<WCharacter, int, int, int>>();
				int num = 0;
				foreach (TroopRosterElement troopRosterElement in this._roster.GetTroopRoster())
				{
					WCharacter item = new WCharacter(this._roster.GetCharacterAtIndex(num));
					list.Add(new ValueTuple<WCharacter, int, int, int>(item, this._roster.GetElementNumber(num), this._roster.GetElementWoundedNumber(num), this._roster.GetElementXp(num)));
					num++;
				}
				foreach (ValueTuple<WCharacter, int, int, int> valueTuple in list)
				{
					WCharacter item2 = valueTuple.Item1;
					if (((item2 != null) ? item2.Base : null) != null && !valueTuple.Item1.IsHero)
					{
						WCharacter wcharacter = TroopMatcher.PickBestFromFaction(faction, valueTuple.Item1, true, true, null) ?? valueTuple.Item1;
						if (!(wcharacter == valueTuple.Item1))
						{
							this._roster.AddToCounts(valueTuple.Item1.Base, -valueTuple.Item2, false, -valueTuple.Item3, 0, true, -1);
							this._roster.AddToCounts(wcharacter.Base, valueTuple.Item2, false, valueTuple.Item3, valueTuple.Item4, true, -1);
							flag = true;
							Log.Debug(string.Format("{0}: swapped {1}x {2} to {3} (hero-safe).", new object[]
							{
								this.Party.Name,
								valueTuple.Item2,
								valueTuple.Item1.Name,
								wcharacter.Name
							}));
						}
					}
				}
				if (flag)
				{
					Log.Debug(this.Party.Name + ": hero-safe swap complete for faction " + (((faction != null) ? faction.Name : null) ?? "null") + ".");
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "SwapTroopsPreservingHeroes failed for " + this.Party.Name, null);
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x00021B38 File Offset: 0x0001FD38
		public int HeroCount
		{
			get
			{
				int num = 0;
				foreach (WRosterElement wrosterElement in this.Elements)
				{
					if (wrosterElement.Troop.IsHero)
					{
						num += wrosterElement.Number;
					}
				}
				return num;
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x00021B98 File Offset: 0x0001FD98
		public int EliteCount
		{
			get
			{
				int num = 0;
				foreach (WRosterElement wrosterElement in this.Elements)
				{
					if (wrosterElement.Troop.IsElite)
					{
						num += wrosterElement.Number;
					}
				}
				return num;
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00021BF8 File Offset: 0x0001FDF8
		public float EliteRatio
		{
			get
			{
				if (this.HealthyCount - this.HeroCount > 0)
				{
					return (float)this.EliteCount / (float)(this.HealthyCount - this.HeroCount);
				}
				return 0f;
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00021C28 File Offset: 0x0001FE28
		public int CustomCount
		{
			get
			{
				int num = 0;
				foreach (WRosterElement wrosterElement in this.Elements)
				{
					if (wrosterElement.Troop.IsCustom)
					{
						num += wrosterElement.Number;
					}
				}
				return num;
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00021C88 File Offset: 0x0001FE88
		public float CustomRatio
		{
			get
			{
				if (this.HealthyCount - this.HeroCount > 0)
				{
					return (float)this.CustomCount / (float)(this.HealthyCount - this.HeroCount);
				}
				return 0f;
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00021CB8 File Offset: 0x0001FEB8
		public int RetinueCount
		{
			get
			{
				int num = 0;
				foreach (WRosterElement wrosterElement in this.Elements)
				{
					if (wrosterElement.Troop.IsRetinue)
					{
						num += wrosterElement.Number;
					}
				}
				return num;
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00021D18 File Offset: 0x0001FF18
		public float RetinueRatio
		{
			get
			{
				if (this.HealthyCount - this.HeroCount <= 0)
				{
					return 0f;
				}
				return (float)this.RetinueCount / (float)(this.HealthyCount - this.HeroCount);
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060006C5 RID: 1733 RVA: 0x00021D46 File Offset: 0x0001FF46
		public int InfantryCount
		{
			get
			{
				return this.CountByFormation(FormationClass.Infantry);
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x00021D4F File Offset: 0x0001FF4F
		public int ArchersCount
		{
			get
			{
				return this.CountByFormation(FormationClass.Ranged);
			}
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x00021D58 File Offset: 0x0001FF58
		public int CavalryCount
		{
			get
			{
				return this.CountByFormation(FormationClass.Cavalry);
			}
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x00021D61 File Offset: 0x0001FF61
		public float InfantryRatio
		{
			get
			{
				if (this.HealthyCount - this.HeroCount > 0)
				{
					return (float)this.InfantryCount / (float)(this.HealthyCount - this.HeroCount);
				}
				return 0f;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060006C9 RID: 1737 RVA: 0x00021D8F File Offset: 0x0001FF8F
		public float ArchersRatio
		{
			get
			{
				if (this.HealthyCount - this.HeroCount > 0)
				{
					return (float)this.ArchersCount / (float)(this.HealthyCount - this.HeroCount);
				}
				return 0f;
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x00021DBD File Offset: 0x0001FFBD
		public float CavalryRatio
		{
			get
			{
				if (this.HealthyCount - this.HeroCount > 0)
				{
					return (float)this.CavalryCount / (float)(this.HealthyCount - this.HeroCount);
				}
				return 0f;
			}
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x00021DEC File Offset: 0x0001FFEC
		private int CountByFormation(FormationClass cls)
		{
			int num = 0;
			foreach (WRosterElement wrosterElement in this.Elements)
			{
				CharacterObject @base = wrosterElement.Troop.Base;
				FormationClass? formationClass = (@base != null) ? new FormationClass?(@base.DefaultFormationClass) : null;
				if (formationClass.GetValueOrDefault() == cls & formationClass != null)
				{
					num += wrosterElement.Number;
				}
			}
			return num;
		}

		// Token: 0x04000199 RID: 409
		private readonly TroopRoster _roster = roster;

		// Token: 0x0400019A RID: 410
		private readonly WParty _party = party;
	}
}
