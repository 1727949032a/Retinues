using System;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E8 RID: 232
	public sealed class ArmedPeasantry : Doctrine
	{
		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x0600093A RID: 2362 RVA: 0x0002D0F5 File Offset: 0x0002B2F5
		public override TextObject Name
		{
			get
			{
				return L.T("armed_peasantry", "Armed Peasantry");
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x0600093B RID: 2363 RVA: 0x0002D106 File Offset: 0x0002B306
		public override TextObject Description
		{
			get
			{
				return L.T("armed_peasantry_description", "Unlocks villager troops.");
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x0600093C RID: 2364 RVA: 0x0002D117 File Offset: 0x0002B317
		public override int Column
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x0600093D RID: 2365 RVA: 0x0002D11A File Offset: 0x0002B31A
		public override int Row
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x0002D11D File Offset: 0x0002B31D
		public override bool IsDisabled
		{
			get
			{
				return Config.NoDoctrineRequirements;
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x0600093F RID: 2367 RVA: 0x0002D129 File Offset: 0x0002B329
		public override TextObject DisabledMessage
		{
			get
			{
				return L.T("armed_peasantry_disabled_message", "Disabled: special troops unlocked from config.");
			}
		}

		// Token: 0x020001C1 RID: 449
		public sealed class AP_DefendVillageOnlyCustom : Feat
		{
			// Token: 0x170004B8 RID: 1208
			// (get) Token: 0x06000D1E RID: 3358 RVA: 0x0003701D File Offset: 0x0003521D
			public override TextObject Description
			{
				get
				{
					return L.T("steadfast_soldiers_defend_village_only_custom", "Defend a village from a raid using only custom troops.");
				}
			}

			// Token: 0x170004B9 RID: 1209
			// (get) Token: 0x06000D1F RID: 3359 RVA: 0x0003702E File Offset: 0x0003522E
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D20 RID: 3360 RVA: 0x00037031 File Offset: 0x00035231
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (!battle.IsVillageRaid)
				{
					return;
				}
				if (!battle.PlayerIsDefender)
				{
					return;
				}
				if (Player.Party.MemberRoster.CustomRatio < 0.99f)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001C2 RID: 450
		public sealed class AP_HeadmanVillageQuests : Feat
		{
			// Token: 0x170004BA RID: 1210
			// (get) Token: 0x06000D22 RID: 3362 RVA: 0x00037075 File Offset: 0x00035275
			public override TextObject Description
			{
				get
				{
					return L.T("armed_peasantry_headman_quests", "Complete 3 quests for the headman of one of your villages.");
				}
			}

			// Token: 0x170004BB RID: 1211
			// (get) Token: 0x06000D23 RID: 3363 RVA: 0x00037086 File Offset: 0x00035286
			public override int Target
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x06000D24 RID: 3364 RVA: 0x0003708C File Offset: 0x0003528C
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
				if (settlement == null || !settlement.IsVillage)
				{
					return;
				}
				if (settlement.OwnerClan != Clan.PlayerClan)
				{
					return;
				}
				if (!hero.IsHeadman)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001C3 RID: 451
		public sealed class AP_LandownerVillageQuests : Feat
		{
			// Token: 0x170004BC RID: 1212
			// (get) Token: 0x06000D26 RID: 3366 RVA: 0x000370FD File Offset: 0x000352FD
			public override TextObject Description
			{
				get
				{
					return L.T("armed_peasantry_landowner_quests", "Complete 3 quests for a landowner of one of your villages.");
				}
			}

			// Token: 0x170004BD RID: 1213
			// (get) Token: 0x06000D27 RID: 3367 RVA: 0x0003710E File Offset: 0x0003530E
			public override int Target
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x06000D28 RID: 3368 RVA: 0x00037114 File Offset: 0x00035314
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
				if (settlement == null || !settlement.IsVillage)
				{
					return;
				}
				if (settlement.OwnerClan != Clan.PlayerClan)
				{
					return;
				}
				if (!hero.IsRuralNotable)
				{
					return;
				}
				if (hero.IsHeadman)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}
	}
}
