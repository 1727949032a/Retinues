using System;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000EA RID: 234
	public sealed class IronDiscipline : Doctrine
	{
		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06000946 RID: 2374 RVA: 0x0002D172 File Offset: 0x0002B372
		public override TextObject Name
		{
			get
			{
				return L.T("iron_discipline", "Iron Discipline");
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06000947 RID: 2375 RVA: 0x0002D183 File Offset: 0x0002B383
		public override TextObject Description
		{
			get
			{
				return L.T("iron_discipline_description", "+5 to skill caps.");
			}
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06000948 RID: 2376 RVA: 0x0002D194 File Offset: 0x0002B394
		public override int Column
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06000949 RID: 2377 RVA: 0x0002D197 File Offset: 0x0002B397
		public override int Row
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x020001C7 RID: 455
		public sealed class ID_Upgrade100BasicToMax : Feat
		{
			// Token: 0x170004C4 RID: 1220
			// (get) Token: 0x06000D37 RID: 3383 RVA: 0x0003732C File Offset: 0x0003552C
			public override TextObject Description
			{
				get
				{
					return L.T("iron_discipline_upgrade_100_basic_to_max", "Upgrade 100 basic custom troops to max tier.");
				}
			}

			// Token: 0x170004C5 RID: 1221
			// (get) Token: 0x06000D38 RID: 3384 RVA: 0x0003733D File Offset: 0x0003553D
			public override int Target
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06000D39 RID: 3385 RVA: 0x00037341 File Offset: 0x00035541
			public override void PlayerUpgradedTroops(WCharacter upgradeFromTroop, WCharacter upgradeToTroop, int number)
			{
				if (upgradeToTroop.IsElite)
				{
					return;
				}
				if (!upgradeToTroop.IsCustom)
				{
					return;
				}
				if (!upgradeToTroop.IsMaxTier)
				{
					return;
				}
				base.AdvanceProgress(number);
			}
		}

		// Token: 0x020001C8 RID: 456
		public sealed class ID_LeadArmy10Days : Feat
		{
			// Token: 0x170004C6 RID: 1222
			// (get) Token: 0x06000D3B RID: 3387 RVA: 0x0003736E File Offset: 0x0003556E
			public override TextObject Description
			{
				get
				{
					return L.T("iron_discipline_lead_army_10_days", "Lead an army for 10 days in a row.");
				}
			}

			// Token: 0x170004C7 RID: 1223
			// (get) Token: 0x06000D3C RID: 3388 RVA: 0x0003737F File Offset: 0x0003557F
			public override int Target
			{
				get
				{
					return 10;
				}
			}

			// Token: 0x06000D3D RID: 3389 RVA: 0x00037383 File Offset: 0x00035583
			public override void OnDailyTick()
			{
				if (Player.IsArmyLeader)
				{
					base.AdvanceProgress(1);
					return;
				}
				base.SetProgress(0);
			}
		}

		// Token: 0x020001C9 RID: 457
		public sealed class ID_DefeatTwiceSizeOnlyCustom : Feat
		{
			// Token: 0x170004C8 RID: 1224
			// (get) Token: 0x06000D3F RID: 3391 RVA: 0x000373A4 File Offset: 0x000355A4
			public override TextObject Description
			{
				get
				{
					return L.T("iron_discipline_defeat_twice_size_only_custom", "Defeat a party twice your size using only custom troops.");
				}
			}

			// Token: 0x170004C9 RID: 1225
			// (get) Token: 0x06000D40 RID: 3392 RVA: 0x000373B5 File Offset: 0x000355B5
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D41 RID: 3393 RVA: 0x000373B8 File Offset: 0x000355B8
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (battle.EnemyTroopCount < 2 * battle.FriendlyTroopCount)
				{
					return;
				}
				if (Player.Party.MemberRoster.CustomRatio < 0.99f)
				{
					return;
				}
				if (Player.Party.MemberRoster.CustomCount <= 0)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}
	}
}
