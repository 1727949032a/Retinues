using System;
using System.Collections.Generic;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000ED RID: 237
	public sealed class AdaptiveTraining : Doctrine
	{
		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06000957 RID: 2391 RVA: 0x0002D222 File Offset: 0x0002B422
		public override TextObject Name
		{
			get
			{
				return L.T("adaptive_training", "Adaptive Training");
			}
		}

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06000958 RID: 2392 RVA: 0x0002D233 File Offset: 0x0002B433
		public override TextObject Description
		{
			get
			{
				return L.T("adaptive_training_description", "Allow XP refunds.");
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06000959 RID: 2393 RVA: 0x0002D244 File Offset: 0x0002B444
		public override int Column
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x0600095A RID: 2394 RVA: 0x0002D247 File Offset: 0x0002B447
		public override int Row
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x0600095B RID: 2395 RVA: 0x0002D24A File Offset: 0x0002B44A
		public override bool IsDisabled
		{
			get
			{
				return Config.ForceXpRefunds || (Config.SkillXpCostPerPoint == 0 && Config.BaseSkillXpCost == 0);
			}
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x0002D275 File Offset: 0x0002B475
		public override TextObject DisabledMessage
		{
			get
			{
				if (!Config.ForceXpRefunds)
				{
					return L.T("adaptive_training_disabled_message_cost", "Disabled: no XP costs set by config.");
				}
				return L.T("adaptive_training_disabled_message_refund", "Disabled: XP refunds already enabled by config.");
			}
		}

		// Token: 0x020001D0 RID: 464
		public sealed class AT_150InEachSkill : Feat
		{
			// Token: 0x170004D6 RID: 1238
			// (get) Token: 0x06000D5B RID: 3419 RVA: 0x00037694 File Offset: 0x00035894
			public override TextObject Description
			{
				get
				{
					return L.T("adaptive_training_150_in_each_skill", "For each skill, have at least one custom troop with a level of 150 or higher.");
				}
			}

			// Token: 0x170004D7 RID: 1239
			// (get) Token: 0x06000D5C RID: 3420 RVA: 0x000376A5 File Offset: 0x000358A5
			public override int Target
			{
				get
				{
					return 8;
				}
			}

			// Token: 0x06000D5D RID: 3421 RVA: 0x000376A8 File Offset: 0x000358A8
			public override void OnDailyTick()
			{
				List<SkillObject> list = new List<SkillObject>();
				foreach (WCharacter wcharacter in Player.Troops)
				{
					foreach (KeyValuePair<SkillObject, int> keyValuePair in wcharacter.Skills)
					{
						if (!list.Contains(keyValuePair.Key) && keyValuePair.Value >= 150)
						{
							list.Add(keyValuePair.Key);
						}
					}
				}
				base.SetProgress(list.Count);
			}
		}

		// Token: 0x020001D1 RID: 465
		public sealed class AT_WinWithEvenSplit : Feat
		{
			// Token: 0x170004D8 RID: 1240
			// (get) Token: 0x06000D5F RID: 3423 RVA: 0x0003776C File Offset: 0x0003596C
			public override TextObject Description
			{
				get
				{
					return L.T("adaptive_training_win_with_even_split", "Win a battle against over 100 enemies using a party evenly split among infantry, cavalry and ranged clan troops");
				}
			}

			// Token: 0x170004D9 RID: 1241
			// (get) Token: 0x06000D60 RID: 3424 RVA: 0x0003777D File Offset: 0x0003597D
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D61 RID: 3425 RVA: 0x00037780 File Offset: 0x00035980
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.EnemyTroopCount < 100)
				{
					return;
				}
				if (battle.IsLost)
				{
					return;
				}
				WRoster memberRoster = Player.Party.MemberRoster;
				if ((double)memberRoster.InfantryRatio < 0.25)
				{
					return;
				}
				if ((double)memberRoster.ArchersRatio < 0.25)
				{
					return;
				}
				if ((double)memberRoster.CavalryRatio < 0.25)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001D2 RID: 466
		public sealed class AT_5Weapons : Feat
		{
			// Token: 0x170004DA RID: 1242
			// (get) Token: 0x06000D63 RID: 3427 RVA: 0x000377F5 File Offset: 0x000359F5
			public override TextObject Description
			{
				get
				{
					return L.T("adaptive_training_5_weapons", "In a single battle, get a kill using five different weapons classes.");
				}
			}

			// Token: 0x170004DB RID: 1243
			// (get) Token: 0x06000D64 RID: 3428 RVA: 0x00037806 File Offset: 0x00035A06
			public override int Target
			{
				get
				{
					return 5;
				}
			}

			// Token: 0x06000D65 RID: 3429 RVA: 0x0003780C File Offset: 0x00035A0C
			public override void OnBattleEnd(Battle battle)
			{
				HashSet<int> hashSet = new HashSet<int>();
				foreach (Combat.Kill kill in battle.Kills)
				{
					if (kill.KillerIsPlayer)
					{
						hashSet.Add(kill.BlowWeaponClass);
					}
				}
				base.SetProgress(Math.Max(base.Progress, hashSet.Count));
			}
		}
	}
}
