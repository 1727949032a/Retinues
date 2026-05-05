using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game.Events;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E6 RID: 230
	public sealed class StalwartMilitia : Doctrine
	{
		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x0600092C RID: 2348 RVA: 0x0002D05B File Offset: 0x0002B25B
		public override TextObject Name
		{
			get
			{
				return L.T("stalwart_militia", "Stalwart Militia");
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x0600092D RID: 2349 RVA: 0x0002D06C File Offset: 0x0002B26C
		public override TextObject Description
		{
			get
			{
				return L.T("stalwart_militia_description", "Unlocks militia troops.");
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x0600092E RID: 2350 RVA: 0x0002D07D File Offset: 0x0002B27D
		public override int Column
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x0002D080 File Offset: 0x0002B280
		public override int Row
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06000930 RID: 2352 RVA: 0x0002D083 File Offset: 0x0002B283
		public override bool IsDisabled
		{
			get
			{
				return Config.NoDoctrineRequirements;
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x0002D08F File Offset: 0x0002B28F
		public override TextObject DisabledMessage
		{
			get
			{
				return L.T("stalwart_militia_disabled_message", "Disabled: special troops unlocked from config.");
			}
		}

		// Token: 0x020001BB RID: 443
		public sealed class SM_DefendCityFromSiege : Feat
		{
			// Token: 0x170004AC RID: 1196
			// (get) Token: 0x06000D06 RID: 3334 RVA: 0x00036CEC File Offset: 0x00034EEC
			public override TextObject Description
			{
				get
				{
					return L.T("stalwart_militia_defend_city_from_siege", "Defend a city against a besieging enemy army.");
				}
			}

			// Token: 0x170004AD RID: 1197
			// (get) Token: 0x06000D07 RID: 3335 RVA: 0x00036CFD File Offset: 0x00034EFD
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D08 RID: 3336 RVA: 0x00036D00 File Offset: 0x00034F00
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (!battle.IsSiege)
				{
					return;
				}
				if (!battle.PlayerIsDefender)
				{
					return;
				}
				if (!battle.EnemyIsInArmy)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001BC RID: 444
		public sealed class SM_PlayerKillsInSiegeDefense : Feat
		{
			// Token: 0x170004AE RID: 1198
			// (get) Token: 0x06000D0A RID: 3338 RVA: 0x00036D36 File Offset: 0x00034F36
			public override TextObject Description
			{
				get
				{
					return L.T("stalwart_militia_player_kill_50_siegers", "Personally slay 50 assailants during a siege defense.");
				}
			}

			// Token: 0x170004AF RID: 1199
			// (get) Token: 0x06000D0B RID: 3339 RVA: 0x00036D47 File Offset: 0x00034F47
			public override int Target
			{
				get
				{
					return 50;
				}
			}

			// Token: 0x06000D0C RID: 3340 RVA: 0x00036D4C File Offset: 0x00034F4C
			public override void OnBattleEnd(Battle battle)
			{
				if (!battle.IsSiege)
				{
					return;
				}
				if (!battle.PlayerIsDefender)
				{
					return;
				}
				if (battle.IsLost)
				{
					return;
				}
				int num = battle.Kills.Count((Combat.Kill k) => k.KillerIsPlayer);
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
			}
		}

		// Token: 0x020001BD RID: 445
		public sealed class SM_RaiseMilitiaTo400 : Feat
		{
			// Token: 0x170004B0 RID: 1200
			// (get) Token: 0x06000D0E RID: 3342 RVA: 0x00036DB7 File Offset: 0x00034FB7
			public override TextObject Description
			{
				get
				{
					return L.T("stalwart_militia_raise_militia_400", "Raise the militia value of a fief to 400.");
				}
			}

			// Token: 0x170004B1 RID: 1201
			// (get) Token: 0x06000D0F RID: 3343 RVA: 0x00036DC8 File Offset: 0x00034FC8
			public override int Target
			{
				get
				{
					return 400;
				}
			}

			// Token: 0x06000D10 RID: 3344 RVA: 0x00036DD0 File Offset: 0x00034FD0
			public override void OnDailyTick()
			{
				if (Campaign.Current == null || Clan.PlayerClan == null)
				{
					return;
				}
				List<Settlement> list = (from s in Clan.PlayerClan.Settlements
				where s.IsTown || s.IsCastle
				select s).ToList<Settlement>();
				if (list.Count == 0)
				{
					return;
				}
				int progress = (int)(from s in list
				select s.Militia).DefaultIfEmpty(0f).Max();
				base.SetProgress(progress);
			}
		}
	}
}
