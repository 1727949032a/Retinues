using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000094 RID: 148
	[SafeClass]
	public class WFaction : BaseBannerFaction
	{
		// Token: 0x060005EF RID: 1519 RVA: 0x0001E9A5 File Offset: 0x0001CBA5
		public WFaction(IFaction faction)
		{
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x0001E9B4 File Offset: 0x0001CBB4
		public IFaction Base
		{
			get
			{
				return this._faction;
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x0001E9BC File Offset: 0x0001CBBC
		public override string Name
		{
			get
			{
				IFaction faction = this._faction;
				if (faction == null)
				{
					return null;
				}
				return faction.Name.ToString();
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x0001E9D4 File Offset: 0x0001CBD4
		public override string StringId
		{
			get
			{
				IFaction faction = this._faction;
				if (faction == null)
				{
					return null;
				}
				return faction.StringId;
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x060005F3 RID: 1523 RVA: 0x0001E9E7 File Offset: 0x0001CBE7
		public override uint Color
		{
			get
			{
				IFaction faction = this._faction;
				if (faction == null)
				{
					return 0U;
				}
				return faction.Color;
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x060005F4 RID: 1524 RVA: 0x0001E9FA File Offset: 0x0001CBFA
		public override uint Color2
		{
			get
			{
				IFaction faction = this._faction;
				if (faction == null)
				{
					return 0U;
				}
				return faction.Color2;
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x060005F5 RID: 1525 RVA: 0x0001EA0D File Offset: 0x0001CC0D
		public override Banner BaseBanner
		{
			get
			{
				IFaction @base = this.Base;
				if (@base == null)
				{
					return null;
				}
				return @base.Banner;
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x060005F6 RID: 1526 RVA: 0x0001EA20 File Offset: 0x0001CC20
		public BannerImageIdentifier Image
		{
			get
			{
				if (this.Base.Banner == null)
				{
					return null;
				}
				return new BannerImageIdentifier(this.Base.Banner, false);
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x060005F7 RID: 1527 RVA: 0x0001EA42 File Offset: 0x0001CC42
		public ImageIdentifier ImageIdentifier
		{
			get
			{
				if (this.Base.Banner == null)
				{
					return null;
				}
				return new BannerImageIdentifier(this.Base.Banner, false);
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x060005F8 RID: 1528 RVA: 0x0001EA64 File Offset: 0x0001CC64
		public bool HasFiefs
		{
			get
			{
				MBReadOnlyList<Town> fiefs = this.Base.Fiefs;
				return fiefs != null && fiefs.Count > 0;
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060005F9 RID: 1529 RVA: 0x0001EA7F File Offset: 0x0001CC7F
		public bool IsClan
		{
			get
			{
				return this.Base is Clan;
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x0001EA8F File Offset: 0x0001CC8F
		public bool IsKingdom
		{
			get
			{
				return this.Base is Kingdom;
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x060005FB RID: 1531 RVA: 0x0001EA9F File Offset: 0x0001CC9F
		public bool IsPlayerFaction
		{
			get
			{
				return this == Player.Clan || this == Player.Kingdom;
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x0001EABB File Offset: 0x0001CCBB
		public bool IsPlayerClan
		{
			get
			{
				return this == Player.Clan;
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x060005FD RID: 1533 RVA: 0x0001EAC8 File Offset: 0x0001CCC8
		public bool IsPlayerKingdom
		{
			get
			{
				return this == Player.Kingdom;
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x0001EAD5 File Offset: 0x0001CCD5
		public List<WSettlement> Settlements
		{
			get
			{
				IFaction faction = this._faction;
				IEnumerable<WSettlement> source;
				if (faction == null)
				{
					source = null;
				}
				else
				{
					source = faction.Settlements.Select(delegate(Settlement s)
					{
						if (s != null)
						{
							return new WSettlement(s);
						}
						return null;
					});
				}
				return source.ToList<WSettlement>();
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x060005FF RID: 1535 RVA: 0x0001EB14 File Offset: 0x0001CD14
		public List<WParty> Parties
		{
			get
			{
				return (from mp in MobileParty.All
				select new WParty(mp) into p
				where p.PlayerFaction == this
				select p).ToList<WParty>();
			}
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x0001EB60 File Offset: 0x0001CD60
		private void Replace(WCharacter oldTroop, WCharacter newTroop)
		{
			if (oldTroop == null || newTroop == null)
			{
				return;
			}
			oldTroop.Remove(newTroop);
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06000601 RID: 1537 RVA: 0x0001EB7C File Offset: 0x0001CD7C
		// (set) Token: 0x06000602 RID: 1538 RVA: 0x0001EB9A File Offset: 0x0001CD9A
		public override WCharacter RetinueElite
		{
			get
			{
				WCharacter retinueElite = this._retinueElite;
				if (retinueElite == null || !retinueElite.IsValid)
				{
					return null;
				}
				return this._retinueElite;
			}
			set
			{
				this.Replace(this._retinueElite, value);
				this._retinueElite = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000603 RID: 1539 RVA: 0x0001EBB6 File Offset: 0x0001CDB6
		// (set) Token: 0x06000604 RID: 1540 RVA: 0x0001EBD4 File Offset: 0x0001CDD4
		public override WCharacter RetinueBasic
		{
			get
			{
				WCharacter retinueBasic = this._retinueBasic;
				if (retinueBasic == null || !retinueBasic.IsValid)
				{
					return null;
				}
				return this._retinueBasic;
			}
			set
			{
				this.Replace(this._retinueBasic, value);
				this._retinueBasic = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x0001EBF0 File Offset: 0x0001CDF0
		// (set) Token: 0x06000606 RID: 1542 RVA: 0x0001EC0E File Offset: 0x0001CE0E
		public override WCharacter RootElite
		{
			get
			{
				WCharacter rootElite = this._rootElite;
				if (rootElite == null || !rootElite.IsValid)
				{
					return null;
				}
				return this._rootElite;
			}
			set
			{
				this.Replace(this._rootElite, value);
				this._rootElite = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000607 RID: 1543 RVA: 0x0001EC2A File Offset: 0x0001CE2A
		// (set) Token: 0x06000608 RID: 1544 RVA: 0x0001EC48 File Offset: 0x0001CE48
		public override WCharacter RootBasic
		{
			get
			{
				WCharacter rootBasic = this._rootBasic;
				if (rootBasic == null || !rootBasic.IsValid)
				{
					return null;
				}
				return this._rootBasic;
			}
			set
			{
				this.Replace(this._rootBasic, value);
				this._rootBasic = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000609 RID: 1545 RVA: 0x0001EC64 File Offset: 0x0001CE64
		// (set) Token: 0x0600060A RID: 1546 RVA: 0x0001EC82 File Offset: 0x0001CE82
		public override WCharacter MilitiaMelee
		{
			get
			{
				WCharacter militiaMelee = this._militiaMelee;
				if (militiaMelee == null || !militiaMelee.IsValid)
				{
					return null;
				}
				return this._militiaMelee;
			}
			set
			{
				this.Replace(this._militiaMelee, value);
				this._militiaMelee = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x0600060B RID: 1547 RVA: 0x0001EC9E File Offset: 0x0001CE9E
		// (set) Token: 0x0600060C RID: 1548 RVA: 0x0001ECBC File Offset: 0x0001CEBC
		public override WCharacter MilitiaMeleeElite
		{
			get
			{
				WCharacter militiaMeleeElite = this._militiaMeleeElite;
				if (militiaMeleeElite == null || !militiaMeleeElite.IsValid)
				{
					return null;
				}
				return this._militiaMeleeElite;
			}
			set
			{
				this.Replace(this._militiaMeleeElite, value);
				this._militiaMeleeElite = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x0600060D RID: 1549 RVA: 0x0001ECD8 File Offset: 0x0001CED8
		// (set) Token: 0x0600060E RID: 1550 RVA: 0x0001ECF6 File Offset: 0x0001CEF6
		public override WCharacter MilitiaRanged
		{
			get
			{
				WCharacter militiaRanged = this._militiaRanged;
				if (militiaRanged == null || !militiaRanged.IsValid)
				{
					return null;
				}
				return this._militiaRanged;
			}
			set
			{
				this.Replace(this._militiaRanged, value);
				this._militiaRanged = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x0600060F RID: 1551 RVA: 0x0001ED12 File Offset: 0x0001CF12
		// (set) Token: 0x06000610 RID: 1552 RVA: 0x0001ED30 File Offset: 0x0001CF30
		public override WCharacter MilitiaRangedElite
		{
			get
			{
				WCharacter militiaRangedElite = this._militiaRangedElite;
				if (militiaRangedElite == null || !militiaRangedElite.IsValid)
				{
					return null;
				}
				return this._militiaRangedElite;
			}
			set
			{
				this.Replace(this._militiaRangedElite, value);
				this._militiaRangedElite = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06000611 RID: 1553 RVA: 0x0001ED4C File Offset: 0x0001CF4C
		// (set) Token: 0x06000612 RID: 1554 RVA: 0x0001ED6A File Offset: 0x0001CF6A
		public override WCharacter CaravanGuard
		{
			get
			{
				WCharacter caravanGuard = this._caravanGuard;
				if (caravanGuard == null || !caravanGuard.IsValid)
				{
					return null;
				}
				return this._caravanGuard;
			}
			set
			{
				this.Replace(this._caravanGuard, value);
				this._caravanGuard = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000613 RID: 1555 RVA: 0x0001ED86 File Offset: 0x0001CF86
		// (set) Token: 0x06000614 RID: 1556 RVA: 0x0001EDA4 File Offset: 0x0001CFA4
		public override WCharacter CaravanMaster
		{
			get
			{
				WCharacter caravanMaster = this._caravanMaster;
				if (caravanMaster == null || !caravanMaster.IsValid)
				{
					return null;
				}
				return this._caravanMaster;
			}
			set
			{
				this.Replace(this._caravanMaster, value);
				this._caravanMaster = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000615 RID: 1557 RVA: 0x0001EDC0 File Offset: 0x0001CFC0
		// (set) Token: 0x06000616 RID: 1558 RVA: 0x0001EDDE File Offset: 0x0001CFDE
		public override WCharacter Villager
		{
			get
			{
				WCharacter villager = this._villager;
				if (villager == null || !villager.IsValid)
				{
					return null;
				}
				return this._villager;
			}
			set
			{
				this.Replace(this._villager, value);
				this._villager = value;
				base.InvalidateCategoryCache();
			}
		}

		// Token: 0x0400017F RID: 383
		private readonly IFaction _faction = faction;

		// Token: 0x04000180 RID: 384
		private WCharacter _retinueElite;

		// Token: 0x04000181 RID: 385
		private WCharacter _retinueBasic;

		// Token: 0x04000182 RID: 386
		private WCharacter _rootElite;

		// Token: 0x04000183 RID: 387
		private WCharacter _rootBasic;

		// Token: 0x04000184 RID: 388
		private WCharacter _militiaMelee;

		// Token: 0x04000185 RID: 389
		private WCharacter _militiaMeleeElite;

		// Token: 0x04000186 RID: 390
		private WCharacter _militiaRanged;

		// Token: 0x04000187 RID: 391
		private WCharacter _militiaRangedElite;

		// Token: 0x04000188 RID: 392
		private WCharacter _caravanGuard;

		// Token: 0x04000189 RID: 393
		private WCharacter _caravanMaster;

		// Token: 0x0400018A RID: 394
		private WCharacter _villager;
	}
}
