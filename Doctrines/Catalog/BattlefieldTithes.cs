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
	// Token: 0x020000DF RID: 223
	public sealed class BattlefieldTithes : Doctrine
	{
		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x060008FF RID: 2303 RVA: 0x0002CDCC File Offset: 0x0002AFCC
		public override TextObject Name
		{
			get
			{
				return L.T("battlefield_tithes", "Battlefield Tithes");
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000900 RID: 2304 RVA: 0x0002CDDD File Offset: 0x0002AFDD
		public override TextObject Description
		{
			get
			{
				return L.T("battlefield_tithes_description", "Unlock items from ally kills.");
			}
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000901 RID: 2305 RVA: 0x0002CDEE File Offset: 0x0002AFEE
		public override int Column
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x0002CDF1 File Offset: 0x0002AFF1
		public override int Row
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000903 RID: 2307 RVA: 0x0002CDF4 File Offset: 0x0002AFF4
		public override bool IsDisabled
		{
			get
			{
				return !Config.UnlockItemsFromKills || Config.AllEquipmentUnlocked;
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06000904 RID: 2308 RVA: 0x0002CE0E File Offset: 0x0002B00E
		public override TextObject DisabledMessage
		{
			get
			{
				if (!Config.AllEquipmentUnlocked)
				{
					return L.T("battlefield_tithes_disabled_message_unlocks_from_kills", "Disabled: unlocks from kills disabled by config.");
				}
				return L.T("battlefield_tithes_disabled_message_all_equipment", "Disabled: all equipment already unlocked by config.");
			}
		}

		// Token: 0x020001A6 RID: 422
		public sealed class BT_QuestForAlliedLord : Feat
		{
			// Token: 0x17000482 RID: 1154
			// (get) Token: 0x06000CB0 RID: 3248 RVA: 0x00035F34 File Offset: 0x00034134
			public override TextObject Description
			{
				get
				{
					return L.T("battlefield_tithes_quest_for_allied_lord", "Complete 5 quests for allied lords.");
				}
			}

			// Token: 0x17000483 RID: 1155
			// (get) Token: 0x06000CB1 RID: 3249 RVA: 0x00035F45 File Offset: 0x00034145
			public override int Target
			{
				get
				{
					return 5;
				}
			}

			// Token: 0x06000CB2 RID: 3250 RVA: 0x00035F48 File Offset: 0x00034148
			public override void OnQuestCompleted(Quest quest)
			{
				if (!quest.IsSuccessful)
				{
					return;
				}
				if (Player.Kingdom == null)
				{
					WHero giver = quest.Giver;
					if (((giver != null) ? giver.Hero.MapFaction.StringId : null) != Player.MapFaction.StringId)
					{
						return;
					}
					WHero giver2 = quest.Giver;
					if (giver2 == null || !giver2.IsPartyLeader)
					{
						return;
					}
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001A7 RID: 423
		public sealed class BT_LeadArmyVictory : Feat
		{
			// Token: 0x17000484 RID: 1156
			// (get) Token: 0x06000CB4 RID: 3252 RVA: 0x00035FC1 File Offset: 0x000341C1
			public override TextObject Description
			{
				get
				{
					return L.T("battlefield_tithes_lead_army_victory", "Lead an army of mostly allied troops to victory against an enemy army.");
				}
			}

			// Token: 0x17000485 RID: 1157
			// (get) Token: 0x06000CB5 RID: 3253 RVA: 0x00035FD2 File Offset: 0x000341D2
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CB6 RID: 3254 RVA: 0x00035FD5 File Offset: 0x000341D5
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (!Player.IsArmyLeader)
				{
					return;
				}
				if (battle.AllyTroopCount < battle.PlayerTroopCount)
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

		// Token: 0x020001A8 RID: 424
		public sealed class BT_TurnTideAlliedArmyBattle : Feat
		{
			// Token: 0x17000486 RID: 1158
			// (get) Token: 0x06000CB8 RID: 3256 RVA: 0x00036010 File Offset: 0x00034210
			public override TextObject Description
			{
				get
				{
					return L.T("battlefield_tithes_turn_tide_allied_army_battle", "Turn the tide of a battle involving an allied army.");
				}
			}

			// Token: 0x17000487 RID: 1159
			// (get) Token: 0x06000CB9 RID: 3257 RVA: 0x00036021 File Offset: 0x00034221
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CBA RID: 3258 RVA: 0x00036024 File Offset: 0x00034224
			public override void OnBattleStart(Battle battle)
			{
				BattlefieldTithes.BT_TurnTideAlliedArmyBattle.IsCandidate = false;
				if (battle.AllyTroopCount == 0)
				{
					return;
				}
				if ((double)battle.EnemyTroopCount < 1.25 * (double)battle.AllyTroopCount)
				{
					return;
				}
				if (!battle.AllyIsInArmy)
				{
					return;
				}
				BattlefieldTithes.BT_TurnTideAlliedArmyBattle.IsCandidate = true;
			}

			// Token: 0x06000CBB RID: 3259 RVA: 0x0003605F File Offset: 0x0003425F
			public override void OnBattleEnd(Battle battle)
			{
				if (!BattlefieldTithes.BT_TurnTideAlliedArmyBattle.IsCandidate)
				{
					return;
				}
				if (battle.IsLost)
				{
					return;
				}
				base.AdvanceProgress(1);
			}

			// Token: 0x0400051B RID: 1307
			private static bool IsCandidate;
		}
	}
}
