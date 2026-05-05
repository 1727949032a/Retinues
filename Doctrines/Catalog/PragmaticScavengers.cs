using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E0 RID: 224
	public sealed class PragmaticScavengers : Doctrine
	{
		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06000906 RID: 2310 RVA: 0x0002CE43 File Offset: 0x0002B043
		public override TextObject Name
		{
			get
			{
				return L.T("pragmatic_scavengers", "Pragmatic Scavengers");
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06000907 RID: 2311 RVA: 0x0002CE54 File Offset: 0x0002B054
		public override TextObject Description
		{
			get
			{
				return L.T("pragmatic_scavengers_description", "Unlock items from ally casualties.");
			}
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06000908 RID: 2312 RVA: 0x0002CE65 File Offset: 0x0002B065
		public override int Column
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000909 RID: 2313 RVA: 0x0002CE68 File Offset: 0x0002B068
		public override int Row
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x0600090A RID: 2314 RVA: 0x0002CE6B File Offset: 0x0002B06B
		public override bool IsDisabled
		{
			get
			{
				return !Config.UnlockItemsFromKills || Config.AllEquipmentUnlocked;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x0600090B RID: 2315 RVA: 0x0002CE85 File Offset: 0x0002B085
		public override TextObject DisabledMessage
		{
			get
			{
				if (!Config.AllEquipmentUnlocked)
				{
					return L.T("pragmatic_scavengers_disabled_message_unlocks_from_kills", "Disabled: unlocks from kills disabled by config.");
				}
				return L.T("pragmatic_scavengers_disabled_message_all_equipment", "Disabled: all equipment already unlocked by config.");
			}
		}

		// Token: 0x020001A9 RID: 425
		public sealed class PS_Allies100Casualties : Feat
		{
			// Token: 0x17000488 RID: 1160
			// (get) Token: 0x06000CBD RID: 3261 RVA: 0x00036082 File Offset: 0x00034282
			public override TextObject Description
			{
				get
				{
					return L.T("pragmatic_scavengers_defense_allies_100", "Win a battle in which allies suffer over 100 casualties.");
				}
			}

			// Token: 0x17000489 RID: 1161
			// (get) Token: 0x06000CBE RID: 3262 RVA: 0x00036093 File Offset: 0x00034293
			public override int Target
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06000CBF RID: 3263 RVA: 0x00036098 File Offset: 0x00034298
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (battle.AllyTroopCount == 0)
				{
					return;
				}
				int num = (from k in battle.Kills
				where k.VictimIsAllyTroop
				select k).Count<Combat.Kill>();
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
			}
		}

		// Token: 0x020001AA RID: 426
		public sealed class PS_AllyArmyWin3 : Feat
		{
			// Token: 0x1700048A RID: 1162
			// (get) Token: 0x06000CC1 RID: 3265 RVA: 0x000360FF File Offset: 0x000342FF
			public override TextObject Description
			{
				get
				{
					return L.T("pragmatic_scavengers_army_win_allies_50", "Win three battles in a row while part of an allied lord's army.");
				}
			}

			// Token: 0x1700048B RID: 1163
			// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x00036110 File Offset: 0x00034310
			public override int Target
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x06000CC3 RID: 3267 RVA: 0x00036113 File Offset: 0x00034313
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost || !battle.PlayerIsInArmy || Player.IsArmyLeader)
				{
					base.SetProgress(0);
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001AB RID: 427
		public sealed class PS_RescueLord : Feat
		{
			// Token: 0x1700048C RID: 1164
			// (get) Token: 0x06000CC5 RID: 3269 RVA: 0x00036144 File Offset: 0x00034344
			public override TextObject Description
			{
				get
				{
					return L.T("pragmatic_scavengers_rescue_lord", "Rescue a defeated lord from a enemy party's prisoner train.");
				}
			}

			// Token: 0x1700048D RID: 1165
			// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x00036155 File Offset: 0x00034355
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CC7 RID: 3271 RVA: 0x00036158 File Offset: 0x00034358
			public override void OnBattleStart(Battle battle)
			{
				PragmaticScavengers.PS_RescueLord.LordInCaptivity = false;
				using (List<WCharacter>.Enumerator enumerator = battle.EnemyPrisoners.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsHero)
						{
							PragmaticScavengers.PS_RescueLord.LordInCaptivity = true;
							break;
						}
					}
				}
			}

			// Token: 0x06000CC8 RID: 3272 RVA: 0x000361B8 File Offset: 0x000343B8
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (!PragmaticScavengers.PS_RescueLord.LordInCaptivity)
				{
					return;
				}
				base.AdvanceProgress(1);
			}

			// Token: 0x0400051C RID: 1308
			private static bool LordInCaptivity;
		}
	}
}
