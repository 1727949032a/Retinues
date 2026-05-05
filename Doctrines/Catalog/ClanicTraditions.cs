using System;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E3 RID: 227
	public sealed class ClanicTraditions : Doctrine
	{
		// Token: 0x170003BB RID: 955
		// (get) Token: 0x0600091B RID: 2331 RVA: 0x0002CF96 File Offset: 0x0002B196
		public override TextObject Name
		{
			get
			{
				return L.T("clan_traditions", "Clan Traditions");
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x0600091C RID: 2332 RVA: 0x0002CFA7 File Offset: 0x0002B1A7
		public override TextObject Description
		{
			get
			{
				return L.T("clan_traditions_description", "Troops can equip smithed weapons.");
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x0600091D RID: 2333 RVA: 0x0002CFB8 File Offset: 0x0002B1B8
		public override int Column
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x0600091E RID: 2334 RVA: 0x0002CFBB File Offset: 0x0002B1BB
		public override int Row
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x020001B2 RID: 434
		public sealed class CT_OwnSmithy30Days : Feat
		{
			// Token: 0x1700049A RID: 1178
			// (get) Token: 0x06000CE2 RID: 3298 RVA: 0x00036794 File Offset: 0x00034994
			public override TextObject Description
			{
				get
				{
					return L.T("clan_traditions_own_smithy_30_days", "Own a smithy in a town of your clan's culture for 30 days.");
				}
			}

			// Token: 0x1700049B RID: 1179
			// (get) Token: 0x06000CE3 RID: 3299 RVA: 0x000367A5 File Offset: 0x000349A5
			public override int Target
			{
				get
				{
					return 30;
				}
			}

			// Token: 0x06000CE4 RID: 3300 RVA: 0x000367AC File Offset: 0x000349AC
			public override void OnDailyTick()
			{
				Hero mainHero = Hero.MainHero;
				foreach (Workshop workshop in ((mainHero != null) ? mainHero.OwnedWorkshops : null))
				{
					if (workshop != null && workshop.Settlement != null)
					{
						WorkshopType workshopType = workshop.WorkshopType;
						if ((workshopType == null || workshopType.StringId.Contains("smith")) && !(new WCulture(workshop.Settlement.Culture) != Player.Culture))
						{
							base.AdvanceProgress(1);
							return;
						}
					}
				}
				base.SetProgress(0);
			}
		}

		// Token: 0x020001B3 RID: 435
		public sealed class CT_Companions50Kills : Feat
		{
			// Token: 0x1700049C RID: 1180
			// (get) Token: 0x06000CE6 RID: 3302 RVA: 0x00036864 File Offset: 0x00034A64
			public override TextObject Description
			{
				get
				{
					return L.T("clan_traditions_companions_50_kills", "Win a battle in which you and your companions get 50 or more kills.");
				}
			}

			// Token: 0x1700049D RID: 1181
			// (get) Token: 0x06000CE7 RID: 3303 RVA: 0x00036875 File Offset: 0x00034A75
			public override int Target
			{
				get
				{
					return 50;
				}
			}

			// Token: 0x06000CE8 RID: 3304 RVA: 0x0003687C File Offset: 0x00034A7C
			public override void OnBattleEnd(Battle battle)
			{
				int num = 0;
				foreach (Combat.Kill kill in battle.Kills)
				{
					if (kill.KillerIsPlayer || (kill.KillerIsPlayerTroop && kill.Killer.IsHero))
					{
						num++;
					}
				}
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
			}
		}

		// Token: 0x020001B4 RID: 436
		public sealed class CT_AcquireNewFief : Feat
		{
			// Token: 0x1700049E RID: 1182
			// (get) Token: 0x06000CEA RID: 3306 RVA: 0x00036904 File Offset: 0x00034B04
			public override TextObject Description
			{
				get
				{
					return L.T("clan_traditions_acquire_new_fief", "Acquire a new fief for your clan.");
				}
			}

			// Token: 0x1700049F RID: 1183
			// (get) Token: 0x06000CEB RID: 3307 RVA: 0x00036915 File Offset: 0x00034B15
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CEC RID: 3308 RVA: 0x00036918 File Offset: 0x00034B18
			public override void OnSettlementOwnerChanged(SettlementOwnerChange change)
			{
				if (change.NewOwner.Clan != Player.Clan)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}
	}
}
