using System;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game.Events;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000DE RID: 222
	public sealed class LionsShare : Doctrine
	{
		// Token: 0x1700039D RID: 925
		// (get) Token: 0x060008F8 RID: 2296 RVA: 0x0002CD55 File Offset: 0x0002AF55
		public override TextObject Name
		{
			get
			{
				return L.T("lions_share", "Lion's Share");
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x0002CD66 File Offset: 0x0002AF66
		public override TextObject Description
		{
			get
			{
				return L.T("lions_share_description", "Hero kills count twice for unlocks.");
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x0002CD77 File Offset: 0x0002AF77
		public override int Column
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x060008FB RID: 2299 RVA: 0x0002CD7A File Offset: 0x0002AF7A
		public override int Row
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x0002CD7D File Offset: 0x0002AF7D
		public override bool IsDisabled
		{
			get
			{
				return !Config.UnlockItemsFromKills || Config.AllEquipmentUnlocked;
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x060008FD RID: 2301 RVA: 0x0002CD97 File Offset: 0x0002AF97
		public override TextObject DisabledMessage
		{
			get
			{
				if (!Config.AllEquipmentUnlocked)
				{
					return L.T("lions_share_disabled_message_unlocks_from_kills", "Disabled: unlocks from kills disabled by config.");
				}
				return L.T("lions_share_disabled_message_all_equipment", "Disabled: all equipment already unlocked by config.");
			}
		}

		// Token: 0x020001A3 RID: 419
		public sealed class LS_25PersonalKills : Feat
		{
			// Token: 0x1700047C RID: 1148
			// (get) Token: 0x06000CA4 RID: 3236 RVA: 0x00035E07 File Offset: 0x00034007
			public override TextObject Description
			{
				get
				{
					return L.T("lions_share_25_personal_kills", "Personally defeat 25 enemies in one battle.");
				}
			}

			// Token: 0x1700047D RID: 1149
			// (get) Token: 0x06000CA5 RID: 3237 RVA: 0x00035E18 File Offset: 0x00034018
			public override int Target
			{
				get
				{
					return 25;
				}
			}

			// Token: 0x06000CA6 RID: 3238 RVA: 0x00035E1C File Offset: 0x0003401C
			public override void OnBattleEnd(Battle battle)
			{
				int num = battle.Kills.Count((Combat.Kill k) => k.KillerIsPlayer);
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
			}
		}

		// Token: 0x020001A4 RID: 420
		public sealed class LS_5Tier5Plus : Feat
		{
			// Token: 0x1700047E RID: 1150
			// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x00035E6C File Offset: 0x0003406C
			public override TextObject Description
			{
				get
				{
					return L.T("lions_share_5_tier_5_plus", "Personally defeat 5 tier 5+ troops in one battle.");
				}
			}

			// Token: 0x1700047F RID: 1151
			// (get) Token: 0x06000CA9 RID: 3241 RVA: 0x00035E7D File Offset: 0x0003407D
			public override int Target
			{
				get
				{
					return 5;
				}
			}

			// Token: 0x06000CAA RID: 3242 RVA: 0x00035E80 File Offset: 0x00034080
			public override void OnBattleEnd(Battle battle)
			{
				int num = battle.Kills.Count((Combat.Kill k) => k.KillerIsPlayer && k.Victim.Tier >= 5);
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
			}
		}

		// Token: 0x020001A5 RID: 421
		public sealed class LS_KillEnemyLord : Feat
		{
			// Token: 0x17000480 RID: 1152
			// (get) Token: 0x06000CAC RID: 3244 RVA: 0x00035ED0 File Offset: 0x000340D0
			public override TextObject Description
			{
				get
				{
					return L.T("lions_share_kill_enemy_lord", "Personally defeat an enemy lord in battle.");
				}
			}

			// Token: 0x17000481 RID: 1153
			// (get) Token: 0x06000CAD RID: 3245 RVA: 0x00035EE1 File Offset: 0x000340E1
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CAE RID: 3246 RVA: 0x00035EE4 File Offset: 0x000340E4
			public override void OnBattleEnd(Battle battle)
			{
				int num = battle.Kills.Count((Combat.Kill k) => k.KillerIsPlayer && k.Victim.IsHero);
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
			}
		}
	}
}
