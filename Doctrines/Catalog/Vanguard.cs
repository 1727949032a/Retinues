using System;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000F0 RID: 240
	public sealed class Vanguard : Doctrine
	{
		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06000968 RID: 2408 RVA: 0x0002D30A File Offset: 0x0002B50A
		public override TextObject Name
		{
			get
			{
				return L.T("vanguard", "Vanguard");
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06000969 RID: 2409 RVA: 0x0002D31B File Offset: 0x0002B51B
		public override TextObject Description
		{
			get
			{
				return L.T("vanguard_description", "+15% retinue cap.");
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x0600096A RID: 2410 RVA: 0x0002D32C File Offset: 0x0002B52C
		public override int Column
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x0600096B RID: 2411 RVA: 0x0002D32F File Offset: 0x0002B52F
		public override int Row
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x0600096C RID: 2412 RVA: 0x0002D332 File Offset: 0x0002B532
		public override bool IsDisabled
		{
			get
			{
				return Config.MaxBasicRetinueRatio >= 0.85f && Config.MaxEliteRetinueRatio >= 0.85f;
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x0600096D RID: 2413 RVA: 0x0002D35B File Offset: 0x0002B55B
		public override TextObject DisabledMessage
		{
			get
			{
				return L.T("vanguard_disabled_message", "Disabled: retinue cap already maxed in config.");
			}
		}

		// Token: 0x020001D9 RID: 473
		public sealed class VG_ClearHideoutRetinueOnly : Feat
		{
			// Token: 0x170004E8 RID: 1256
			// (get) Token: 0x06000D7F RID: 3455 RVA: 0x00037B2D File Offset: 0x00035D2D
			public override TextObject Description
			{
				get
				{
					return L.T("vanguard_clear_hideout_retinue_only", "Clear a hideout using only your retinue.");
				}
			}

			// Token: 0x170004E9 RID: 1257
			// (get) Token: 0x06000D80 RID: 3456 RVA: 0x00037B3E File Offset: 0x00035D3E
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D81 RID: 3457 RVA: 0x00037B44 File Offset: 0x00035D44
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
				foreach (Combat.Kill kill in battle.Kills)
				{
					if (!kill.KillerIsPlayer && (!kill.KillerIsPlayerTroop || !kill.Killer.IsRetinue) && !kill.VictimIsPlayer && (!kill.VictimIsPlayerTroop || !kill.Victim.IsRetinue))
					{
						return;
					}
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001DA RID: 474
		public sealed class VG_Win100RetinueOnly : Feat
		{
			// Token: 0x170004EA RID: 1258
			// (get) Token: 0x06000D83 RID: 3459 RVA: 0x00037BF0 File Offset: 0x00035DF0
			public override TextObject Description
			{
				get
				{
					return L.T("vanguard_win_100_retinue_only", "Win a 100+ battle using only your retinue.");
				}
			}

			// Token: 0x170004EB RID: 1259
			// (get) Token: 0x06000D84 RID: 3460 RVA: 0x00037C01 File Offset: 0x00035E01
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D85 RID: 3461 RVA: 0x00037C04 File Offset: 0x00035E04
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (battle.TotalTroopCount < 100)
				{
					return;
				}
				if (battle.AllyTroopCount > 0)
				{
					return;
				}
				if (Player.Party.MemberRoster.RetinueRatio < 1f)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001DB RID: 475
		public sealed class VG_FirstMeleeKillInSiege : Feat
		{
			// Token: 0x170004EC RID: 1260
			// (get) Token: 0x06000D87 RID: 3463 RVA: 0x00037C4B File Offset: 0x00035E4B
			public override TextObject Description
			{
				get
				{
					return L.T("vanguard_first_melee_kill_in_siege", "Have a retinue get the first melee kill in a siege assault.");
				}
			}

			// Token: 0x170004ED RID: 1261
			// (get) Token: 0x06000D88 RID: 3464 RVA: 0x00037C5C File Offset: 0x00035E5C
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D89 RID: 3465 RVA: 0x00037C60 File Offset: 0x00035E60
			public override void OnBattleEnd(Battle battle)
			{
				if (!battle.IsSiege)
				{
					return;
				}
				if (battle.PlayerIsDefender)
				{
					return;
				}
				foreach (Combat.Kill kill in battle.Kills)
				{
					if (!kill.IsMissile)
					{
						if (!kill.KillerIsPlayerTroop)
						{
							break;
						}
						if (!kill.Killer.IsRetinue)
						{
							break;
						}
						base.AdvanceProgress(1);
						break;
					}
				}
			}
		}
	}
}
