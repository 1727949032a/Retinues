using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E7 RID: 231
	public sealed class RoadWardens : Doctrine
	{
		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06000933 RID: 2355 RVA: 0x0002D0A8 File Offset: 0x0002B2A8
		public override TextObject Name
		{
			get
			{
				return L.T("road_wardens", "Road Wardens");
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06000934 RID: 2356 RVA: 0x0002D0B9 File Offset: 0x0002B2B9
		public override TextObject Description
		{
			get
			{
				return L.T("road_wardens_description", "Unlocks caravan troops.");
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06000935 RID: 2357 RVA: 0x0002D0CA File Offset: 0x0002B2CA
		public override int Column
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06000936 RID: 2358 RVA: 0x0002D0CD File Offset: 0x0002B2CD
		public override int Row
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x0002D0D0 File Offset: 0x0002B2D0
		public override bool IsDisabled
		{
			get
			{
				return Config.NoDoctrineRequirements;
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06000938 RID: 2360 RVA: 0x0002D0DC File Offset: 0x0002B2DC
		public override TextObject DisabledMessage
		{
			get
			{
				return L.T("road_wardens_disabled_message", "Disabled: special troops unlocked from config.");
			}
		}

		// Token: 0x020001BE RID: 446
		public sealed class RW_OwnThreeCaravans : Feat
		{
			// Token: 0x170004B2 RID: 1202
			// (get) Token: 0x06000D12 RID: 3346 RVA: 0x00036E6E File Offset: 0x0003506E
			public override TextObject Description
			{
				get
				{
					return L.T("road_wardens_three_caravans", "Own three caravans at the same time.");
				}
			}

			// Token: 0x170004B3 RID: 1203
			// (get) Token: 0x06000D13 RID: 3347 RVA: 0x00036E7F File Offset: 0x0003507F
			public override int Target
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x06000D14 RID: 3348 RVA: 0x00036E84 File Offset: 0x00035084
			public override void OnDailyTick()
			{
				WFaction clan = Player.Clan;
				IEnumerable<string> enumerable;
				if (clan == null)
				{
					enumerable = null;
				}
				else
				{
					MBReadOnlyList<Hero> heroes = clan.Base.Heroes;
					if (heroes == null)
					{
						enumerable = null;
					}
					else
					{
						enumerable = from h in heroes
						select h.StringId;
					}
				}
				IEnumerable<string> enumerable2 = enumerable;
				if (enumerable2 != null && enumerable2.Count<string>() == 0)
				{
					return;
				}
				int num = 0;
				foreach (WParty wparty in WParty.All)
				{
					if (wparty.IsCaravan && !(wparty.Leader == null) && enumerable2.Contains(wparty.Leader.StringId))
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

		// Token: 0x020001BF RID: 447
		public sealed class RW_MerchantTownQuests : Feat
		{
			// Token: 0x170004B4 RID: 1204
			// (get) Token: 0x06000D16 RID: 3350 RVA: 0x00036F60 File Offset: 0x00035160
			public override TextObject Description
			{
				get
				{
					return L.T("road_wardens_merchant_town_quests", "Complete 3 quests for a merchant from one of your towns.");
				}
			}

			// Token: 0x170004B5 RID: 1205
			// (get) Token: 0x06000D17 RID: 3351 RVA: 0x00036F71 File Offset: 0x00035171
			public override int Target
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x06000D18 RID: 3352 RVA: 0x00036F74 File Offset: 0x00035174
			public override void OnQuestCompleted(Quest quest)
			{
				if (!quest.IsSuccessful)
				{
					return;
				}
				WHero giver = quest.Giver;
				Hero hero = (giver != null) ? giver.Hero : null;
				if (hero == null)
				{
					return;
				}
				Settlement settlement = hero.CurrentSettlement ?? hero.HomeSettlement;
				if (settlement == null || !settlement.IsTown)
				{
					return;
				}
				if (settlement.OwnerClan != Clan.PlayerClan)
				{
					return;
				}
				if (!hero.IsMerchant)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001C0 RID: 448
		public sealed class RW_ClearHideout : Feat
		{
			// Token: 0x170004B6 RID: 1206
			// (get) Token: 0x06000D1A RID: 3354 RVA: 0x00036FE5 File Offset: 0x000351E5
			public override TextObject Description
			{
				get
				{
					return L.T("road_wardens_clear_hideout", "Clear a bandit hideout.");
				}
			}

			// Token: 0x170004B7 RID: 1207
			// (get) Token: 0x06000D1B RID: 3355 RVA: 0x00036FF6 File Offset: 0x000351F6
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D1C RID: 3356 RVA: 0x00036FF9 File Offset: 0x000351F9
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (!battle.IsHideout)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}
	}
}
