using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Retinues.Game.Wrappers;
using Retinues.Mods;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Game.Events
{
	// Token: 0x020000A8 RID: 168
	[SafeClass]
	public class Battle : Combat
	{
		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x00024483 File Offset: 0x00022683
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return MissionBehaviorType.Other;
			}
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00024488 File Offset: 0x00022688
		public Battle(MapEvent mapEvent = null)
		{
			try
			{
				MapEvent mapEvent2 = mapEvent;
				if (mapEvent == null)
				{
					MobileParty mainParty = MobileParty.MainParty;
					mapEvent2 = ((mainParty != null) ? mainParty.MapEvent : null);
				}
				this.MapEvent = mapEvent2;
				WParty party = Player.Party;
				int? num;
				if (party == null)
				{
					num = null;
				}
				else
				{
					WRoster memberRoster = party.MemberRoster;
					num = ((memberRoster != null) ? new int?(memberRoster.HealthyCount) : null);
				}
				int? num2 = num;
				this.PlayerTroopCount = num2.GetValueOrDefault();
				this.EnemyTroopCount = this.GetRosters(this.EnemySide, false).Sum((WRoster r) => r.HealthyCount);
				this.AllyTroopCount = this.GetRosters(this.PlayerSide, false).Sum((WRoster r) => r.HealthyCount);
				this.FriendlyTroopCount = this.PlayerTroopCount + this.AllyTroopCount;
				this.TotalTroopCount = this.PlayerTroopCount + this.EnemyTroopCount + this.AllyTroopCount;
				this.EnemyPrisoners = (from e in this.GetRosters(this.EnemySide, true).SelectMany((WRoster r) => r.Elements)
				select e.Troop into t
				where t != null
				select t).ToList<WCharacter>();
				WParty party2 = Player.Party;
				this.PlayerIsInArmy = (party2 != null && party2.IsInArmy);
				this.AllyIsInArmy = this.PartiesOnSide(this.PlayerSide, false).Any((WParty p) => p != null && p.IsInArmy);
				this.EnemyIsInArmy = this.PartiesOnSide(this.EnemySide, false).Any((WParty p) => p != null && p.IsInArmy);
				this.EnemyLeaders = this.GetLeaders(this.EnemySide);
				this.AllyLeaders = (from l in this.GetLeaders(this.PlayerSide)
				where l != Player.Character
				select l).ToList<WCharacter>();
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x0002472C File Offset: 0x0002292C
		public bool IsWon
		{
			get
			{
				return this.MapEvent != null && this.PlayerSide != BattleSideEnum.None && this.MapEvent.WinningSide == this.PlayerSide;
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000726 RID: 1830 RVA: 0x00024754 File Offset: 0x00022954
		public bool IsLost
		{
			get
			{
				return !this.IsWon;
			}
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06000727 RID: 1831 RVA: 0x0002475F File Offset: 0x0002295F
		public bool IsFieldBattle
		{
			get
			{
				MapEvent mapEvent = this.MapEvent;
				return mapEvent != null && mapEvent.IsFieldBattle;
			}
		}

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06000728 RID: 1832 RVA: 0x00024774 File Offset: 0x00022974
		public bool IsNavalBattle
		{
			get
			{
				try
				{
					if (!ModCompatibility.HasNavalDLC)
					{
						return false;
					}
					Mission mission = Mission.Current;
					if (mission != null && Battle.MissionIsNavalBattleProperty != null)
					{
						object value = Battle.MissionIsNavalBattleProperty.GetValue(mission);
						bool flag;
						bool flag2;
						if (value is bool)
						{
							flag = (bool)value;
							flag2 = true;
						}
						else
						{
							flag2 = false;
						}
						if (flag2 && flag)
						{
							return true;
						}
					}
					if (this.MapEvent != null && Battle.MapEventIsNavalMapEventProperty != null)
					{
						object value = Battle.MapEventIsNavalMapEventProperty.GetValue(this.MapEvent);
						bool flag3;
						bool flag4;
						if (value is bool)
						{
							flag3 = (bool)value;
							flag4 = true;
						}
						else
						{
							flag4 = false;
						}
						if (flag4 && flag3)
						{
							return true;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
				return false;
			}
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06000729 RID: 1833 RVA: 0x00024834 File Offset: 0x00022A34
		public bool IsHideout
		{
			get
			{
				MapEvent mapEvent = this.MapEvent;
				return mapEvent != null && mapEvent.IsHideoutBattle;
			}
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x0600072A RID: 1834 RVA: 0x00024847 File Offset: 0x00022A47
		public bool IsSiege
		{
			get
			{
				MapEvent mapEvent = this.MapEvent;
				return mapEvent != null && mapEvent.IsSiegeAssault;
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x0600072B RID: 1835 RVA: 0x0002485A File Offset: 0x00022A5A
		public bool IsVillageRaid
		{
			get
			{
				if (this.MapEvent != null && !this.MapEvent.IsFieldBattle)
				{
					Settlement mapEventSettlement = this.MapEvent.MapEventSettlement;
					if (mapEventSettlement != null && mapEventSettlement.IsVillage)
					{
						return !this.IsSiege;
					}
				}
				return false;
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x0600072C RID: 1836 RVA: 0x00024895 File Offset: 0x00022A95
		public bool PlayerIsDefender
		{
			get
			{
				return this.PlayerSide == BattleSideEnum.Defender;
			}
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x000248A0 File Offset: 0x00022AA0
		public List<WParty> AllParties
		{
			get
			{
				List<WParty> list = new List<WParty>();
				list.AddRange(this.PartiesOnSide(BattleSideEnum.Attacker, false));
				list.AddRange(this.PartiesOnSide(BattleSideEnum.Defender, false));
				return list;
			}
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x0600072E RID: 1838 RVA: 0x000248C3 File Offset: 0x00022AC3
		public List<WParty> EnemyParties
		{
			get
			{
				return this.PartiesOnSide(this.EnemySide, false).ToList<WParty>();
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x0600072F RID: 1839 RVA: 0x000248D7 File Offset: 0x00022AD7
		public List<WParty> AllyParties
		{
			get
			{
				return this.PartiesOnSide(this.PlayerSide, false).ToList<WParty>();
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06000730 RID: 1840 RVA: 0x000248EC File Offset: 0x00022AEC
		public BattleSideEnum PlayerSide
		{
			get
			{
				if (this.MapEvent == null || Player.Party == null)
				{
					return BattleSideEnum.None;
				}
				using (IEnumerator<WParty> enumerator = this.PartiesOnSide(BattleSideEnum.Attacker, true).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == Player.Party)
						{
							return BattleSideEnum.Attacker;
						}
					}
				}
				using (IEnumerator<WParty> enumerator = this.PartiesOnSide(BattleSideEnum.Defender, true).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == Player.Party)
						{
							return BattleSideEnum.Defender;
						}
					}
				}
				return BattleSideEnum.None;
			}
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06000731 RID: 1841 RVA: 0x000249A4 File Offset: 0x00022BA4
		public BattleSideEnum EnemySide
		{
			get
			{
				if (this.PlayerSide == BattleSideEnum.Attacker)
				{
					return BattleSideEnum.Defender;
				}
				if (this.PlayerSide == BattleSideEnum.Defender)
				{
					return BattleSideEnum.Attacker;
				}
				return BattleSideEnum.None;
			}
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x000249BC File Offset: 0x00022BBC
		public void LogReport()
		{
			Log.Debug("--- Battle Report ---");
			Log.Debug("Outcome: " + (this.IsWon ? "Victory" : "Defeat"));
			Log.Debug("Type: " + (this.IsSiege ? "Siege" : (this.IsVillageRaid ? "Village Raid" : (this.IsHideout ? "Hideout" : "Field Battle"))));
			Log.Debug(string.Format("Sides: Player is {0}, Enemy is {1}", this.PlayerSide, this.EnemySide));
			Log.Debug(string.Format("Counts: Player={0}, Allies={1}, Enemies={2}, Total={3}", new object[]
			{
				this.PlayerTroopCount,
				this.AllyTroopCount,
				this.EnemyTroopCount,
				this.TotalTroopCount
			}));
			string[] array = new string[5];
			array[0] = "Enemy Leaders: [";
			array[1] = string.Join(", ", this.EnemyLeaders.Select(delegate(WCharacter l)
			{
				if (l == null)
				{
					return null;
				}
				return l.Name;
			}));
			array[2] = "], Ally Leaders: [";
			array[3] = string.Join(", ", this.AllyLeaders.Select(delegate(WCharacter l)
			{
				if (l == null)
				{
					return null;
				}
				return l.Name;
			}));
			array[4] = "]";
			Log.Debug(string.Concat(array));
			Log.Debug(string.Format("Player In Army: {0}, Allies In Army: {1}, Enemies In Army: {2}", this.PlayerIsInArmy, this.AllyIsInArmy, this.EnemyIsInArmy));
			Log.Debug("---------------------");
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00024B79 File Offset: 0x00022D79
		private List<WCharacter> GetLeaders(BattleSideEnum side)
		{
			return this.PartiesOnSide(side, false).Select(delegate(WParty p)
			{
				if (p == null)
				{
					return null;
				}
				return p.Leader;
			}).ToList<WCharacter>();
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00024BAC File Offset: 0x00022DAC
		private List<WRoster> GetRosters(BattleSideEnum side, bool prisoners = false)
		{
			List<WRoster> list = new List<WRoster>();
			foreach (WParty wparty in this.PartiesOnSide(side, false))
			{
				list.Add(prisoners ? wparty.PrisonRoster : wparty.MemberRoster);
			}
			return list;
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00024C14 File Offset: 0x00022E14
		public IEnumerable<WParty> PartiesOnSide(BattleSideEnum side, bool includePlayer = false)
		{
			Battle.<PartiesOnSide>d__46 <PartiesOnSide>d__ = new Battle.<PartiesOnSide>d__46(-2);
			<PartiesOnSide>d__.<>4__this = this;
			<PartiesOnSide>d__.<>3__side = side;
			<PartiesOnSide>d__.<>3__includePlayer = includePlayer;
			return <PartiesOnSide>d__;
		}

		// Token: 0x040001C9 RID: 457
		private static readonly PropertyInfo MissionIsNavalBattleProperty = typeof(Mission).GetProperty("IsNavalBattle", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040001CA RID: 458
		private static readonly PropertyInfo MapEventIsNavalMapEventProperty = typeof(MapEvent).GetProperty("IsNavalMapEvent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040001CB RID: 459
		public List<WCharacter> AllyLeaders = new List<WCharacter>();

		// Token: 0x040001CC RID: 460
		public List<WCharacter> EnemyLeaders = new List<WCharacter>();

		// Token: 0x040001CD RID: 461
		public bool PlayerIsInArmy;

		// Token: 0x040001CE RID: 462
		public bool AllyIsInArmy;

		// Token: 0x040001CF RID: 463
		public bool EnemyIsInArmy;

		// Token: 0x040001D0 RID: 464
		public int TotalTroopCount;

		// Token: 0x040001D1 RID: 465
		public int FriendlyTroopCount;

		// Token: 0x040001D2 RID: 466
		public int PlayerTroopCount;

		// Token: 0x040001D3 RID: 467
		public int EnemyTroopCount;

		// Token: 0x040001D4 RID: 468
		public int AllyTroopCount;

		// Token: 0x040001D5 RID: 469
		public List<WCharacter> EnemyPrisoners;

		// Token: 0x040001D6 RID: 470
		private readonly MapEvent MapEvent;
	}
}
