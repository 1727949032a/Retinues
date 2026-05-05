using System;
using System.Collections.Generic;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Localization;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000092 RID: 146
	[SafeClass]
	public class WCulture : BaseBannerFaction
	{
		// Token: 0x060005BD RID: 1469 RVA: 0x0001DDA8 File Offset: 0x0001BFA8
		public WCulture(CultureObject culture)
		{
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x060005BE RID: 1470 RVA: 0x0001DDB7 File Offset: 0x0001BFB7
		public static IEnumerable<WCulture> All
		{
			get
			{
				return new WCulture.<get_All>d__2(-2);
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0001DDC0 File Offset: 0x0001BFC0
		public CultureObject Base
		{
			get
			{
				return this._culture;
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x060005C0 RID: 1472 RVA: 0x0001DDC8 File Offset: 0x0001BFC8
		public override string Name
		{
			get
			{
				CultureObject @base = this.Base;
				if (@base == null)
				{
					return null;
				}
				TextObject name = @base.Name;
				if (name == null)
				{
					return null;
				}
				return name.ToString();
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0001DDE6 File Offset: 0x0001BFE6
		public override string StringId
		{
			get
			{
				CultureObject @base = this.Base;
				return ((@base != null) ? @base.StringId : null) ?? this.Name;
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x060005C2 RID: 1474 RVA: 0x0001DE04 File Offset: 0x0001C004
		public override uint Color
		{
			get
			{
				CultureObject @base = this.Base;
				if (@base == null)
				{
					return 0U;
				}
				return @base.Color;
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x0001DE17 File Offset: 0x0001C017
		public override uint Color2
		{
			get
			{
				CultureObject @base = this.Base;
				if (@base == null)
				{
					return 0U;
				}
				return @base.Color2;
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x060005C4 RID: 1476 RVA: 0x0001DE2A File Offset: 0x0001C02A
		public override Banner BaseBanner
		{
			get
			{
				if (this.Base == null)
				{
					return null;
				}
				return this.Base.Banner;
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x0001DE41 File Offset: 0x0001C041
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

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x0001DE63 File Offset: 0x0001C063
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

		// Token: 0x060005C7 RID: 1479 RVA: 0x0001DE85 File Offset: 0x0001C085
		public WCharacter TryGet(CharacterObject co)
		{
			if (co == null)
			{
				return null;
			}
			return new WCharacter(co);
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x060005C8 RID: 1480 RVA: 0x0001DE92 File Offset: 0x0001C092
		public override WCharacter RootBasic
		{
			get
			{
				return this.TryGet(this.Base.BasicTroop);
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x0001DEA5 File Offset: 0x0001C0A5
		public override WCharacter RootElite
		{
			get
			{
				return this.TryGet(this.Base.EliteBasicTroop);
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060005CA RID: 1482 RVA: 0x0001DEB8 File Offset: 0x0001C0B8
		public override WCharacter MilitiaMelee
		{
			get
			{
				return this.TryGet(this.Base.MeleeMilitiaTroop);
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x0001DECB File Offset: 0x0001C0CB
		public override WCharacter MilitiaMeleeElite
		{
			get
			{
				return this.TryGet(this.Base.MeleeEliteMilitiaTroop);
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x060005CC RID: 1484 RVA: 0x0001DEDE File Offset: 0x0001C0DE
		public override WCharacter MilitiaRanged
		{
			get
			{
				return this.TryGet(this.Base.RangedMilitiaTroop);
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x0001DEF1 File Offset: 0x0001C0F1
		public override WCharacter MilitiaRangedElite
		{
			get
			{
				return this.TryGet(this.Base.RangedEliteMilitiaTroop);
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x060005CE RID: 1486 RVA: 0x0001DF04 File Offset: 0x0001C104
		public override WCharacter Villager
		{
			get
			{
				return this.TryGet(this.Base.Villager);
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x060005CF RID: 1487 RVA: 0x0001DF17 File Offset: 0x0001C117
		public override WCharacter CaravanMaster
		{
			get
			{
				return this.TryGet(this.Base.CaravanMaster);
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x060005D0 RID: 1488 RVA: 0x0001DF2A File Offset: 0x0001C12A
		public override WCharacter CaravanGuard
		{
			get
			{
				return this.TryGet(this.Base.CaravanGuard);
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x0001DF40 File Offset: 0x0001C140
		public override List<WCharacter> MercenaryTroops
		{
			get
			{
				List<WCharacter> list = new List<WCharacter>();
				foreach (CharacterObject co in this.Base.BasicMercenaryTroops)
				{
					WCharacter wcharacter = this.TryGet(co);
					if (wcharacter != null)
					{
						foreach (WCharacter item in wcharacter.Tree)
						{
							list.Add(item);
						}
					}
				}
				return list;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x060005D2 RID: 1490 RVA: 0x0001DFEC File Offset: 0x0001C1EC
		public override List<WCharacter> BanditTroops
		{
			get
			{
				return base.GetActiveList(new List<CharacterObject>(4)
				{
					this.Base.BanditBandit,
					this.Base.BanditChief,
					this.Base.BanditBoss,
					this.Base.BanditRaider
				});
			}
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0001E04C File Offset: 0x0001C24C
		public override List<WCharacter> CivilianTroops
		{
			get
			{
				return base.GetActiveList(new List<CharacterObject>(35)
				{
					this.Base.PrisonGuard,
					this.Base.Guard,
					this.Base.Blacksmith,
					this.Base.Weaponsmith,
					this.Base.Townswoman,
					this.Base.TownswomanInfant,
					this.Base.TownswomanChild,
					this.Base.TownswomanTeenager,
					this.Base.VillageWoman,
					this.Base.VillagerMaleChild,
					this.Base.VillagerMaleTeenager,
					this.Base.VillagerFemaleChild,
					this.Base.VillagerFemaleTeenager,
					this.Base.Townsman,
					this.Base.TownsmanInfant,
					this.Base.TownsmanChild,
					this.Base.TownsmanTeenager,
					this.Base.RansomBroker,
					this.Base.GangleaderBodyguard,
					this.Base.MerchantNotary,
					this.Base.ArtisanNotary,
					this.Base.PreacherNotary,
					this.Base.RuralNotableNotary,
					this.Base.ShopWorker,
					this.Base.Tavernkeeper,
					this.Base.TavernGamehost,
					this.Base.Musician,
					this.Base.TavernWench,
					this.Base.Armorer,
					this.Base.HorseMerchant,
					this.Base.Barber,
					this.Base.Merchant,
					this.Base.Beggar,
					this.Base.FemaleBeggar,
					this.Base.FemaleDancer
				});
			}
		}

		// Token: 0x0400017A RID: 378
		private readonly CultureObject _culture = culture;
	}
}
