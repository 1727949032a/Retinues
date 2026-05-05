using System;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000EE RID: 238
	public sealed class Indomitable : Doctrine
	{
		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x0600095E RID: 2398 RVA: 0x0002D2AA File Offset: 0x0002B4AA
		public override TextObject Name
		{
			get
			{
				return L.T("indomitable", "Indomitable");
			}
		}

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x0002D2BB File Offset: 0x0002B4BB
		public override TextObject Description
		{
			get
			{
				return L.T("indomitable_description", "+5 HP to retinues.");
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x0002D2CC File Offset: 0x0002B4CC
		public override int Column
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06000961 RID: 2401 RVA: 0x0002D2CF File Offset: 0x0002B4CF
		public override int Row
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x020001D3 RID: 467
		public sealed class IND_25EquivNoCasualty : Feat
		{
			// Token: 0x170004DC RID: 1244
			// (get) Token: 0x06000D67 RID: 3431 RVA: 0x00037894 File Offset: 0x00035A94
			public override TextObject Description
			{
				get
				{
					return L.T("indomitable_25_equiv_no_casualty", "Have your retinues defeat 25 enemy troops of equivalent tier without a single casualty.");
				}
			}

			// Token: 0x170004DD RID: 1245
			// (get) Token: 0x06000D68 RID: 3432 RVA: 0x000378A5 File Offset: 0x00035AA5
			public override int Target
			{
				get
				{
					return 25;
				}
			}

			// Token: 0x06000D69 RID: 3433 RVA: 0x000378AC File Offset: 0x00035AAC
			public override void OnBattleEnd(Battle battle)
			{
				foreach (Combat.Kill kill in battle.Kills)
				{
					if (!kill.KillerIsPlayerTroop)
					{
						if (kill.Victim.IsRetinue)
						{
							base.SetProgress(0);
						}
					}
					else if (kill.KillerIsPlayerTroop && kill.Killer.IsRetinue && kill.Victim.Tier >= kill.Killer.Tier)
					{
						base.AdvanceProgress(1);
					}
				}
			}
		}

		// Token: 0x020001D4 RID: 468
		public sealed class IND_JoinSiegeDefenderFullStrength : Feat
		{
			// Token: 0x170004DE RID: 1246
			// (get) Token: 0x06000D6B RID: 3435 RVA: 0x00037958 File Offset: 0x00035B58
			public override TextObject Description
			{
				get
				{
					return L.T("indomitable_join_siege_defender_full_strength", "Fight a siege battle as a defender with at least 20 retinue troops and win.");
				}
			}

			// Token: 0x170004DF RID: 1247
			// (get) Token: 0x06000D6C RID: 3436 RVA: 0x00037969 File Offset: 0x00035B69
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D6D RID: 3437 RVA: 0x0003796C File Offset: 0x00035B6C
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
				if (Player.Party.MemberRoster.RetinueCount < 20)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001D5 RID: 469
		public sealed class IND_RetinueOnly3DefWins : Feat
		{
			// Token: 0x170004E0 RID: 1248
			// (get) Token: 0x06000D6F RID: 3439 RVA: 0x000379AD File Offset: 0x00035BAD
			public override TextObject Description
			{
				get
				{
					return L.T("indomitable_win_3_defensive_battles", "Win 3 defensive battles in a row with a retinue-only party.");
				}
			}

			// Token: 0x170004E1 RID: 1249
			// (get) Token: 0x06000D70 RID: 3440 RVA: 0x000379BE File Offset: 0x00035BBE
			public override int Target
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x06000D71 RID: 3441 RVA: 0x000379C1 File Offset: 0x00035BC1
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					base.SetProgress(0);
					return;
				}
				if (Player.Party.MemberRoster.RetinueRatio < 1f)
				{
					base.SetProgress(0);
					return;
				}
				if (battle.PlayerIsDefender)
				{
					base.AdvanceProgress(1);
				}
			}
		}
	}
}
