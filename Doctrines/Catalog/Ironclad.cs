using System;
using System.Collections.Generic;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E5 RID: 229
	public sealed class Ironclad : Doctrine
	{
		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06000927 RID: 2343 RVA: 0x0002D02B File Offset: 0x0002B22B
		public override TextObject Name
		{
			get
			{
				return L.T("ironclad", "Ironclad");
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06000928 RID: 2344 RVA: 0x0002D03C File Offset: 0x0002B23C
		public override TextObject Description
		{
			get
			{
				return L.T("ironclad_description", "No tier restriction for arms and armor.");
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06000929 RID: 2345 RVA: 0x0002D04D File Offset: 0x0002B24D
		public override int Column
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x0600092A RID: 2346 RVA: 0x0002D050 File Offset: 0x0002B250
		public override int Row
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x020001B8 RID: 440
		public sealed class IC_FullSetT6100Kills : Feat
		{
			// Token: 0x170004A6 RID: 1190
			// (get) Token: 0x06000CFA RID: 3322 RVA: 0x00036AA6 File Offset: 0x00034CA6
			public override TextObject Description
			{
				get
				{
					return L.T("ironclad_full_set_t6_100_kills", "Get 100 kills in battle with troops wearing a tier 6 armor and helmet.");
				}
			}

			// Token: 0x170004A7 RID: 1191
			// (get) Token: 0x06000CFB RID: 3323 RVA: 0x00036AB7 File Offset: 0x00034CB7
			public override int Target
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06000CFC RID: 3324 RVA: 0x00036ABC File Offset: 0x00034CBC
			public override void OnBattleEnd(Battle battle)
			{
				foreach (Combat.Kill kill in battle.Kills)
				{
					if (kill.KillerIsPlayerTroop)
					{
						WCharacter killer = kill.Killer;
						if (killer.IsCustom)
						{
							bool flag = true;
							foreach (WEquipment wequipment in killer.Loadout.Equipments)
							{
								if (wequipment.Get(EquipmentIndex.NumAllWeaponSlots).Tier < 6)
								{
									flag = false;
									break;
								}
								if (wequipment.Get(EquipmentIndex.Body).Tier < 6)
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								base.AdvanceProgress(1);
							}
						}
					}
				}
			}
		}

		// Token: 0x020001B9 RID: 441
		public sealed class IC_12TroopsAthletics90 : Feat
		{
			// Token: 0x170004A8 RID: 1192
			// (get) Token: 0x06000CFE RID: 3326 RVA: 0x00036BA8 File Offset: 0x00034DA8
			public override TextObject Description
			{
				get
				{
					return L.T("ironclad_12_troops_athletics_90", "Have 12 custom troops reach 90 in Athletics skill.");
				}
			}

			// Token: 0x170004A9 RID: 1193
			// (get) Token: 0x06000CFF RID: 3327 RVA: 0x00036BB9 File Offset: 0x00034DB9
			public override int Target
			{
				get
				{
					return 12;
				}
			}

			// Token: 0x06000D00 RID: 3328 RVA: 0x00036BC0 File Offset: 0x00034DC0
			public override void OnDailyTick()
			{
				int num = 0;
				using (IEnumerator<WCharacter> enumerator = Player.Troops.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.GetSkill(DefaultSkills.Athletics) >= 90)
						{
							num++;
						}
					}
				}
				base.SetProgress(num);
			}
		}

		// Token: 0x020001BA RID: 442
		public sealed class IC_100BattleOutnumberedLowTier : Feat
		{
			// Token: 0x170004AA RID: 1194
			// (get) Token: 0x06000D02 RID: 3330 RVA: 0x00036C28 File Offset: 0x00034E28
			public override TextObject Description
			{
				get
				{
					return L.T("ironclad_100_battle_outnumbered_low_tier", "Win a 100+ battle, while outnumbered, and fielding only custom troops of tier 1, 2, or 3.");
				}
			}

			// Token: 0x170004AB RID: 1195
			// (get) Token: 0x06000D03 RID: 3331 RVA: 0x00036C39 File Offset: 0x00034E39
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D04 RID: 3332 RVA: 0x00036C3C File Offset: 0x00034E3C
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
				if (battle.FriendlyTroopCount >= battle.EnemyTroopCount)
				{
					return;
				}
				foreach (WRosterElement wrosterElement in Player.Party.MemberRoster.Elements)
				{
					if (!wrosterElement.Troop.IsHero)
					{
						if (!wrosterElement.Troop.IsCustom)
						{
							return;
						}
						if (wrosterElement.Troop.Tier > 3)
						{
							return;
						}
					}
				}
				base.AdvanceProgress(1);
			}
		}
	}
}
