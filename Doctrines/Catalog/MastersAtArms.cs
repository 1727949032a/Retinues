using System;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000EC RID: 236
	public sealed class MastersAtArms : Doctrine
	{
		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06000950 RID: 2384 RVA: 0x0002D1D2 File Offset: 0x0002B3D2
		public override TextObject Name
		{
			get
			{
				return L.T("masters_at_arms", "Masters-At-Arms");
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06000951 RID: 2385 RVA: 0x0002D1E3 File Offset: 0x0002B3E3
		public override TextObject Description
		{
			get
			{
				return L.T("masters_at_arms_description", "+1 upgrade branch for elite troops.");
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06000952 RID: 2386 RVA: 0x0002D1F4 File Offset: 0x0002B3F4
		public override int Column
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06000953 RID: 2387 RVA: 0x0002D1F7 File Offset: 0x0002B3F7
		public override int Row
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06000954 RID: 2388 RVA: 0x0002D1FA File Offset: 0x0002B3FA
		public override bool IsDisabled
		{
			get
			{
				return Config.MaxEliteUpgrades == 4;
			}
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06000955 RID: 2389 RVA: 0x0002D209 File Offset: 0x0002B409
		public override TextObject DisabledMessage
		{
			get
			{
				return L.T("masters_at_arms_disabled_message", "Disabled: elite upgrades already maxed in config.");
			}
		}

		// Token: 0x020001CD RID: 461
		public sealed class MAA_Upgrade100EliteToMax : Feat
		{
			// Token: 0x170004D0 RID: 1232
			// (get) Token: 0x06000D4F RID: 3407 RVA: 0x0003759E File Offset: 0x0003579E
			public override TextObject Description
			{
				get
				{
					return L.T("masters_at_arms_upgrade_100_elite_to_max", "Upgrade 100 elite troops to max tier.");
				}
			}

			// Token: 0x170004D1 RID: 1233
			// (get) Token: 0x06000D50 RID: 3408 RVA: 0x000375AF File Offset: 0x000357AF
			public override int Target
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06000D51 RID: 3409 RVA: 0x000375B3 File Offset: 0x000357B3
			public override void PlayerUpgradedTroops(WCharacter upgradeFromTroop, WCharacter upgradeToTroop, int number)
			{
				if (!upgradeToTroop.IsElite)
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

		// Token: 0x020001CE RID: 462
		public sealed class MAA_KO50Opponents : Feat
		{
			// Token: 0x170004D2 RID: 1234
			// (get) Token: 0x06000D53 RID: 3411 RVA: 0x000375D7 File Offset: 0x000357D7
			public override TextObject Description
			{
				get
				{
					return L.T("masters_at_arms_ko_50_opponents", "Knock out 50 opponents in the arena.");
				}
			}

			// Token: 0x170004D3 RID: 1235
			// (get) Token: 0x06000D54 RID: 3412 RVA: 0x000375E8 File Offset: 0x000357E8
			public override int Target
			{
				get
				{
					return 50;
				}
			}

			// Token: 0x06000D55 RID: 3413 RVA: 0x000375EC File Offset: 0x000357EC
			public override void OnArenaEnd(Combat combat)
			{
				int amount = combat.Kills.Count((Combat.Kill k) => k.KillerIsPlayer);
				base.AdvanceProgress(amount);
			}
		}

		// Token: 0x020001CF RID: 463
		public sealed class MAA_1000EliteKills : Feat
		{
			// Token: 0x170004D4 RID: 1236
			// (get) Token: 0x06000D57 RID: 3415 RVA: 0x00037634 File Offset: 0x00035834
			public override TextObject Description
			{
				get
				{
					return L.T("masters_at_arms_1000_elite_kills", "Get 1000 kills with elite troops.");
				}
			}

			// Token: 0x170004D5 RID: 1237
			// (get) Token: 0x06000D58 RID: 3416 RVA: 0x00037645 File Offset: 0x00035845
			public override int Target
			{
				get
				{
					return 1000;
				}
			}

			// Token: 0x06000D59 RID: 3417 RVA: 0x0003764C File Offset: 0x0003584C
			public override void OnBattleEnd(Battle battle)
			{
				int amount = battle.Kills.Count((Combat.Kill k) => k.KillerIsPlayerTroop && k.Killer.IsElite);
				base.AdvanceProgress(amount);
			}
		}
	}
}
