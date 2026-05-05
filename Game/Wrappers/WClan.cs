using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Localization;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000091 RID: 145
	[SafeClass]
	public class WClan : BaseBannerFaction
	{
		// Token: 0x060005B0 RID: 1456 RVA: 0x0001DBB1 File Offset: 0x0001BDB1
		public WClan(Clan clan)
		{
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x060005B1 RID: 1457 RVA: 0x0001DBC0 File Offset: 0x0001BDC0
		public static IEnumerable<WClan> All
		{
			get
			{
				return new WClan.<get_All>d__2(-2);
			}
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x0001DBC9 File Offset: 0x0001BDC9
		public Clan Base
		{
			get
			{
				return this._clan;
			}
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x060005B3 RID: 1459 RVA: 0x0001DBD1 File Offset: 0x0001BDD1
		public override string Name
		{
			get
			{
				Clan @base = this.Base;
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

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x060005B4 RID: 1460 RVA: 0x0001DBEF File Offset: 0x0001BDEF
		public override string StringId
		{
			get
			{
				Clan @base = this.Base;
				return ((@base != null) ? @base.StringId : null) ?? this.Name;
			}
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x060005B5 RID: 1461 RVA: 0x0001DC0D File Offset: 0x0001BE0D
		public override uint Color
		{
			get
			{
				Clan @base = this.Base;
				if (@base == null)
				{
					return 0U;
				}
				return @base.Color;
			}
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x0001DC20 File Offset: 0x0001BE20
		public override uint Color2
		{
			get
			{
				Clan @base = this.Base;
				if (@base == null)
				{
					return 0U;
				}
				return @base.Color2;
			}
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x060005B7 RID: 1463 RVA: 0x0001DC33 File Offset: 0x0001BE33
		public bool IsPlayerClan
		{
			get
			{
				return this.Base != null && this.Base.StringId == Clan.PlayerClan.StringId;
			}
		}

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x0001DC59 File Offset: 0x0001BE59
		public override Banner BaseBanner
		{
			get
			{
				Clan @base = this.Base;
				if (@base == null)
				{
					return null;
				}
				return @base.Banner;
			}
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x060005B9 RID: 1465 RVA: 0x0001DC6C File Offset: 0x0001BE6C
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

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x0001DC8E File Offset: 0x0001BE8E
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

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x060005BB RID: 1467 RVA: 0x0001DCB0 File Offset: 0x0001BEB0
		public override WCharacter RootBasic
		{
			get
			{
				if (this.IsPlayerClan)
				{
					return null;
				}
				CharacterObject basicTroop = this.Base.BasicTroop;
				if (basicTroop.StringId == base.Culture.RootBasic.StringId)
				{
					return null;
				}
				if (basicTroop == null)
				{
					return null;
				}
				return new WCharacter(basicTroop);
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x0001DD00 File Offset: 0x0001BF00
		public override List<WHero> Heroes
		{
			get
			{
				List<WHero> list = new List<WHero>();
				foreach (Hero hero in this.Base.Heroes)
				{
					if (hero != null)
					{
						WHero whero = new WHero(hero);
						if (whero.IsValid && !whero.HiddenInEncyclopedia)
						{
							if (whero.Skills.Sum((KeyValuePair<SkillObject, int> kv) => kv.Value) != 0)
							{
								list.Add(whero);
							}
						}
					}
				}
				return list;
			}
		}

		// Token: 0x04000179 RID: 377
		private readonly Clan _clan = clan;
	}
}
