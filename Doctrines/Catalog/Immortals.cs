using System;
using System.Linq;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000F1 RID: 241
	public sealed class Immortals : Doctrine
	{
		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x0600096F RID: 2415 RVA: 0x0002D374 File Offset: 0x0002B574
		public override TextObject Name
		{
			get
			{
				return L.T("immortals", "Immortals");
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06000970 RID: 2416 RVA: 0x0002D385 File Offset: 0x0002B585
		public override TextObject Description
		{
			get
			{
				return L.T("immortals_description", "+20% retinue survival chance.");
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06000971 RID: 2417 RVA: 0x0002D396 File Offset: 0x0002B596
		public override int Column
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06000972 RID: 2418 RVA: 0x0002D399 File Offset: 0x0002B599
		public override int Row
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x020001DC RID: 476
		public sealed class IM_100RetinueSurviveStruckDown : Feat
		{
			// Token: 0x170004EE RID: 1262
			// (get) Token: 0x06000D8B RID: 3467 RVA: 0x00037CF4 File Offset: 0x00035EF4
			public override TextObject Description
			{
				get
				{
					return L.T("immortals_100_retinue_survive_struck_down", "Have 100 retinue troops survive being struck down in battle.");
				}
			}

			// Token: 0x170004EF RID: 1263
			// (get) Token: 0x06000D8C RID: 3468 RVA: 0x00037D05 File Offset: 0x00035F05
			public override int Target
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06000D8D RID: 3469 RVA: 0x00037D09 File Offset: 0x00035F09
			public override void OnBattleStart(Battle battle)
			{
				Immortals.IM_100RetinueSurviveStruckDown.WoundedAtBattleStart = this.CountWoundedRetinues();
			}

			// Token: 0x06000D8E RID: 3470 RVA: 0x00037D18 File Offset: 0x00035F18
			public override void OnBattleEnd(Battle battle)
			{
				int num = this.CountWoundedRetinues() - Immortals.IM_100RetinueSurviveStruckDown.WoundedAtBattleStart;
				if (num > 0)
				{
					base.AdvanceProgress(num);
				}
			}

			// Token: 0x06000D8F RID: 3471 RVA: 0x00037D40 File Offset: 0x00035F40
			private int CountWoundedRetinues()
			{
				int num = 0;
				foreach (WRosterElement wrosterElement in Player.Party.MemberRoster.Elements)
				{
					if (wrosterElement.Troop.IsRetinue)
					{
						num += wrosterElement.WoundedNumber;
					}
				}
				return num;
			}

			// Token: 0x0400051F RID: 1311
			private static int WoundedAtBattleStart;
		}

		// Token: 0x020001DD RID: 477
		public sealed class IM_Win100NoDeaths : Feat
		{
			// Token: 0x170004F0 RID: 1264
			// (get) Token: 0x06000D91 RID: 3473 RVA: 0x00037DB0 File Offset: 0x00035FB0
			public override TextObject Description
			{
				get
				{
					return L.T("immortals_win_100_no_deaths", "Win by yourself against 100+ enemies without a single death on your side.");
				}
			}

			// Token: 0x170004F1 RID: 1265
			// (get) Token: 0x06000D92 RID: 3474 RVA: 0x00037DC1 File Offset: 0x00035FC1
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D93 RID: 3475 RVA: 0x00037DC4 File Offset: 0x00035FC4
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (battle.EnemyTroopCount < 100)
				{
					return;
				}
				if (battle.AllyTroopCount > 0)
				{
					return;
				}
				foreach (Combat.Kill kill in battle.Kills)
				{
					if (kill.State == AgentState.Killed && kill.VictimIsPlayerTroop)
					{
						return;
					}
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001DE RID: 478
		public sealed class IM_Retinue200Enemies : Feat
		{
			// Token: 0x170004F2 RID: 1266
			// (get) Token: 0x06000D95 RID: 3477 RVA: 0x00037E54 File Offset: 0x00036054
			public override TextObject Description
			{
				get
				{
					return L.T("immortals_retinue_200_enemies", "Have your retinue defeat 200 enemies in a single battle.");
				}
			}

			// Token: 0x170004F3 RID: 1267
			// (get) Token: 0x06000D96 RID: 3478 RVA: 0x00037E65 File Offset: 0x00036065
			public override int Target
			{
				get
				{
					return 200;
				}
			}

			// Token: 0x06000D97 RID: 3479 RVA: 0x00037E6C File Offset: 0x0003606C
			public override void OnBattleEnd(Battle battle)
			{
				int num = battle.Kills.Count((Combat.Kill kill) => kill.KillerIsPlayerTroop && kill.Killer.IsRetinue);
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
			}
		}
	}
}
