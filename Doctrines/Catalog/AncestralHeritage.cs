using System;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E1 RID: 225
	public sealed class AncestralHeritage : Doctrine
	{
		// Token: 0x170003AF RID: 943
		// (get) Token: 0x0600090D RID: 2317 RVA: 0x0002CEBA File Offset: 0x0002B0BA
		public override TextObject Name
		{
			get
			{
				return L.T("ancestral_heritage", "Ancestral Heritage");
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x0600090E RID: 2318 RVA: 0x0002CECB File Offset: 0x0002B0CB
		public override TextObject Description
		{
			get
			{
				return L.T("ancestral_heritage_description", "Unlocks all own culture items.");
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x0600090F RID: 2319 RVA: 0x0002CEDC File Offset: 0x0002B0DC
		public override int Column
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000910 RID: 2320 RVA: 0x0002CEDF File Offset: 0x0002B0DF
		public override int Row
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000911 RID: 2321 RVA: 0x0002CEE2 File Offset: 0x0002B0E2
		public override bool IsDisabled
		{
			get
			{
				return Config.AllCultureEquipmentUnlocked || Config.AllEquipmentUnlocked;
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06000912 RID: 2322 RVA: 0x0002CEFC File Offset: 0x0002B0FC
		public override TextObject DisabledMessage
		{
			get
			{
				if (!Config.AllEquipmentUnlocked)
				{
					return L.T("ancestral_heritage_disabled_message_culture_items", "Disabled: culture items already unlocked by config.");
				}
				return L.T("ancestral_heritage_disabled_message_all_equipment", "Disabled: all equipment already unlocked by config.");
			}
		}

		// Token: 0x020001AC RID: 428
		public sealed class AH_150OutnumberedOwnCulture : Feat
		{
			// Token: 0x1700048E RID: 1166
			// (get) Token: 0x06000CCA RID: 3274 RVA: 0x000361DB File Offset: 0x000343DB
			public override TextObject Description
			{
				get
				{
					return L.T("ancestral_heritage_150_outnumbered_own_culture", "Win a 150+ battle while outnumbered and fielding only custom troops of your own culture.");
				}
			}

			// Token: 0x1700048F RID: 1167
			// (get) Token: 0x06000CCB RID: 3275 RVA: 0x000361EC File Offset: 0x000343EC
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CCC RID: 3276 RVA: 0x000361F0 File Offset: 0x000343F0
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (battle.FriendlyTroopCount >= battle.EnemyTroopCount)
				{
					return;
				}
				if (battle.TotalTroopCount < 150)
				{
					return;
				}
				WRoster memberRoster = Player.Party.MemberRoster;
				WCulture culture = Player.Culture;
				foreach (WRosterElement wrosterElement in memberRoster.Elements)
				{
					if (!wrosterElement.Troop.IsHero)
					{
						if (!wrosterElement.Troop.IsCustom)
						{
							return;
						}
						if (wrosterElement.Troop.Culture != culture)
						{
							return;
						}
					}
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001AD RID: 429
		public sealed class AH_TournamentOwnCultureTown : Feat
		{
			// Token: 0x17000490 RID: 1168
			// (get) Token: 0x06000CCE RID: 3278 RVA: 0x000362AC File Offset: 0x000344AC
			public override TextObject Description
			{
				get
				{
					return L.T("ancestral_heritage_tournament_own_culture_town", "Win a tournament in a town of your own culture.");
				}
			}

			// Token: 0x17000491 RID: 1169
			// (get) Token: 0x06000CCF RID: 3279 RVA: 0x000362BD File Offset: 0x000344BD
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CD0 RID: 3280 RVA: 0x000362C0 File Offset: 0x000344C0
			public override void OnTournamentFinished(Tournament tournament)
			{
				if (tournament.Winner != Player.Character)
				{
					return;
				}
				WSettlement town = tournament.Town;
				if (((town != null) ? town.Culture : null) != Player.Culture)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001AE RID: 430
		public sealed class AH_CaptureOwnCultureFief : Feat
		{
			// Token: 0x17000492 RID: 1170
			// (get) Token: 0x06000CD2 RID: 3282 RVA: 0x00036304 File Offset: 0x00034504
			public override TextObject Description
			{
				get
				{
					return L.T("ancestral_heritage_capture_own_culture_fief", "Capture a fief of your own culture from an enemy kingdom.");
				}
			}

			// Token: 0x17000493 RID: 1171
			// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x00036315 File Offset: 0x00034515
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CD4 RID: 3284 RVA: 0x00036318 File Offset: 0x00034518
			public override void OnSettlementOwnerChanged(SettlementOwnerChange change)
			{
				if (!change.WasCaptured)
				{
					return;
				}
				if (change.NewOwner != Player.Character)
				{
					return;
				}
				WSettlement settlement = change.Settlement;
				if (((settlement != null) ? settlement.Culture : null) != Player.Culture)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}
	}
}
